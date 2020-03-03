using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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

        [Space]
        [SerializeField] private GameObject exitButton;

        [Space]
        [SerializeField] private TMP_Text mainMenuHelpText;

        [Space]
        [SerializeField] private PostProcessLayer mainCameraPostProcessLayer;

        [Space]
        [SerializeField] private AudioSource musicAudioSource;

        #endregion

        #region Public properties

        public bool IsMatchRunning { get; private set; } = false;
        public bool IsMatchPaused  { get; private set; } = false;

        #endregion

        public override void OnAwake()
        {
            base.OnAwake();

            // TODO: Set fullscreen for demos.
            Screen.fullScreen = true;

#if UNITY_ANDROID || UNITY_IOS
            //Application.targetFrameRate = 60;
            Application.targetFrameRate = 30;

            mainCameraPostProcessLayer.enabled = false;
            mainMenuHelpText.text = "hold to go up\nremember to keep an eye on your fuel";
#else
            mainCameraPostProcessLayer.enabled = true;

            mainMenuHelpText.text = "hold the left mouse button or space to go up\nremember to keep an eye on your fuel";
#endif

#if !UNITY_STANDALONE
            exitButton.SetActive(false);
#endif
        }

        private void Start()
        {
            // Music
            musicAudioSource.volume = SettingsController.Singleton.MusicVolume;
            SettingsController.Singleton.OnMusicVolumeChanged += (float value) => { musicAudioSource.volume = value; };
        }

        public void StartMatch()
        {
            if (!PlayerController.Singleton.IsSpawning && !PlayerController.Singleton.IsReviving)
            {
                IsMatchRunning = true;
                IsMatchPaused  = false;

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
            if (IsMatchRunning)
            {
                IsMatchRunning = false;
                IsMatchPaused = false;

                gameScreen.SetActive(false);
                deathScreen.SetActive(true);

                DifficultyController.Singleton.End();
                ScoreController.Singleton.End(true);
                MapController.Singleton.End();
            }
        }

        public void PauseMatch(bool pause)
        {
            IsMatchPaused = pause;
        }

        public void AbortMatch()
        {
            IsMatchRunning = false;
            IsMatchPaused  = false;

            gameScreen.SetActive(false);

            DifficultyController.Singleton.End();
            ScoreController.Singleton.End(false);
            MapController.Singleton.End();

            PlayerController.Singleton.DieWithoutEndingMatch();
        }

        public void ExitGame()
        {
            if (!Application.isEditor)
            {
                Application.Quit();
            }
            else
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
        }

    }

}
