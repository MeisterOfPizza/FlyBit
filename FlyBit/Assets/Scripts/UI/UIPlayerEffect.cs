using TMPro;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace FlyBit.UI
{

    sealed class UIPlayerEffect : MonoBehaviour
    {

        #region Editor

        [Header("References")]
        [SerializeField] private Image    iconOutline;
        [SerializeField] private Image    icon;
        [SerializeField] private TMP_Text durationText;

        #endregion

        #region Public properties

        public Graphic[] TargetGraphics
        {
            get
            {
                return new Graphic[] { iconOutline, icon, durationText };
            }
        }

        public UIColorInvert.ColorOption[] ColorOptions
        {
            get
            {
                return new UIColorInvert.ColorOption[]
                {
                    new UIColorInvert.ColorOption(Color.black, iconOutline),
                    new UIColorInvert.ColorOption(Color.white, icon),
                    new UIColorInvert.ColorOption(Color.white, durationText)
                };
            }
        }

        #endregion

        public void Initialize(Sprite icon, float duration)
        {
            this.icon.sprite        = icon;
            this.iconOutline.sprite = icon;
            SetDurationText(duration);
        }

        public void SetDurationText(float duration)
        {
            durationText.text = duration.ToString("F1") + "s";
        }

    }

}
