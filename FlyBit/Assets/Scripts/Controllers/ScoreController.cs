using TMPro;
using UnityEngine;

#pragma warning disable 0649

namespace FlyBit.Controllers
{

    class ScoreController : Controller<ScoreController>
    {

        #region Editor

        [Header("References")]
        [SerializeField] private Camera mainCamera;

        [Space]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text timeAliveText;
        [SerializeField] private TMP_Text distanceTraveledText;

        #endregion

        #region Public properties

        public float TimeAlive
        {
            get
            {
                return timeAlive;
            }
        }

        #endregion

        #region Private variables

        private int   score;
        private float timeAlive;
        private float distanceTraveled;

        private bool timeAliveCounterIsPaused = true;

        #endregion

        public void Begin()
        {
            score            = 0;
            timeAlive        = 0f;
            distanceTraveled = 0f;

            timeAliveCounterIsPaused = false;
        }

        public void End()
        {
            timeAliveCounterIsPaused = true;
        }

        private void Update()
        {
            if (GameController.Singleton.IsMatchRunning && !PlayerController.Singleton.IsDead)
            {
                if (!timeAliveCounterIsPaused)
                {
                    timeAlive += Time.deltaTime;

                    string formattedTimeAlive = "TIME ALIVE: ";

                    if (timeAlive > 60f)
                    {
                        if ((int)timeAlive % 60 == 0)
                        {
                            formattedTimeAlive += Mathf.FloorToInt(timeAlive / 60f) + " MIN";
                        }
                        else
                        {
                            formattedTimeAlive += Mathf.FloorToInt(timeAlive / 60f) + " MIN : " + (timeAlive % 60f).ToString("F1") + " SEC";
                        }
                    }
                    else
                    {
                        formattedTimeAlive += timeAlive.ToString("F1") + " SEC";
                    }

                    timeAliveText.text = formattedTimeAlive;
                }

                //print("X: " + mainCamera.ScreenToWorldPoint(distanceTraveledText.transform.position).x);
                //print("Y: " + PlayerController.Singleton.GetWallSection(mainCamera.ScreenToWorldPoint(distanceTraveledText.transform.position))?.transform.position.y);
                distanceTraveledText.transform.position = new Vector2(distanceTraveledText.transform.position.x,
                                                                      mainCamera.WorldToViewportPoint(
                                                                          PlayerController.Singleton.GetWallSection(
                                                                              mainCamera.ScreenToWorldPoint(distanceTraveledText.transform.position)
                                                                              )?.transform.position ?? Vector3.zero
                                                                          ).y
                                                                      );
            }
        }

        public void PauseTimeAlive(bool pause)
        {
            timeAliveCounterIsPaused = pause;
        }

        public void IncreaseScore()
        {
            score += 10;

            scoreText.text = "SCORE: " + score;
        }

        public void AddDistanceTraveled(float distance)
        {
            distanceTraveled         += distance;
            distanceTraveledText.text = distanceTraveled.ToString("F1") + " M";
        }

    }

}
