using FlyBit.Extensions;
using FlyBit.Map;
using FlyBit.PowerUps;
using FlyBit.Templates;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable 0649

namespace FlyBit.Controllers
{

    class MapController : Controller<MapController>
    {

        #region Editor

        [Header("References")]
        [SerializeField] private Transform  mapContainer;

        [Space]
        [SerializeField] private Transform playerTransform;

        [Space]
        [SerializeField] private SectionTemplate   defaultSectionTemplate;
        [SerializeField] private SectionTemplate[] sectionTemplates;

        [Header("Prefabs")]
        [SerializeField] private GameObject wallSectionPrefab;
        /*
        [SerializeField] private GameObject   wallPrefab;
        [SerializeField] private GameObject   scorePointPrefab;
        [SerializeField] private GameObject[] powerUpPrefabs;
        */

        [Header("Values")]
        [SerializeField] private float playerSeeRadius = 20f;

        /*
        [Space]
        [SerializeField] private float minMapMiddleSpace = 2.5f;
        [SerializeField] private float maxMapMiddleSpace = 5f;
        [Space]
        [SerializeField]                private int   scorePointCount       = 5;
        [SerializeField, Range(0f, 1f)] private float scorePointSpawnChance = 0.2f;

        [Space]
        [SerializeField]                private int   powerUpCount        = 2;
        [SerializeField, Range(0f, 1f)] private float powerUpsSpawnChance = 0.05f;
        */

        #endregion

        #region Public properties

        public float PlayerSeeRadius
        {
            get
            {
                return playerSeeRadius;
            }
        }

        #endregion

        #region Private variables

        //private GameObjectPool<WallColumn> wallPool;
        //private GameObjectPool<ScorePoint> scorePointPool;
        //private GameObjectPool<PowerUp>[]  powerUpPools;

        private Dictionary<SectionTemplate, GameObjectPool<WallSection>> wallSectionTemplatePairs;
        private GameObjectPool<WallSection>[]                            wallSectionPools;

        private WallSection lastWallSectionSpawned;

        #endregion

        public override void OnAwake()
        {
            /*
            wallPool       = new GameObjectPool<WallColumn>(mapContainer, wallPrefab, 500);
            scorePointPool = new GameObjectPool<ScorePoint>(mapContainer, scorePointPrefab, scorePointCount);

            foreach (var scorePoint in scorePointPool.PooledItemsNonAloc)
            {
                scorePoint.Initialize(scorePointPool.PoolItem);
            }

            powerUpPools = new GameObjectPool<PowerUp>[powerUpPrefabs.Length];
            for (int i = 0; i < powerUpPrefabs.Length; i++)
            {
                powerUpPools[i] = new GameObjectPool<PowerUp>(mapContainer, powerUpPrefabs[i], powerUpCount);

                foreach (var powerUp in powerUpPools[i].PooledItemsNonAloc)
                {
                    powerUp.Initialize(powerUpPools[i].PoolItem);
                }
            }
            */

            // Create a new array with the default section template added in and sort it by spawn chance.
            var templates            = sectionTemplates.Concat(new SectionTemplate[] { defaultSectionTemplate }).OrderBy(st => st.SpawnChance).ToArray();
            wallSectionTemplatePairs = new Dictionary<SectionTemplate, GameObjectPool<WallSection>>(templates.Length);
            wallSectionPools         = new GameObjectPool<WallSection>[templates.Length];

            for (int i = 0; i < templates.Length; i++)
            {
                var pool = new GameObjectPool<WallSection>(mapContainer, wallSectionPrefab, 5);
                wallSectionTemplatePairs.Add(templates[i], pool);
                wallSectionPools[i] = pool;

                foreach (var section in pool.PooledItemsNonAloc)
                {
                    section.Initialize(templates[i]);
                }
            }
        }

        public void Begin()
        {
            ResetAll();
            BuildMap();

            StartCoroutine("SpawnCycle");
        }

        public void End()
        {
            StopAllCoroutines();
        }

