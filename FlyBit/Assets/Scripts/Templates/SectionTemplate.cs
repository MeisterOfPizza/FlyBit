using System;
using UnityEngine;

#pragma warning disable 0649

namespace FlyBit.Templates
{

    [CreateAssetMenu(menuName = "Templates/Section")]
    sealed class SectionTemplate : ScriptableObject
    {

        #region Editor

        [Header("References")]
        [SerializeField] private GameObject wallColumnPrefab;

        [Header("Values")]
        [SerializeField, Range(0f, 1f)] private float minNormalizedDifficulty = 0.1f;
        [SerializeField, Range(0f, 1f)] private float spawnChance             = 0.2f;
        [SerializeField, Range(5, 100)] private int   minColumnCount          = 5;
        [SerializeField, Range(5, 100)] private int   maxColumnCount          = 10;
        [SerializeField, Range(0f, 1f)] private float maxScorePointFrequency  = 0.1f;
        [SerializeField, Range(1, 100)] private int   maxPowerUpCount         = 2;

        [Space]
        [SerializeField] private float minWallColumnSpaceHeight = 2.5f;
        [SerializeField] private float maxWallColumnSpaceHeight = 5f;
        [SerializeField] private float wallFormationScale       = 1f;

        [Space]
        [SerializeField] private SectionPowerUp[] sectionPowerUps;

        [Space]
        [SerializeField] private SectionWallFormation possibleFormations = SectionWallFormation.Line;

        #endregion

        #region Classes

        [Serializable]
        public class SectionPowerUp
        {

            [SerializeField, Range(0f, 1f)] private float      spawnChance = 0.5f;
            [SerializeField]                private GameObject prefab;

            #region Public properties

            public float SpawnChance
            {
                get
                {
                    return spawnChance;
                }
            }

            public GameObject Prefab
            {
                get
                {
                    return prefab;
                }
            }

            #endregion

            #region Helper methods

            public Tuple<GameObject, float> GetPrefabProbabilityPair()
            {
                return new Tuple<GameObject, float>(prefab, spawnChance);
            }

            #endregion

        }

        #endregion

        #region Enums

        [Flags]
        public enum SectionWallFormation : byte
        {
            Line   = 1,
            Wave   = 2,
            Circle = 4,
            Climb  = 8,
            Drop   = 16,
            Box    = 32,
            Curve  = 64
        }

        #endregion

        #region Public properties

        public GameObject WallColumnPrefab
        {
            get
            {
                return wallColumnPrefab;
            }
        }

        public float MinNormalizedDifficulty
        {
            get
            {
                return minNormalizedDifficulty;
            }
        }

        public float SpawnChance
        {
            get
            {
                return spawnChance;
            }
        }

        public int MinColumnCount
        {
            get
            {
                return minColumnCount;
            }
        }

        public int MaxColumnCount
        {
            get
            {
                return maxColumnCount;
            }
        }

        public float MaxScorePointFrequency
        {
            get
            {
                return maxScorePointFrequency;
            }
        }

        public int MaxPowerUpCount
        {
            get
            {
                return maxPowerUpCount;
            }
        }

        public float MinWallColumnSpaceHeight
        {
            get
            {
                return minWallColumnSpaceHeight;
            }
        }

        public float MaxWallColumnSpaceHeight
        {
            get
            {
                return maxWallColumnSpaceHeight;
            }
        }

        public float WallFormationScale
        {
            get
            {
                return wallFormationScale;
            }
        }

        public SectionPowerUp[] SectionPowerUps
        {
            get
            {
                return sectionPowerUps;
            }
        }

        public SectionWallFormation PossibleFormations
        {
            get
            {
                return possibleFormations;
            }
        }

        #endregion

        #region Helper methods

        public Tuple<GameObject, float>[] GetPrefabProbabilityPairs()
        {
            var pairs = new Tuple<GameObject, float>[sectionPowerUps.Length];

            for (int i = 0; i < pairs.Length; i++)
            {
                pairs[i] = sectionPowerUps[i].GetPrefabProbabilityPair();
            }

            return pairs;
        }

        #endregion

    }

}
