using FlyBit.Extensions;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace FlyBit.Controllers
{

    class PlayerController : Controller<PlayerController>
    {

        #region Constants

        private const int MAX_LIVES = 3;

        #endregion

        #region Editor

        [Header("References")]
        [SerializeField] private Animator       animator;
        [SerializeField] private GameObject     playerModel;
        [SerializeField] private SpriteRenderer fuelBar;
        [SerializeField] private ParticleSystem crashParticleSystem;

        [Header("UI References")]
        [SerializeField] private Image[] heartIcons = new Image[MAX_LIVES];

        [Header("Values")]
        [SerializeField] private float thrustMoveSpeed = 10.5f;
        [SerializeField] private float fallMoveSpeed   = 7.5f;
        [SerializeField] private float moveLerpSpeed   = 1.5f;

        [Space]
        [SerializeField] private float thrustTurnSpeed = 5f;
        [SerializeField] private float fallTurnSpeed   = 3f;
        [SerializeField] private float turnLerpSpeed   = 5f;

        [Space]
        [SerializeField] private float initialThrustFuelConsumption = 0.1f;
        [SerializeField] private float maxFuel                      = 5f;

        [Space]
        [SerializeField] private float spawnInvisibilityTime  = 1.0f;
        [SerializeField] private float reviveInvisibilityTime = 1.5f;

        [Space]
        [SerializeField] private Gradient fuelGradient;

        #endregion

        #region Public properties

        public bool IsDead { get; private set; } = false;

        /// <summary>
        /// Should the physics be inverted on the player?
        /// </summary>
        public bool Invert { get; set; } = false;

        #endregion

        #region Private variables

        private bool isSpawning;
        private bool isReviving;

        private bool  isThrusting;
        private float fuel;
        private float moveSpeed;
        private float turnSpeed;
        private int   livesLeft;

        #endregion

        #region MonoBehaviour

        private void Update()
        {
            if (GameController.Singleton.IsMatchRunning && !IsDead && !isSpawning && !isReviving)
            {
                CheckIfShouldThrust();

                if (!isThrusting)
                {
                    Fall();

                    fuel = Mathf.Clamp(fuel + Time.deltaTime, 0f, maxFuel);
                }
                else
                {
                    Thurst();
                }

                fuelBar.material.SetFloat("_FillAmount", fuel / maxFuel);
                fuelBar.color = fuelGradient.Evaluate(fuel / maxFuel);
            }
        }

        #endregion

        #region Player life cycle

        public void ResetPlayer()
        {
            fuel      = maxFuel;
            moveSpeed = fallMoveSpeed;
            turnSpeed = fallTurnSpeed;

            fuelBar.material.SetFloat("_FillAmount", fuel / maxFuel);
            fuelBar.color = fuelGradient.Evaluate(fuel / maxFuel);

            isReviving = false;
            IsDead     = false;

            livesLeft = MAX_LIVES;

            transform.position = Vector3.zero;
            transform.right    = Vector3.right;

            crashParticleSystem.Stop();
            crashParticleSystem.Clear();

            playerModel.gameObject.SetActive(true);

            for (int i = 0; i < MAX_LIVES; i++)
            {
                heartIcons[i].gameObject.SetActive(true);
            }

            isSpawning = true;

            ScoreController.Singleton.PauseTimeAlive(true);

            StartCoroutine("SpawnEffect");
        }

        private IEnumerator SpawnEffect()
        {
            animator.Play("Revive_Blink");

            float timeLeft = spawnInvisibilityTime;

            while (timeLeft > 0f)
            {
                fuelBar.material.SetFloat("_FillAmount", 1f - timeLeft / spawnInvisibilityTime);
                fuelBar.color = fuelGradient.Evaluate(1f - timeLeft / spawnInvisibilityTime);

                timeLeft -= Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            animator.Play("Rest");

            ScoreController.Singleton.PauseTimeAlive(false);

            isSpawning = false;
        }

        private void Crash()
        {
            if (!isSpawning && !isReviving && !IsDead)
            {
                if (livesLeft > 0)
                {
                    heartIcons[livesLeft - 1].gameObject.SetActive(false);
                }

                livesLeft--;

                var main = crashParticleSystem.main;
                main.startColor = Invert ? Color.white : Color.black;
                crashParticleSystem.Play();

                if (livesLeft >= 0)
                {
                    Revive();
                }
                else
                {
                    Die();
                }
            }
        }

        private void Revive()
        {
            moveSpeed  = fallMoveSpeed;
            turnSpeed  = fallTurnSpeed;
            fuel       = maxFuel;
            isReviving = true;

            playerModel.SetActive(false);

            DifficultyController.Singleton.Pause(true);
            ScoreController.Singleton.PauseTimeAlive(true);

            StartCoroutine("ReviveEffect");
        }

        private IEnumerator ReviveEffect()
        {
            yield return new WaitForSeconds(0.5f);

            playerModel.SetActive(true);
            animator.Play("Revive_Blink");

            transform.position = Vector2.zero;
            transform.right    = Vector3.right;
            MapController.Singleton.RebuildMap();

            float timeLeft = reviveInvisibilityTime;

            while (timeLeft > 0f)
            {
                fuelBar.material.SetFloat("_FillAmount", 1f - timeLeft / reviveInvisibilityTime);
                fuelBar.color = fuelGradient.Evaluate(1f - timeLeft / reviveInvisibilityTime);

                timeLeft -= Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            animator.Play("Rest");

            DifficultyController.Singleton.Pause(false);
            ScoreController.Singleton.PauseTimeAlive(false);

            isReviving = false;
        }

        private void Die()
        {
            playerModel.gameObject.SetActive(false);

            IsDead = true;

            GameController.Singleton.EndMatch();
        }

        public void GiveLife()
        {
            if (livesLeft < MAX_LIVES)
            {
                livesLeft++;

                heartIcons[livesLeft - 1].gameObject.SetActive(true);
            }
        }

        #endregion

        #region Thrusting

        private void CheckIfShouldThrust()
        {
            if (fuel >= initialThrustFuelConsumption)
            {
                bool shouldThrust = false;

#if UNITY_STANDALONE
                shouldThrust = !MathE.IsPointerOverUIObject(Input.mousePosition) && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow));
#elif UNITY_IOS || UNITY_ANDROID

#endif

                if (shouldThrust && !isThrusting)
                {
                    fuel = Mathf.Clamp(fuel - initialThrustFuelConsumption, 0f, maxFuel);
                }

                isThrusting = shouldThrust;
            }
            else
            {
                isThrusting = false;
            }
        }

        private void Thurst()
        {
            fuel = Mathf.Clamp(fuel - Time.deltaTime, 0f, maxFuel);

            moveSpeed = Mathf.Lerp(moveSpeed, thrustMoveSpeed, moveLerpSpeed * Time.deltaTime);
            turnSpeed = Mathf.Lerp(turnSpeed, thrustTurnSpeed, turnLerpSpeed * Time.deltaTime);

            transform.right     = Vector3.Lerp(transform.right, Invert ? Vector3.down : Vector3.up, turnSpeed * Time.deltaTime);
            transform.position += transform.right * moveSpeed * Time.deltaTime;

            ScoreController.Singleton.AddDistanceTraveled(transform.right.magnitude * moveSpeed * Time.deltaTime);
        }

        #endregion

        #region Falling

        private void Fall()
        {
            moveSpeed = Mathf.Lerp(moveSpeed, fallMoveSpeed, moveLerpSpeed * Time.deltaTime);
            turnSpeed = Mathf.Lerp(turnSpeed, fallTurnSpeed, turnLerpSpeed * Time.deltaTime);

            transform.right     = Vector3.Lerp(transform.right, Invert ? Vector3.up : Vector3.down, turnSpeed * Time.deltaTime);
            transform.position += transform.right * moveSpeed * Time.deltaTime;

            ScoreController.Singleton.AddDistanceTraveled(transform.right.magnitude * moveSpeed * Time.deltaTime);
        }

        #endregion

        #region Collisions

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Crash();
        }

        #endregion

    }

}