        private IEnumerator SpawnCycle()
        {
            while (GameController.Singleton.IsMatchRunning)
            {
                foreach (var pool in wallSectionPools)
                {
                    foreach (var wallSection in pool.ActiveItems)
                    {
                        if (wallSection.CanDespawn(playerTransform.position.x - playerSeeRadius))
                        {
                            wallSection.Despawn();
                            pool.PoolItem(wallSection);

                            WallSection nextWallSection = GetWallSection(Random.value);
                            nextWallSection.Spawn(lastWallSectionSpawned.EndPoint);

                            lastWallSectionSpawned = nextWallSection;
                        }
                    }
                }

                /*
                foreach (var wall in wallPool.ActiveItems)
                {
                    if (!wall.IsMoving && Mathf.FloorToInt(wall.transform.position.x) < Mathf.FloorToInt(playerTransform.position.x - playerSeeRadius / 3f))
                    {
                        wall.OpenCloseSection(false);

                        var newWall = wallPool.GetItem();
                        newWall.Spawn(new Vector2(wallPool.ActiveItemsNonAloc[wallPool.ActiveItemCount - 2].transform.position.x + 1f, 1.5f * Mathf.Sin(ScoreController.Singleton.TimeAlive)), Random.Range(minMapMiddleSpace, maxMapMiddleSpace));

                        if (scorePointPool.HasAvailableItems && Random.value <= scorePointSpawnChance)
                        {
                            var scorePoint = scorePointPool.GetItem();
                            scorePoint.transform.position = newWall.transform.position;
                        }
                        else if (Random.value <= powerUpsSpawnChance)
                        {
                            int powerUpType = Random.Range(0, powerUpPrefabs.Length);

                            if (powerUpPools[powerUpType].HasAvailableItems)
                            {
                                var powerUp = powerUpPools[powerUpType].GetItem();
                                powerUp.transform.position = newWall.transform.position;
                            }
                        }
                    }
                }

                foreach (var scorePoint in scorePointPool.ActiveItems)
                {
                    if (Mathf.FloorToInt(scorePoint.transform.position.x) < Mathf.FloorToInt(playerTransform.position.x - playerSeeRadius))
                    {
                        scorePointPool.PoolItem(scorePoint);
                    }
                }

                foreach (var pool in powerUpPools)
                {
                    foreach (var powerUp in pool.ActiveItems)
                    {
                        if (Mathf.FloorToInt(powerUp.transform.position.x) < Mathf.FloorToInt(playerTransform.position.x - playerSeeRadius))
                        {
                            pool.PoolItem(powerUp);
                        }
                    }
                }
                */

                yield return new WaitForEndOfFrame();
            }
        }

        private void ResetAll()
        {
            foreach (var pair in wallSectionTemplatePairs)
            {
                pair.Value.PoolAllItems();

                foreach (var wallSection in pair.Value.PooledItemsNonAloc)
                {
                    wallSection.Despawn();
                }
            }

            lastWallSectionSpawned = null;

            /*
            wallPool.PoolAllItems();

            foreach (var wall in wallPool.AllItems)
            {
                wall.Initialize(wallPool.PoolItem);
            }

            int current = Mathf.FloorToInt(playerTransform.position.x - playerSeeRadius);

            while (current < playerTransform.position.x + playerSeeRadius)
            {
                var wall = wallPool.GetItem();
                wall.Spawn(new Vector2(current, 0f), Random.Range(minMapMiddleSpace, maxMapMiddleSpace));

                current++;
            }
            */
        }

        private void BuildMap()
        {
            Vector2 current = new Vector2(playerTransform.position.x - playerSeeRadius, 0f);

            while (current.x < playerTransform.position.x + playerSeeRadius)
            {
                WallSection wallSection = GetWallSection(Random.value);
                wallSection.Spawn(current);

                current = wallSection.EndPoint;

                lastWallSectionSpawned = wallSection;
            }
        }

        private WallSection GetWallSection(float value)
        {
            int i = wallSectionTemplatePairs.Count - 1;
            foreach (var pair in wallSectionTemplatePairs)
            {
                if (pair.Key.SpawnChance >= value && pair.Key.MinNormalizedDifficulty >= DifficultyController.Singleton.NormalizedDifficulty)
                {
                    return wallSectionTemplatePairs.ElementAt(Random.Range(0, i)).Value.GetItem();
                }

                i--;
            }

            return wallSectionTemplatePairs[defaultSectionTemplate].GetItem();
        }

        public void SetMapColor(Color color)
        {
            foreach (var pool in wallSectionPools)
            {
                foreach (var wallSection in pool.AllItems)
                {
                    wallSection.SetColor(color);
                }
            }
        }

    }

}
