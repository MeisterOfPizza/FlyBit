using FlyBit.Controllers;
using FlyBit.Extensions;
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

        private GameObjectPool<ScorePoint> pool;

        private bool canBeTaken;

        #endregion

        public void Initialize(GameObjectPool<ScorePoint> pool)
        {
            this.pool = pool;
        }

        public void Despawn()
        {
            pool.PoolItem(this);
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

                if (PlayerController.Singleton.HasDubblePoints)
                {
                    ScoreController.Singleton.IncreaseScore(20);
                    ScoreController.Singleton.AddStatRecordValue(ScoreController.StatRecordType.DubblePointsScoreGained, 10);
                }
                else
                {
                    ScoreController.Singleton.IncreaseScore(10);
                }

                ScoreController.Singleton.AddStatRecordValue(ScoreController.StatRecordType.ScorePointsTaken, 1);
                ScoreController.Singleton.AddStatRecordValue(ScoreController.StatRecordType.ScorePointsScoreGained, 10);

                pool.PoolItem(this);
            }
        }

        public void SetColor(Color color)
        {
            spriteRenderer.color = color;
        }

    }

}
