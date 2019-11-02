using FlyBit.Controllers;
using UnityEngine;

namespace FlyBit.PowerUps
{

    sealed class InvertPowerUp : PowerUp
    {

        #region Editor

        [Header("Values")]
        [SerializeField] private int scoreBaseReward       = 100;
        [SerializeField] private int scoreDifficultyReward = 150;

        #endregion

        protected override void Activate()
        {
            EffectsController.Singleton.ToggleInvertEffect();

            // Calculate the score and round it to nearest 10.
            int score = scoreBaseReward + Mathf.FloorToInt(scoreDifficultyReward * DifficultyController.Singleton.NormalizedDifficulty / 10) * 10;

            ScoreController.Singleton.IncreaseScore(score);

            ScoreController.Singleton.AddStatRecordValue(ScoreController.StatRecordType.InvertPowerUpsTaken, 1);
            ScoreController.Singleton.AddStatRecordValue(ScoreController.StatRecordType.InvertPowerUpsScoreGained, score);
        }

    }

}
