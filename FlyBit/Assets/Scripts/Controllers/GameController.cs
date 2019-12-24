using TMPro;
using UnityEngine;

#pragma warning disable 0649

namespace FlyBit.Controllers
{

    public class GameController : Controller<GameController>
    {

        #region Editor

        [Header("References")]
        [SerializeField] private GameObject menuScreen;
        [SerializeField] private GameObject gameScreen;
        [SerializeField] private GameObject deathScreen;

        #endregion

        #region Public properties

        public bool IsMatchRunning { get; private set; } = false;

        #endregion

        private void Start()
        {
            // TODO: Set fullscreen for demos.
            Screen.fullScreen = true;

#if UNITY_ANDROID || UNITY_IOS
            Application.targetFrameRate = 60;
#endif
        }

        public void StartMatch()
        {
            if (!PlayerController.Singleton.IsSpawning && !PlayerController.Singleton.IsReviving)
            {
                IsMatchRunning = true;

                menuScreen.SetActive(false);
                gameScreen.SetActive(true);
                deathScreen.SetActive(false);

                EffectsController.Singleton.ResetAllEffects();
                DifficultyController.Singleton.Begin();
                ScoreController.Singleton.Begin();
                PlayerController.Singleton.ResetPlayer();
                MapController.Singleton.Begin();
            }
        }

        public void EndMatch()
        {
            IsMatchRunning = false;

            gameScreen.SetActive(false);
            deathScreen.SetActive(true);

            DifficultyController.Singleton.End();
            ScoreController.Singleton.End();
            MapController.Singleton.End();
        }

    }

}
