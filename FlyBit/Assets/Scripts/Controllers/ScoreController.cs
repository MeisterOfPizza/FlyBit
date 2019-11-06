using FlyBit.Extensions;
using FlyBit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private Transform     uiGainedScoreContainer;
        [SerializeField] private GameObject    uiGainedScorePrefab;
        [SerializeField] private UIColorInvert uiGainedScoreColorInvert;

        [Space]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text timeAliveText;
        [SerializeField] private TMP_Text distanceTraveledText;

        [Header("Death Screen References")]
        [SerializeField] private TMP_Text     scoreFinalText;
        [SerializeField] private TMP_Text     timeAliveFinalText;
        [SerializeField] private TMP_Text     distanceTraveledFinalText;
        [SerializeField] private StatRecord[] statRecords;

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

        private GameObjectPool<UIGainedScore> uiGainedScorePool;

        private int   score;
        private float timeAlive;
        private float distanceTraveled;

        private bool timeAliveCounterIsPaused = true;

        private Dictionary<StatRecordType, float> statRecordTypes = new Dictionary<StatRecordType, float>()
        {
            { StatRecordType.ScorePointsTaken,           0f },
            { StatRecordType.ScorePointsScoreGained,     0f },
            { StatRecordType.InvertPowerUpsTaken,        0f },
            { StatRecordType.InvertPowerUpsScoreGained,  0f },
            { StatRecordType.InfiniteFuelPowerUpsTaken,  0f },
            { StatRecordType.InfiniteFuelDuration,       0f },
            { StatRecordType.ExtraLifePowerUpsTaken,     0f },
            { StatRecordType.ExtraLifePowerUpsMissed,    0f },
            { StatRecordType.DubblePointsPowerUpsTaken,  0f },
            { StatRecordType.DubblePointsScoreGained,    0f },
            { StatRecordType.HyperdrivePowerUpsTaken,    0f },
            { StatRecordType.HyperdriveDistanceTraveled, 0f }
        };

        #endregion

        #region Enums

        public enum StatRecordType
        {
            ScorePointsTaken,
            ScorePointsScoreGained,
            InvertPowerUpsTaken,
            InvertPowerUpsScoreGained,
            InfiniteFuelPowerUpsTaken,
            InfiniteFuelDuration,
            ExtraLifePowerUpsTaken,
            ExtraLifePowerUpsMissed,
            DubblePointsPowerUpsTaken,
            DubblePointsScoreGained,
            HyperdrivePowerUpsTaken,
            HyperdriveDistanceTraveled
        }

        #endregion

        #region Classes

        [Serializable]
        private class StatRecord
        {

            [SerializeField] private StatRecordType statRecordType;
            [SerializeField] private TMP_Text       statRecordText;
            [SerializeField] private string         postfix = "taken";

            public StatRecordType StatRecordType
            {
                get
                {
                    return statRecordType;
                }
            }

            public TMP_Text StatRecordText
            {
                get
                {
                    return statRecordText;
                }
            }

            public string Postfix
            {
                get
                {
                    return postfix;
                }
            }

        }

        #endregion

        #region Life cycle

        public override void OnAwake()
        {
            uiGainedScorePool = new GameObjectPool<UIGainedScore>(uiGainedScoreContainer, uiGainedScorePrefab, 25);

            foreach (var uiGainedScore in uiGainedScorePool.PooledItemsNonAloc)
            {
                uiGainedScore.Initialize(uiGainedScorePool);
            }

            uiGainedScoreColorInvert.AddColorOptions(Color.white, uiGainedScorePool.PooledItemsNonAloc.Select(e => e.TextGraphic).ToArray());
        }

        public void Begin()
        {
            score            = 0;
            timeAlive        = 0f;
            distanceTraveled = 0f;

            scoreText.text            = "SCORE: 0";
            timeAliveText.text        = "TIME ALIVE: 0 SEC";
            distanceTraveledText.text = "0 M";

            timeAliveCounterIsPaused = false;

            uiGainedScorePool.PoolAllItems();

            foreach (var key in statRecordTypes.Keys.ToList())
            {
                statRecordTypes[key] = 0f;
            }
        }

        public void End()
        {
            timeAliveCounterIsPaused = true;

            UpdateDeathScreenStats();
        }

        private void Update()
        {
            if (GameController.Singleton.IsMatchRunning && !PlayerController.Singleton.IsDead)
            {
                if (!timeAliveCounterIsPaused)
                {
                    timeAlive += Time.deltaTime;

                    timeAliveText.text = "TIME ALIVE: " + MathE.FormatTimeAlive(timeAlive);
                }
            }
        }

        #endregion

        #region Helpers

        public void PauseTimeAlive(bool pause)
        {
            timeAliveCounterIsPaused = pause;
        }

        public void IncreaseScore(int scoreToAdd)
        {
            score += scoreToAdd;

            scoreText.text = "SCORE: " + score;

            var uiGainedScore = uiGainedScorePool.GetItem();

            if (uiGainedScore != null)
            {
                uiGainedScore.Spawn(scoreToAdd);
            }
        }

        public void AddDistanceTraveled(float distance)
        {
            distanceTraveled         += distance;
            distanceTraveledText.text = distanceTraveled.ToString("F0") + " M";
        }

        public void AddStatRecordValue(StatRecordType statRecordType, float value)
        {
            statRecordTypes[statRecordType] += value;
        }

        private void UpdateDeathScreenStats()
        {
            int   scoreHighScore            = StatController.Singleton.GetSetScoreHighScore(score);
            float timeAliveHighScore        = StatController.Singleton.GetSetTimeAliveHighScore(timeAlive);
            float distanceTraveledHighScore = StatController.Singleton.GetSetDistanceTraveledHighScore(distanceTraveled);

            // TODO: Get high score from db or local file.
            scoreFinalText.text            = string.Format("score: {0} ({1})", score, score > scoreHighScore ? "RECORD!" : "record is " + scoreHighScore);
            timeAliveFinalText.text        = string.Format("time alive: {0} ({1})", MathE.FormatTimeAlive(timeAlive).ToLower(), timeAlive > timeAliveHighScore ? "RECORD!" : "record is " + MathE.FormatTimeAlive(timeAliveHighScore).ToLower());
            distanceTraveledFinalText.text = string.Format("distance traveled: {0} m ({1})", distanceTraveled.ToString("F0"), distanceTraveled > distanceTraveledHighScore ? "RECORD!" : "record is " + distanceTraveledHighScore.ToString("F0") + " m");

            foreach (var statRecord in statRecords)
            {
                statRecord.StatRecordText.text = statRecordTypes[statRecord.StatRecordType].ToString("F0") + " " + statRecord.Postfix;
            }
        }

        #endregion

    }

}
