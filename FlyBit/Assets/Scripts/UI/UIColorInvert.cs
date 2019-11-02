using FlyBit.Extensions;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace FlyBit.UI
{

    sealed class UIColorInvert : MonoBehaviour
    {

        #region Editor

        [Header("References")]
        [SerializeField] private ColorOption[] colorOptions;

        #endregion

        #region Classes

        [Serializable]
        private class ColorOption
        {

            [SerializeField] private Color   defaultColor = Color.white;
            [SerializeField] private Graphic targetGraphic;

            public ColorOption(Color color, Graphic graphic)
            {
                this.defaultColor  = color;
                this.targetGraphic = graphic;
            }

            public void SetColor(bool invert)
            {
                targetGraphic.color = invert ? defaultColor.Invert() : defaultColor;
            }

        }

        #endregion

        public void AddColorOptions(Color defaultColor, params Graphic[] graphics)
        {
            var options = colorOptions.ToList();

            foreach (var graphic in graphics)
            {
                options.Add(new ColorOption(defaultColor, graphic));
            }

            colorOptions = options.ToArray();
        }

        public void SetColor(bool invert)
        {
            foreach (var option in colorOptions)
            {
                option.SetColor(invert);
            }
        }

    }

}
