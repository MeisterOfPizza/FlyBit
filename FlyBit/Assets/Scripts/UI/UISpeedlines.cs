using FlyBit.Extensions;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace FlyBit.UI
{

    class UISpeedlines : MonoBehaviour
    {

        #region Editor

        [Header("References")]
        [SerializeField] private GameObject speedlinePrefab;

        [Header("Values")]
        [SerializeField] private int   speedlineCount       = 100;
        [SerializeField] private float speedlineInnerRadius = 250f;
        [SerializeField] private float speedlineDeathRadius = 1000f;
        [SerializeField] private float speedlineSpeed       = 2000f;
        [SerializeField] private float minSpeedlineHeight   = 750f;
        [SerializeField] private float maxSpeedlineHeight   = 1000f;
        [SerializeField] private Color speedlineColor       = Color.white;

        [Space]
        [SerializeField] private bool fadeOutOnDeath;

        #endregion

        #region Private variables

        private GameObjectPool<Image> speedlinePool;

        private bool effectIsActive;

        private Color currentSpeedlineColor;

        #endregion

        private void Awake()
        {
            speedlinePool = new GameObjectPool<Image>(transform, speedlinePrefab, speedlineCount);

            currentSpeedlineColor = speedlineColor;
        }

        private void Update()
        {
            if (effectIsActive)
            {
                foreach (var speedline in speedlinePool.ActiveItems)
                {
                    var speedlineRect = speedline.rectTransform;

                    if (speedlineRect.localPosition.magnitude >= speedlineDeathRadius)
                    {
                        speedlineRect.sizeDelta     = new Vector2(speedlineRect.sizeDelta.x, Random.Range(minSpeedlineHeight, maxSpeedlineHeight));
                        speedlineRect.localRotation = Quaternion.Euler(0, 0, Random.value * 360f);
                        speedlineRect.localPosition = -speedlineRect.up * Random.Range(speedlineInnerRadius, speedlineDeathRadius);

                        speedline.color = currentSpeedlineColor;
                    }
                    else
                    {
                        speedlineRect.localPosition = Vector2.MoveTowards(speedlineRect.localPosition, -speedlineRect.up * (speedlineDeathRadius + 1), speedlineSpeed * Time.deltaTime);

                        if (fadeOutOnDeath)
                        {
                            speedline.color = SpeedlineColorByDistance(speedlineRect.localPosition.magnitude);
                        }
                    }
                }
            }
        }

        public void Play()
        {
            effectIsActive = true;

            for (int i = 0; i < speedlineCount; i++)
            {
                var speedline     = speedlinePool.GetItem();
                var speedlineRect = speedline.rectTransform;

                speedlineRect.sizeDelta     = new Vector2(speedlineRect.sizeDelta.x, Random.Range(minSpeedlineHeight, maxSpeedlineHeight));
                speedlineRect.localRotation = Quaternion.Euler(0, 0, Random.value * 360f);
                speedlineRect.localPosition = -speedlineRect.up * Random.Range(speedlineInnerRadius, speedlineDeathRadius);

                speedline.color = currentSpeedlineColor;
            }
        }

        public void Stop()
        {
            effectIsActive = false;

            speedlinePool.PoolAllItems();
        }

        #region Helpers

        private Color SpeedlineColorByDistance(float distance)
        {
            return new Color(currentSpeedlineColor.r, currentSpeedlineColor.g, currentSpeedlineColor.b, 1f - distance / speedlineDeathRadius);
        }

        public void InvertSpeedlineColors(bool invert)
        {
            currentSpeedlineColor = invert ? speedlineColor.Invert() : speedlineColor;
        }

        #endregion

    }

}
