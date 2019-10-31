using FlyBit.Controllers;
using System;
using UnityEngine;

#pragma warning disable 0649

namespace FlyBit.Map
{

    sealed class ScorePoint : MonoBehaviour
    {

        #region Editor

        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        #endregion

        #region Private variables

        private Action<ScorePoint> poolCall;

        private bool canBeTaken;

        #endregion

        public void Initialize(Action<ScorePoint> poolCall)
        {
            this.poolCall = poolCall;
        }

        private void OnEnable()
        {
            canBeTaken = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (canBeTaken)
            {
                canBeTaken = false;

                ScoreController.Singleton.IncreaseScore();

                poolCall.Invoke(this);
            }
        }

        public void SetColor(Color color)
        {
            spriteRenderer.color = color;
        }

    }

}
