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
        [SerializeField] private Image    iconImage;
        [SerializeField] private TMP_Text durationText;

        #endregion

        #region Public properties

        public Graphic[] TargetGraphics
        {
            get
            {
                return new Graphic[] { iconImage, durationText };
            }
        }

        #endregion

        public void Initialize(Sprite icon, float duration)
        {
            iconImage.sprite = icon;
            SetDurationText(duration);
        }

        public void SetDurationText(float duration)
        {
            durationText.text = duration.ToString("F1") + "s";
        }

    }

}
