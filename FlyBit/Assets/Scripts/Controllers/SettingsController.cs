using FlyBit.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace FlyBit.Controllers
{

    class SettingsController : Controller<SettingsController>
    {

        #region Private constants

        private const string MUSIC_VOLUME_KEY   = "MusicVolume";
        private const string EFFECTS_VOLUME_KEY = "EffectsVolume";
        private const string MISC_VOLUME_KEY    = "MiscVolume";
        private const string MUTED_KEY          = "Muted";

        #endregion

        #region Editor

        [Header("References")]
        [SerializeField] private Button   muteButton;
        [SerializeField] private TMP_Text muteButtonText;
        [SerializeField] private UISlider musicVolumeSlider;
        [SerializeField] private UISlider effectsVolumeSlider;
        [SerializeField] private UISlider miscVolumeSlider;

        [Header("Values")]
        [SerializeField, Range(0f, 1f)] private float defaultMusicVolume   = 0.5f;
        [SerializeField, Range(0f, 1f)] private float defaultEffectsVolume = 0.5f;
        [SerializeField, Range(0f, 1f)] private float defaultMiscVolume    = 0.5f;

        #endregion

        #region Public properties

        public float MusicVolume
        {
            get
            {
                return isMuted ? 0f : currentMusicVolume;
            }
        }

        public float EffectsVolume
        {
            get
            {
                return isMuted ? 0f : currentEffectsVolume;
            }
        }

        public float MiscVolume
        {
            get
            {
                return isMuted ? 0f : currentMiscVolume;
            }
        }

        public Action<float> OnMusicVolumeChanged   { get; set; }
        public Action<float> OnEffectsVolumeChanged { get; set; }
        public Action<float> OnMiscVolumeChanged    { get; set; }

        #endregion

        #region Private variables

        private bool hasInitialized;

        private float currentMusicVolume;
        private float currentEffectsVolume;
        private float currentMiscVolume;

        private bool isMuted;

        #endregion

        public override void OnAwake()
        {
            currentMusicVolume   = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, defaultMusicVolume);
            currentEffectsVolume = PlayerPrefs.GetFloat(EFFECTS_VOLUME_KEY, defaultEffectsVolume);
            currentMiscVolume    = PlayerPrefs.GetFloat(MISC_VOLUME_KEY, defaultMiscVolume);
            isMuted              = PlayerPrefs.GetInt(MUTED_KEY, 0) != 0;
        }

        private void Start()
        {
            OnMusicVolumeChanged?.Invoke(MusicVolume);
            OnEffectsVolumeChanged?.Invoke(EffectsVolume);
            OnMiscVolumeChanged?.Invoke(MiscVolume);

            UpdateUI();

            hasInitialized = true;
        }

        public void SetMusicVolume(float value)
        {
            if (hasInitialized)
            {
                currentMusicVolume = value;

                OnMusicVolumeChanged?.Invoke(value);

                SaveSettings();
            }
        }

        public void SetEffectsVolume(float value)
        {
            if (hasInitialized)
            {
                currentEffectsVolume = value;

                OnEffectsVolumeChanged?.Invoke(value);

                SaveSettings();
            }
        }

        public void SetMiscVolume(float value)
        {
            if (hasInitialized)
            {
                currentMiscVolume = value;

                OnMiscVolumeChanged?.Invoke(value);

                SaveSettings();
            }
        }

        public void ToggleMuted()
        {
            if (hasInitialized)
            {
                isMuted = !isMuted;

                muteButtonText.text = isMuted ? "Unmute" : "Mute";

                OnMusicVolumeChanged?.Invoke(MusicVolume);
                OnEffectsVolumeChanged?.Invoke(EffectsVolume);
                OnMiscVolumeChanged?.Invoke(MiscVolume);

                SaveSettings();
            }
        }

        private void UpdateUI()
        {
            muteButtonText.text = isMuted ? "Unmute" : "Mute";

            musicVolumeSlider.SetValue(currentMusicVolume, true);
            effectsVolumeSlider.SetValue(currentEffectsVolume, true);
            miscVolumeSlider.SetValue(currentMiscVolume, true);
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, currentMusicVolume);
            PlayerPrefs.SetFloat(EFFECTS_VOLUME_KEY, currentEffectsVolume);
            PlayerPrefs.SetFloat(MISC_VOLUME_KEY, currentMiscVolume);
            PlayerPrefs.SetInt(MUTED_KEY, isMuted ? 1 : 0);
            PlayerPrefs.Save();
        }

    }

}
