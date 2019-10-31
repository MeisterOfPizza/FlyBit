using TMPro;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace FlyBit.Controllers
{

    class EffectsController : Controller<EffectsController>
    {

        #region Editor

        [Header("Invert Colors")]
        [SerializeField] private TMP_Text       scoreText;
        [SerializeField] private TMP_Text       timeAliveText;
        [SerializeField] private TMP_Text       distanceTraveledText;
        [SerializeField] private Camera         mainCamera;
        [SerializeField] private SpriteRenderer playerSpriteRenderer;
        [SerializeField] private Image[]        heartImages;

        #endregion

        #region Private variables

        private bool invertEffectIsOn;

        #endregion

        public void ToggleInvertEffect()
        {
            invertEffectIsOn = !invertEffectIsOn;

            scoreText.color            = invertEffectIsOn ? Color.black : Color.white;
            timeAliveText.color        = invertEffectIsOn ? Color.black : Color.white;
            distanceTraveledText.color = invertEffectIsOn ? Color.white : Color.black;
            mainCamera.backgroundColor = invertEffectIsOn ? Color.black : Color.white;
            playerSpriteRenderer.color = invertEffectIsOn ? Color.white : Color.black;

            foreach (var heartImage in heartImages) { heartImage.color = invertEffectIsOn ? Color.black : Color.white; }

            MapController.Singleton.SetMapColor(invertEffectIsOn ? Color.white : Color.black);
            PlayerController.Singleton.Invert = invertEffectIsOn;
        }

    }

}
