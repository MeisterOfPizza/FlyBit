using FlyBit.UI;
using UnityEngine;

#pragma warning disable 0649

namespace FlyBit.Controllers
{

    class EffectsController : Controller<EffectsController>
    {

        #region Editor

        [Header("Invert Colors")]
        [SerializeField] private Camera         mainCamera;
        [SerializeField] private SpriteRenderer playerSpriteRenderer;
        [SerializeField] private UIColorInvert  gameScreen;
        [SerializeField] private UIColorInvert  deathScreen;
        [SerializeField] private UIColorInvert  resetGameButton;
        [SerializeField] private UIColorInvert  uiGainedScoreColorInvert;

        #endregion

        #region Private variables

        private bool invertEffectIsOn;

        #endregion

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
            gameScreen.SetColor(invertEffectIsOn);
            deathScreen.SetColor(invertEffectIsOn);
            resetGameButton.SetColor(invertEffectIsOn);
            uiGainedScoreColorInvert.SetColor(invertEffectIsOn);

            MapController.Singleton.SetMapColor(invertEffectIsOn ? Color.white : Color.black);
            PlayerController.Singleton.Invert = invertEffectIsOn;
        }

    }

}
