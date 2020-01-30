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
        [SerializeField] private PostProcessLayer mainCameraPostProcessLayer;

        [Space]
        [SerializeField] private AudioSource musicAudioSource;

        #endregion

        #region Public properties

        public bool IsMatchRunning { get; private set; } = false;

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
#else
            mainCameraPostProcessLayer.enabled = true;
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
