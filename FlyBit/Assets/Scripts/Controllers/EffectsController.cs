using FlyBit.Map;
using FlyBit.Templates;
using FlyBit.UI;
using System.Collections;
using TMPro;
using UnityEngine;

#pragma warning disable 0649

namespace FlyBit.Controllers
{

    public class EffectsController : Controller<EffectsController>
    {

        #region Editor

        [Header("References - Main")]
        [SerializeField] private Camera         mainCamera;
        [SerializeField] private SpriteRenderer playerSpriteRenderer;

        [Header("References - Invert Colors")]
        [SerializeField] private UIColorInvert menuScreen;
        [SerializeField] private UIColorInvert pauseScreen;
        [SerializeField] private UIColorInvert settingsScreen;
        [SerializeField] private UIColorInvert gameScreen;
        [SerializeField] private UIColorInvert deathScreen;
        [SerializeField] private UIColorInvert resetGameButton;
        [SerializeField] private UIColorInvert uiGainedScoreColorInvert;
        [SerializeField] private UIColorInvert uiPlayerEffectsColorInvert;

        [Space]
        [SerializeField] private TMP_FontAsset textFontAsset;

        [SerializeField] private Texture2D mouseIconBlack;
        [SerializeField] private Texture2D mouseIconWhite;

        [Header("References - Hyperdrive")]
        [SerializeField] private Transform    playerTransform;
        [SerializeField] private Animator     playerAnimator;
        [SerializeField] private Collider2D   playerCollider;
        [SerializeField] private UISpeedlines hyperdriveSpeedlines;

        [Space]
        [SerializeField] private HyperdriveWallSection hyperdriveWallSection;

        [Space]
        [SerializeField] private SectionTemplate hyperdriveEntrySectionTemplate;

        [Header("Values - Hyperdrive")]
        [SerializeField] private float hyperdriveAnimationTime = 1f;
        [SerializeField] private float hyperdriveTravelTime    = 1f;
        [SerializeField] private float hyperdriveWaitTime      = 0.25f;

        [Space]
        [SerializeField] private float hyperdriveMinDistance = 250f;
        [SerializeField] private float hyperdriveMaxDistance = 500f;

        #endregion

        #region Private variables

        private bool invertEffectIsOn;

        #endregion

        public override void OnAwake()
        {
            ResetAllEffects();
        }

        public void ResetAllEffects()
        {
            invertEffectIsOn = true;
            ToggleInvertEffect();
        }

        public void ToggleInvertEffect()
        {
            invertEffectIsOn = !invertEffectIsOn;

            mainCamera.backgroundColor = invertEffectIsOn ? Color.black : Color.white;
            playerSpriteRenderer.color = invertEffectIsOn ? Color.white : Color.black;

            menuScreen.SetColor(invertEffectIsOn);
            pauseScreen.SetColor(invertEffectIsOn);
            settingsScreen.SetColor(invertEffectIsOn);
            gameScreen.SetColor(invertEffectIsOn);
            deathScreen.SetColor(invertEffectIsOn);
            resetGameButton.SetColor(invertEffectIsOn);
            uiGainedScoreColorInvert.SetColor(invertEffectIsOn);
            uiPlayerEffectsColorInvert.SetColor(invertEffectIsOn);

            textFontAsset.material.SetColor(ShaderUtilities.ID_UnderlayColor, invertEffectIsOn ? Color.white : Color.black);

            Cursor.SetCursor(invertEffectIsOn ? mouseIconWhite : mouseIconBlack, Vector2.zero, CursorMode.Auto);

            hyperdriveSpeedlines.InvertSpeedlineColors(invertEffectIsOn);
            hyperdriveWallSection.SetColor(invertEffectIsOn ? Color.white : Color.black);

            MapController.Singleton.SetMapColor(invertEffectIsOn ? Color.white : Color.black);
            PlayerController.Singleton.Invert = invertEffectIsOn;
        }

