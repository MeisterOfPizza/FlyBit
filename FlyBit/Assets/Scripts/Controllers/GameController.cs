using UnityEngine;

#pragma warning disable 0649

namespace FlyBit.Controllers
{

    class GameController : Controller<GameController>
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
        }

        public void StartMatch()
        {
            IsMatchRunning = true;

            menuScreen.SetActive(false);
            gameScreen.SetActive(true);
            deathScreen.SetActive(false);

            EffectsController.Singleton.ResetAllEffects();
            PlayerController.Singleton.ResetPlayer();
            MapController.Singleton.Begin();
            ScoreController.Singleton.Begin();
            DifficultyController.Singleton.Begin();
        }

        public void EndMatch()
        {
            IsMatchRunning = false;

            gameScreen.SetActive(false);
            deathScreen.SetActive(true);

            MapController.Singleton.End();
            ScoreController.Singleton.End();
            DifficultyController.Singleton.End();
        }

    }

}
