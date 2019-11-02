using UnityEngine;

namespace FlyBit.Controllers
{

    class StatController : Controller<StatController>
    {

        #region Private constants

        private const string VALUE_KEY_SCORE_HIGH_SCORE             = "score-highscore";
        private const string VALUE_KEY_TIME_ALIVE_HIGH_SCORE        = "time-alive-highscore";
        private const string VALUE_KEY_DISTANCE_TRAVELED_HIGH_SCORE = "distance-traveled-highscore";

        #endregion

        public int GetSetScoreHighScore(int newScore)
        {
            int highscore = PlayerPrefs.GetInt(VALUE_KEY_SCORE_HIGH_SCORE, 0);

            if (newScore > highscore)
            {
                PlayerPrefs.SetInt(VALUE_KEY_SCORE_HIGH_SCORE, newScore);
            }

            return highscore;
        }

        public float GetSetTimeAliveHighScore(float newTimeAlive)
        {
            float highscore = PlayerPrefs.GetFloat(VALUE_KEY_TIME_ALIVE_HIGH_SCORE, 0);

            if (newTimeAlive > highscore)
            {
                PlayerPrefs.SetFloat(VALUE_KEY_TIME_ALIVE_HIGH_SCORE, newTimeAlive);
            }

            return highscore;
        }

        public float GetSetDistanceTraveledHighScore(float newDistanceTraveled)
        {
            float highscore = PlayerPrefs.GetFloat(VALUE_KEY_DISTANCE_TRAVELED_HIGH_SCORE, 0);

            if (newDistanceTraveled > highscore)
            {
                PlayerPrefs.SetFloat(VALUE_KEY_DISTANCE_TRAVELED_HIGH_SCORE, newDistanceTraveled);
            }

            return highscore;
        }

    }

}
