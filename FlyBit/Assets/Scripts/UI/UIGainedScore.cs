using FlyBit.Extensions;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace FlyBit.UI
{

    sealed class UIGainedScore : MonoBehaviour
    {

        #region Editor

        [Header("References")]
        [SerializeField] private TMP_Text scoreText;

        [Header("Values")]
        [SerializeField] private float speed    = 3f;
        [SerializeField] private float fadeTime = 1.5f;

        #endregion

        #region Public properties

        public Graphic TextGraphic
        {
            get
            {
                return scoreText;
            }
        }

        #endregion

        #region Private variables

        private GameObjectPool<UIGainedScore> pool;
        private Vector2                       direction;

        #endregion

        public void Initialize(GameObjectPool<UIGainedScore> pool)
        {
            this.pool = pool;
        }

        public void Spawn(int score)
        {
            transform.localPosition = Vector3.zero;
            scoreText.text          = "+" + score;
            scoreText.color         = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, 1f);

            direction = new Vector2(Random.Range(0.5f, 1f), Random.Range(-1f, 1f));

            StartCoroutine("PlayEffect");
        }

        private void OnDisable()
        {
            StopCoroutine("PlayEffect");
        }

        private IEnumerator PlayEffect()
        {
            float duration = fadeTime;

            while (duration > 0f)
            {
                duration -= Time.deltaTime;
                transform.localPosition += (Vector3)(direction * speed * Time.deltaTime);

                scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, duration / fadeTime);

                yield return new WaitForEndOfFrame();
            }

            pool.PoolItem(this);
        }

    }

}
