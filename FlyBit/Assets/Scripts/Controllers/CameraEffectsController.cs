using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

#pragma warning disable 0649

namespace FlyBit.Controllers
{

    class CameraEffectsController : Controller<CameraEffectsController>
    {

        #region Editor

        [Header("References")]
        [SerializeField] private Camera mainCamera;

        [Space]
        [SerializeField] private PostProcessProfile postProcessProfile;

        #endregion

        #region Effects

        public void PlayShakeCamera(float duration, float magnitude)
        {
            IEnumerator coroutine = ShakeCamera(duration, magnitude);

            StartCoroutine(coroutine);
        }

        public void PlayHyperdriveZoom(float duration, float magnitude)
        {
            IEnumerator coroutine = HyperdriveZoom(duration, magnitude);

            StartCoroutine(coroutine);
        }

        #endregion

        #region Effect enumerators

        private IEnumerator ShakeCamera(float duration, float magnitude)
        {
            Vector3 originalPos = mainCamera.transform.localPosition;
            float   elapsed     = 0f;

            while (elapsed < duration)
            {
                float f = Mathf.Sin(elapsed * Mathf.PI / duration);
                float x = Random.Range(-magnitude, magnitude) * f;
                float y = Random.Range(-magnitude, magnitude) * f;

                mainCamera.transform.localPosition = new Vector3(x, y, originalPos.z);

                elapsed += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            mainCamera.transform.localPosition = originalPos;
        }

        private IEnumerator HyperdriveZoom(float duration, float magnitude)
        {
            Vector3 originalPos  = mainCamera.transform.localPosition;
            float   elapsed      = 0f;
            float   originalSize = mainCamera.orthographicSize;

            while (elapsed < duration)
            {
                float z    = originalPos.z + Mathf.Sin(elapsed * Mathf.PI / duration) * magnitude;
                float size = originalSize + Mathf.Sin(elapsed * Mathf.PI / duration) * magnitude;

                mainCamera.transform.localPosition = new Vector3(originalPos.x, originalPos.y, z);
                mainCamera.orthographicSize        = size;

                elapsed += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            mainCamera.transform.localPosition = originalPos;
            mainCamera.orthographicSize        = originalSize;
        }

        #endregion

    }

}
