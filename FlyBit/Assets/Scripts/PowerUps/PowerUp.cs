using FlyBit.Controllers;
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

        [Space]
        [SerializeField] private AudioClip[] onActivatedAudioClips;

        #endregion

        #region Protected and private variables

        protected bool isActivated;

        private Action<PowerUp> onActivatePoolCall;

        #endregion

        public void Initialize(Action<PowerUp> onActivatePoolCall)
        {
            this.onActivatePoolCall = onActivatePoolCall;
        }

        private void OnEnable()
        {
            isActivated = false;
            spriteRenderer.enabled = true;
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

                PlayerController.Singleton.PlayAudioClip(onActivatedAudioClips[UnityEngine.Random.Range(0, onActivatedAudioClips.Length)], SettingsController.Singleton.EffectsVolume);

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
