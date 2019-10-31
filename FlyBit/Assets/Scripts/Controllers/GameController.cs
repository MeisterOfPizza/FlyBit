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

        #endregion

        #region Public properties

        public bool IsMatchRunning { get; private set; } = false;

        #endregion

        private void Start()
        {
            Screen.fullScreen = false;
        }

        public void StartMatch()
        {
            IsMatchRunning = true;

            menuScreen.SetActive(false);
            gameScreen.SetActive(true);

            PlayerController.Singleton.ResetPlayer();
            MapController.Singleton.Begin();
            ScoreController.Singleton.Begin();
        }

        public void EndMatch()
        {
            IsMatchRunning = false;

            MapController.Singleton.End();
            ScoreController.Singleton.End();
        }

    }

}
