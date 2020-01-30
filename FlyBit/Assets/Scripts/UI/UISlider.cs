using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#pragma warning disable 0649

namespace FlyBit.UI
{

    [DisallowMultipleComponent]
    sealed class UISlider : Graphic, IPointerDownHandler, IDragHandler
    {

        #region Editor

        [Header("References")]
        [SerializeField] private RectTransform fillerContainer;
        [SerializeField] private Image         fillerImage;

        [Header("Values")]
        [SerializeField, Range(0f, 1f)] private float value = 0.0f;

        [Space]
        [SerializeField] private bool verticalFill = true;

        [Space]
        [SerializeField] private bool isIntegerSlider = false;
        [SerializeField] private int  maxIntegerValue = 2;

        [Space]
        [SerializeField] private FloatEvent   onValueSet;
        [SerializeField] private IntegerEvent onIntValueSet;

        #endregion

        #region Public properties

        public float FloatValue
        {
            get
            {
                return value;
            }
        }

        public int IntegerValue
        {
            get
            {
                return Mathf.RoundToInt(value * maxIntegerValue);
            }
        }

        #endregion

        #region Private classes

        [Serializable]
        private class FloatEvent : UnityEvent<float> { }

        [Serializable]
        private class IntegerEvent : UnityEvent<int> { }

        #endregion

        protected override void Start()
        {
            base.Start();

            color = Color.clear;

            UpdateFiller(false);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (Application.isPlaying || fillerImage != null)
            {
                fillerImage.fillMethod = verticalFill ? Image.FillMethod.Vertical : Image.FillMethod.Horizontal;

                UpdateFiller(Application.isPlaying);
            }
        }
#endif

        public void SetValue(float value, bool sendUpdate)
        {
            this.value = value;

            UpdateFiller(sendUpdate);
        }

        private void UpdateFiller(bool sendUpdate)
        {
            if (isIntegerSlider)
            {
                fillerImage.fillAmount = Mathf.Round(value * maxIntegerValue) / maxIntegerValue;
            }
            else
            {
                fillerImage.fillAmount = value;
            }

            if (Application.isPlaying && sendUpdate)
            {
                if (isIntegerSlider)
                {
                    onIntValueSet?.Invoke(Mathf.RoundToInt(value * maxIntegerValue));
                }
                else
                {
                    onValueSet?.Invoke(value);
                }
            }
        }

        private void MoveFiller(Vector2 dragPosition)
        {
            Vector2 point = Vector2.zero;

            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                point = fillerContainer.InverseTransformPoint(dragPosition);
            }
            else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, dragPosition, canvas.worldCamera, out point);
            }

            float valueBorder     = verticalFill ? fillerContainer.rect.height : fillerContainer.rect.width;
            float halfValueBorder = valueBorder / 2f;

            float clampedValue = Mathf.Clamp(verticalFill ? point.y : point.x, -halfValueBorder, halfValueBorder);
            value = (clampedValue + halfValueBorder) / valueBorder;

            UpdateFiller(true);
        }

        #region IPointerDownHandler

        public void OnPointerDown(PointerEventData eventData)
        {
            MoveFiller(eventData.position);
        }

        #endregion

        #region IDragHandler

        public void OnDrag(PointerEventData eventData)
        {
            MoveFiller(eventData.position);
        }

        #endregion

    }

}
