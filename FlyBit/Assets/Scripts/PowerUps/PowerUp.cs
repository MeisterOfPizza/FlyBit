using System;
using UnityEngine;

#pragma warning disable 0649

namespace FlyBit.PowerUps
{

    abstract class PowerUp : MonoBehaviour
    {

        #region Editor

        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        #endregion

        #region Private variables

        private Action<PowerUp> onActivatePoolCall;
        private bool            isActivated;

        #endregion

        public void Initialize(Action<PowerUp> onActivatePoolCall)
        {
            this.onActivatePoolCall = onActivatePoolCall;
        }

        private void OnEnable()
        {
            isActivated = false;
        }

        protected abstract void Activate();
        
        protected virtual void Pool()
        {
            onActivatePoolCall.Invoke(this);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isActivated)
            {
                isActivated = true;

                Activate();
                Pool();
            }
        }

        public void SetColor(Color color)
        {
            spriteRenderer.color = color;
        }

    }

}
