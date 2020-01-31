using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace FlyBit.Controllers
{

    class PauseController : Controller<PauseController>
    {

        #region Editor

        [Header("References")]
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private Animator   animator;

        [Header("UI References")]
        [SerializeField] private Image blurBackground;

        [Header("Values")]
        [SerializeField] private float blurTime     = 0.5f;
        [SerializeField] private float blurStrength = 2f;
        
        #endregion

        #region Private variables

        private bool pauseMenuIsOpen;

        #endregion

        private void Update()
        {
            if (GameController.Singleton.IsMatchRunning
                && Input.GetKeyDown(KeyCode.Escape)
                && !pauseMenuIsOpen
                && !PlayerEffectsController.Singleton.HasPlayerEffect(PlayerEffect.Hyperdrive)
                && !PlayerEffectsController.Singleton.HasPlayerEffect(PlayerEffect.HyperdriveController))
            {
                OpenPauseMenu();
            }
        }

        public void OpenPauseMenu()
        {
            pauseMenuIsOpen = true;

            GameController.Singleton.PauseMatch(true);

            pauseMenu.SetActive(true);
            animator.Play("Pause");

            StartCoroutine("Blur", true);
        }

        public void ClosePauseMenu()
        {
            animator.Play("Play");

            StartCoroutine("Blur", false);
        }

        public void FinishClosePauseMenu()
        {
            pauseMenuIsOpen = false;

            pauseMenu.SetActive(false);
            GameController.Singleton.PauseMatch(false);
        }

        public void OpenMainMenu()
        {
            pauseMenuIsOpen = false;

            mainMenu.SetActive(true);
            pauseMenu.SetActive(false);

            GameController.Singleton.AbortMatch();
        }

        #region Blurring

        private IEnumerator Blur(bool blurIn)
        {
            float time = blurTime;

            while (time > 0f)
            {
                blurBackground.material.SetFloat("_Size", (blurIn ? blurTime - time : time) / blurTime * blurStrength);

                time -= Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }
        }

        #endregion

    }

}
