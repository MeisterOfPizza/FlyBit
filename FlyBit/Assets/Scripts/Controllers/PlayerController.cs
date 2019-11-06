using FlyBit.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private Camera         mainCamera;
        [SerializeField] private Animator       animator;
        [SerializeField] private GameObject     playerModel;
        [SerializeField] private SpriteRenderer fuelBar;
        [SerializeField] private ParticleSystem crashParticleSystem;
        [SerializeField] private Collider2D     playerCollider;

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
        [SerializeField] private float hyperdriveMoveSpeed = 3f;

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

        public bool IsDead { get; private set; } = true;

        public bool IsSpawning
        {
            get
            {
                return isSpawning;
            }
        }

        public bool IsReviving
        {
            get
            {
                return isReviving;
            }
        }

        /// <summary>
        /// Should the physics be inverted on the player?
        /// </summary>
        public bool Invert { get; set; } = false;

        public bool HasDubblePoints
        {
            get
            {
                return playerEffects.ContainsKey(PlayerEffect.DubblePoints);
            }
        }

        #endregion

        #region Private variables

        private Dictionary<PlayerEffect, float> playerEffects = new Dictionary<PlayerEffect, float>();

        private bool  isSpawning;
        private bool  isReviving;
        private bool  isThrusting;
        private float fuel;
        private float moveSpeed;
        private float turnSpeed;
        private int   livesLeft;

        #endregion

        #region Enums

        public enum PlayerEffect
        {
            InfiniteFuel,
            DubblePoints,
            Hyperdrive,
            HyperdriveController
        }

        #endregion

        #region MonoBehaviour

        private void Update()
        {
            if (GameController.Singleton.IsMatchRunning && !IsDead && !isSpawning && !isReviving)
            {
                UpdatePlayerEffects();

                // The player should not be able to move nor fall if they are traveling by hyperdrive:
                if (!playerEffects.ContainsKey(PlayerEffect.Hyperdrive))
                {
                    CheckIfShouldThrust();

                    if (!isThrusting)
                    {
                        Fall();

                        // Refill fuel
                        AddFuel(Time.deltaTime);
                    }
                    else
                    {
                        Thurst();
                    }
                }
                else
                {
                    if (playerEffects.ContainsKey(PlayerEffect.HyperdriveController))
                    {
                        AimHyperdrive();
                    }
                }
            }
        }

        #endregion

        #region Player life cycle

        public void ResetPlayer()
        {
            ClearPlayerEffects();

            moveSpeed = fallMoveSpeed;
            turnSpeed = fallTurnSpeed;

            AddFuel(maxFuel);

            isSpawning = true;
            isReviving = false;
            IsDead     = false;

            livesLeft = MAX_LIVES;

            transform.position = Vector3.zero;
            transform.right    = Vector3.right;

            playerCollider.enabled = false;

            crashParticleSystem.Stop();
            crashParticleSystem.Clear();

            playerModel.gameObject.SetActive(true);

            for (int i = 0; i < MAX_LIVES; i++)
            {
                heartIcons[i].gameObject.SetActive(true);
            }

            DifficultyController.Singleton.Pause(true);
            ScoreController.Singleton.PauseTimeAlive(true);

            StartCoroutine("SpawnEffect");
        }

        private IEnumerator SpawnEffect()
        {
            animator.Play("Revive_Blink");

            float timeLeft = spawnInvisibilityTime;

            while (timeLeft > 0f)
            {
                UpdateFuelBar(1f - timeLeft / spawnInvisibilityTime);

                timeLeft -= Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            animator.Play("Rest");

            DifficultyController.Singleton.Pause(false);
            ScoreController.Singleton.PauseTimeAlive(false);

            isSpawning             = false;
            playerCollider.enabled = true;
        }

        private void Crash()
        {
            if (!isSpawning && !isReviving && !IsDead && !playerEffects.ContainsKey(PlayerEffect.Hyperdrive))
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
            ClearPlayerEffects();

            moveSpeed  = fallMoveSpeed;
            turnSpeed  = fallTurnSpeed;
            isReviving = true;

            AddFuel(maxFuel);

            playerCollider.enabled = false;

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

            crashParticleSystem.Stop();
            crashParticleSystem.Clear();

            transform.position = Vector3.zero;
            transform.right    = Vector3.right;

            MapController.Singleton.RebuildMap(0f);

            float timeLeft = reviveInvisibilityTime;

            while (timeLeft > 0f)
            {
                UpdateFuelBar(1f - timeLeft / reviveInvisibilityTime);

                timeLeft -= Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            animator.Play("Rest");

            DifficultyController.Singleton.Pause(false);
            ScoreController.Singleton.PauseTimeAlive(false);

            isReviving             = false;
            playerCollider.enabled = true;
        }

        private void Die()
        {
            playerModel.gameObject.SetActive(false);
            playerCollider.enabled = false;

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

                if (shouldThrust && !isThrusting && !playerEffects.ContainsKey(PlayerEffect.InfiniteFuel))
                {
                    AddFuel(-initialThrustFuelConsumption);
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
            if (!playerEffects.ContainsKey(PlayerEffect.InfiniteFuel))
            {
                AddFuel(-Time.deltaTime);
            }

            moveSpeed = Mathf.Lerp(moveSpeed, thrustMoveSpeed, moveLerpSpeed * Time.deltaTime);
            turnSpeed = Mathf.Lerp(turnSpeed, thrustTurnSpeed, turnLerpSpeed * Time.deltaTime);

            transform.right = Vector3.Lerp(transform.right, Invert ? Vector3.down : Vector3.up, turnSpeed * Time.deltaTime);

            MapController.Singleton.MoveMap(transform.right * moveSpeed * Time.deltaTime);

            ScoreController.Singleton.AddDistanceTraveled(transform.right.magnitude * moveSpeed * Time.deltaTime);
        }

        #endregion

        #region Falling

        private void Fall()
        {
            moveSpeed = Mathf.Lerp(moveSpeed, fallMoveSpeed, moveLerpSpeed * Time.deltaTime);
            turnSpeed = Mathf.Lerp(turnSpeed, fallTurnSpeed, turnLerpSpeed * Time.deltaTime);

            transform.right = Vector3.Lerp(transform.right, Invert ? Vector3.up : Vector3.down, turnSpeed * Time.deltaTime);

            MapController.Singleton.MoveMap(transform.right * moveSpeed * Time.deltaTime);

            ScoreController.Singleton.AddDistanceTraveled(transform.right.magnitude * moveSpeed * Time.deltaTime);
        }

        #endregion

        #region Hyperdrive

        private void AimHyperdrive()
        {
            float direction = 0f;

#if UNITY_STANDALONE
            if (!MathE.IsPointerOverUIObject(Input.mousePosition) && Input.GetMouseButton(0))
            {
                Vector2 point = mainCamera.ScreenToWorldPoint(Input.mousePosition);

                direction = Mathf.Clamp(point.y - transform.position.y, -1f, 1f);
            }
            else
            {
                if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                {
                    direction = 1f;
                }
                else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                {
                    direction = -1f;
                }
            }
#elif UNITY_IOS || UNITY_ANDROID

#endif

            transform.position += Vector3.up * hyperdriveMoveSpeed * direction * Time.deltaTime;
            transform.position  = new Vector3(0f, Mathf.Clamp(transform.position.y, -MapController.Singleton.ScreenHeight / 2f, MapController.Singleton.ScreenHeight / 2f));
        }

        #endregion

        #region Collisions

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Crash();
        }

        #endregion

        #region Player effects

        public void AddPlayerEffect(PlayerEffect playerEffect, float duration)
        {
            playerEffects[playerEffect] = duration;
        }

        public void RemovePlayerEffect(PlayerEffect playerEffect)
        {
            playerEffects.Remove(playerEffect);
        }

        private void UpdatePlayerEffects()
        {
            foreach (var effect in playerEffects.ToList())
            {
                if (effect.Value <= 0)
                {
                    RemovePlayerEffect(effect.Key);
                }
                else
                {
                    playerEffects[effect.Key] -= Time.deltaTime;

                    if (effect.Key == PlayerEffect.InfiniteFuel)
                    {
                        ScoreController.Singleton.AddStatRecordValue(ScoreController.StatRecordType.InfiniteFuelDuration, Time.deltaTime);
                    }
                }
            }
        }

        private void ClearPlayerEffects()
        {
            playerEffects.Clear();
        }

        #endregion

        #region Helpers

        public void AddFuel(float delta)
        {
            fuel = Mathf.Clamp(fuel + delta, 0f, maxFuel);

            UpdateFuelBar(fuel / maxFuel);
        }

        public void UpdateFuelBar(float percent)
        {
            fuelBar.material.SetFloat("_FillAmount", percent);
            fuelBar.color = fuelGradient.Evaluate(percent);
        }

        #endregion

    }

}
