using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace FlyBit.UI
{

    sealed class UIButtonColorInvert : Graphic
    {

        #region Editor

        [Header("References")]
        [SerializeField] private Button button;

        #endregion

        #region Public properties

        public override Color color
        {
            get
            {
                return this.buttonColor;
            }

            set
            {
                this.buttonColor = value;

                UpdateButtonColors();
            }
        }

        #endregion

        #region Private variables

        private Color buttonColor;

        #endregion

        private void UpdateButtonColors()
        {
            var colors = button.colors;

            colors.normalColor = buttonColor;
            button.colors      = colors;
        }

        public override bool IsActive()
        {
            return false;
        }

    }

}
