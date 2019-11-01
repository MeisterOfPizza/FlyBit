using UnityEngine;

namespace FlyBit.Controllers
{

    class DifficultyController : Controller<DifficultyController>
    {

        #region Editor

        [Header("Values")]
        [SerializeField] private float maxDifficultyTime = 300f;

        #endregion

        #region Public properties

        public float NormalizedDifficulty
        {
            get
            {
                return normalizedDifficulty;
            }
        }

        #endregion

        #region Private variables

        private float time                 = 0f;
        private float normalizedDifficulty = 0f;

        private bool isPaused;

        #endregion

        private void Update()
        {
            if (GameController.Singleton.IsMatchRunning && !isPaused)
            {
                time += Time.deltaTime;

                normalizedDifficulty = Mathf.Clamp01(time / maxDifficultyTime);
            }
        }

        public void Begin()
        {
            time                 = 0f;
            normalizedDifficulty = 0f;

            isPaused = false;
        }

        public void End()
        {
            isPaused = true;
        }

        public void Pause(bool pause)
        {
            isPaused = pause;
        }

    }

}
