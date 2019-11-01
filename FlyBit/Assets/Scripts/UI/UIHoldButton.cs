using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#pragma warning disable 0649

namespace FlyBit.UI
{

    sealed class UIHoldButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {

        #region Editor

        [Header("References")]
        [SerializeField] private Image targetImage;

        [Header("Values")]
        [SerializeField] private float holdTimeComplete = 1f;

        [Space]
        [SerializeField] private float fillAmountSpeed = 3f;

        [Space]
        [SerializeField] private UnityEvent onHoldComplete;

        #endregion

        #region Private variables

        private bool  isHolding;
        private float holdTime;

        #endregion

        private void Update()
        {
            if (isHolding)
            {
                holdTime              += Time.deltaTime;
                targetImage.fillAmount = Mathf.Lerp(targetImage.fillAmount, holdTime / holdTimeComplete, fillAmountSpeed * Time.deltaTime);

                if (holdTime >= holdTimeComplete)
                {
                    onHoldComplete.Invoke();

                    isHolding = false;
                    holdTime  = 0f;
                }
            }
            else
            {
                targetImage.fillAmount = Mathf.Lerp(targetImage.fillAmount, 0f, fillAmountSpeed * Time.deltaTime);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isHolding = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isHolding = false;
            holdTime  = 0f;
        }

        // Required for OnPointerDown and OnPointerUp.
        public void OnPointerClick(PointerEventData eventData) { }

    }

}
