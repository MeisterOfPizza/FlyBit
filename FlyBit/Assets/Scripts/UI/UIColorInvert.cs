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
        [SerializeField] private ColorOption[]  colorOptions;
        [SerializeField] private ButtonOption[] buttonOptions;

        #endregion

        #region Private variables

        private HashSet<ColorOption>  colorOptionsCollection  = new HashSet<ColorOption>();
        private HashSet<ButtonOption> buttonOptionsCollection = new HashSet<ButtonOption>();

        private bool awakeCalled;

        #endregion

        #region Classes

        [Serializable]
        public class ColorOption
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

        [Serializable]
        public class ButtonOption
        {

            [SerializeField] private ColorBlock defaultColorBlock = ColorBlock.defaultColorBlock;
            [SerializeField] private Button     targetButton;

            public Button Button
            {
                get
                {
                    return targetButton;
                }
            }

            public void SetColor(bool invert)
            {
                ColorBlock colorBlock = new ColorBlock()
                {
                    normalColor      = invert ? defaultColorBlock.normalColor.Invert() : defaultColorBlock.normalColor,
                    highlightedColor = invert ? defaultColorBlock.highlightedColor.Invert() : defaultColorBlock.highlightedColor,
                    pressedColor     = invert ? defaultColorBlock.pressedColor.Invert() : defaultColorBlock.pressedColor,
                    selectedColor    = invert ? defaultColorBlock.selectedColor.Invert() : defaultColorBlock.selectedColor,
                    disabledColor    = invert ? defaultColorBlock.disabledColor.Invert() : defaultColorBlock.disabledColor,
                    colorMultiplier  = defaultColorBlock.colorMultiplier,
                    fadeDuration     = defaultColorBlock.fadeDuration
                };

                targetButton.colors = colorBlock;
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

                foreach (var button in buttonOptions)
                {
                    buttonOptionsCollection.Add(button);
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

        public void AddColorOptions(ColorOption[] colorOptions)
        {
            foreach (var option in colorOptions)
            {
                colorOptionsCollection.Add(option);
            }
        }

        public void RemoveColorOption(Graphic graphic)
        {
            var option = colorOptionsCollection.FirstOrDefault(c => c.Graphic == graphic);

            colorOptionsCollection.Remove(option);
        }

        public void ClearOptions()
        {
            colorOptionsCollection.Clear();
            buttonOptionsCollection.Clear();
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

            foreach (var button in buttonOptionsCollection)
            {
                button.SetColor(invert);
            }
        }

    }

}
