using UnityEngine;

namespace FlyBit.Controllers
{

    class SettingsController : Controller<SettingsController>
    {

        #region Private constants

        private const string MUSIC_VOLUME_KEY   = "MusicVolume";
        private const string EFFECTS_VOLUME_KEY = "EffectsVolume";
        private const string MISC_VOLUME_KEY    = "MiscVolume";

        #endregion

        #region Editor

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
                return currentMusicVolume;
            }
        }

        public float EffectsVolume
        {
            get
            {
                return currentEffectsVolume;
            }
        }

        public float MiscVolume
        {
            get
            {
                return currentMiscVolume;
            }
        }

        #endregion

        #region Private variables

        private float currentMusicVolume;
        private float currentEffectsVolume;
        private float currentMiscVolume;

        #endregion

        public override void OnAwake()
        {
            currentMusicVolume   = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, defaultMusicVolume);
            currentEffectsVolume = PlayerPrefs.GetFloat(EFFECTS_VOLUME_KEY, defaultEffectsVolume);
            currentMiscVolume    = PlayerPrefs.GetFloat(MISC_VOLUME_KEY, defaultMiscVolume);
        }

    }

}