        public void HyperdriveTravel()
        {
            // Reset the fuel:
            PlayerController.Singleton.AddFuel(float.MaxValue / 2f);
            PlayerEffectsController.Singleton.AddPlayerEffect(PlayerEffect.Hyperdrive, float.MaxValue);

            MapController.Singleton.OpenCloseMap(true);

            StartCoroutine("HyperdriveEffect");
        }

        private IEnumerator HyperdriveEffect()
        {
            float animationTime = hyperdriveAnimationTime;

            while (animationTime > 0f)
            {
                animationTime -= Time.deltaTime;

                playerTransform.right = Vector3.Lerp(playerTransform.right, Vector3.right, 1f - animationTime / hyperdriveAnimationTime);

                yield return new WaitForEndOfFrame();
            }

            playerAnimator.Play("Hyperdrive_Begin");

            CameraEffectsController.Singleton.PlayHyperdriveZoom(0.5f, 1.5f);

            yield return new WaitForSeconds(0.5f);

            // Give the player control over the hyperdrive travel, giving them the ability to collect score.
            PlayerEffectsController.Singleton.AddPlayerEffect(PlayerEffect.HyperdriveController, float.MaxValue);

            // Play the hyperdrive travel effects:
            hyperdriveSpeedlines.Play();
            hyperdriveWallSection.Play();

            float travelTimeLeft  = hyperdriveTravelTime;
            float travelDistance  = Random.Range(hyperdriveMinDistance, hyperdriveMaxDistance);
            bool  hasBegunExiting = false;

            while (travelTimeLeft > 0f)
            {
                travelTimeLeft -= Time.deltaTime;

                // Increase performance by storing the frameDistance to avoid duplicate calculations:
                float frameDistance = travelDistance * (Time.deltaTime / hyperdriveTravelTime);

                // Update stats:
                ScoreController.Singleton.AddDistanceTraveled(frameDistance);
                ScoreController.Singleton.AddStatRecordValue(ScoreController.StatRecordType.HyperdriveDistanceTraveled, frameDistance);

                // Modify the fuel bar to represent time left on hyperdrive travel:
                PlayerController.Singleton.UpdateFuelBar(travelTimeLeft / hyperdriveTravelTime);

                if (travelTimeLeft <= 1.2f && !hasBegunExiting)
                {
                    playerAnimator.SetTrigger("Exit Hyperdrive");

                    hasBegunExiting = true;
                }

                yield return new WaitForEndOfFrame();
            }

            // The hyperdrive travel is done, stop all effects:
            hyperdriveSpeedlines.Stop();
            hyperdriveWallSection.Stop();

            playerCollider.enabled = false;

            // Remove the hyperdrive travel controller effect:
            PlayerEffectsController.Singleton.RemovePlayerEffect(PlayerEffect.HyperdriveController);

            // Rebuild the map:
            MapController.Singleton.RebuildMap(MapController.Singleton.PlayerSeeRadius * 3f, hyperdriveEntrySectionTemplate);

            // Cool off the engine, aka let the player drift into the map again.
            float coolOffWaitTime = hyperdriveWaitTime;

            while (coolOffWaitTime > 0f)
            {
                coolOffWaitTime -= Time.deltaTime;

                playerTransform.position = Vector3.Lerp(playerTransform.position, Vector3.zero, 1f - coolOffWaitTime / hyperdriveWaitTime);

                PlayerController.Singleton.UpdateFuelBar(1f - coolOffWaitTime / hyperdriveWaitTime);

                MapController.Singleton.MoveMap(new Vector3(15f, 0f) * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }

            // Actually pool the walls:
            hyperdriveWallSection.ResetSection();

            // Give back control of the player to the user:
            PlayerEffectsController.Singleton.RemovePlayerEffect(PlayerEffect.Hyperdrive);
            playerCollider.enabled = true;
        }

    }

}
