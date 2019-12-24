using FlyBit.Extensions;
using System;
using System.Collections.Generic;
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

        #region Private variables

        private HashSet<ColorOption> colorOptionsCollection = new HashSet<ColorOption>();

        private bool awakeCalled;

        #endregion

        #region Classes

        [Serializable]
        private class ColorOption
        {

            [SerializeField] private Color   defaultColor = Color.white;
            [SerializeField] private Graphic targetGraphic;

            public Graphic Graphic
            {
                get
                {
                    return targetGraphic;
                }
            }

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

        private void Awake()
        {
            if (!awakeCalled)
            {
                foreach (var option in colorOptions)
                {
                    colorOptionsCollection.Add(option);
                }

                awakeCalled = true;
            }
        }

        public void AddColorOption(Color defaultColor, Graphic graphic)
        {
            colorOptionsCollection.Add(new ColorOption(defaultColor, graphic));
        }

        public void AddColorOptions(Color defaultColor, params Graphic[] graphics)
        {
            foreach (var graphic in graphics)
            {
                colorOptionsCollection.Add(new ColorOption(defaultColor, graphic));
            }
        }

        public void RemoveColorOption(Graphic graphic)
        {
            var option = colorOptionsCollection.FirstOrDefault(c => c.Graphic == graphic);

            colorOptionsCollection.Remove(option);
        }

        public void ClearColorOptions()
        {
            colorOptionsCollection.Clear();
        }

        public void SetColor(bool invert)
        {
            if (!awakeCalled)
            {
                Awake();
            }

            foreach (var option in colorOptionsCollection)
            {
                option.SetColor(invert);
            }
        }

    }

}
