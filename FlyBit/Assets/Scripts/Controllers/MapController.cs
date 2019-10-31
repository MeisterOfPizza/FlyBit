using FlyBit.Extensions;
using FlyBit.Map;
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
        [SerializeField] private SectionTemplate   startSectionTemplate;
        [SerializeField] private SectionTemplate   defaultSectionTemplate;
        [SerializeField] private SectionTemplate[] sectionTemplates;

        [Header("Prefabs")]
        [SerializeField] private GameObject wallSectionPrefab;
        [SerializeField] private GameObject scorePointPrefab;

        [Header("Values")]
        [SerializeField] private float playerSeeRadius = 20f;

        #endregion

        #region Public properties

        public float PlayerSeeRadius
        {
            get
            {
                return playerSeeRadius;
            }
        }

        public int ScorePointsAvailableToSpawn
        {
            get
            {
                return scorePointPool.PooledItemCount;
            }
        }

        #endregion

        #region Private variables

        private Dictionary<SectionTemplate, GameObjectPool<WallSection>> wallSectionTemplatePairs;
        private GameObjectPool<WallSection>[]                            wallSectionPools;
        private GameObjectPool<ScorePoint>                               scorePointPool;

        private WallSection lastWallSectionSpawned;

        #endregion

        #region Lifecycle

        public override void OnAwake()
        {
            // Create a new array with the default section template added in and sort it by spawn chance.
            var templates            = (new SectionTemplate[] { startSectionTemplate, defaultSectionTemplate }).Concat(sectionTemplates.OrderBy(st => st.SpawnChance)).ToArray();
            wallSectionTemplatePairs = new Dictionary<SectionTemplate, GameObjectPool<WallSection>>(templates.Length);
            wallSectionPools         = new GameObjectPool<WallSection>[templates.Length];

            for (int i = 0; i < templates.Length; i++)
            {
                var pool = new GameObjectPool<WallSection>(mapContainer, wallSectionPrefab, templates[i] == defaultSectionTemplate ? 20 : 3);
                wallSectionTemplatePairs.Add(templates[i], pool);
                wallSectionPools[i] = pool;

                foreach (var section in pool.PooledItemsNonAloc)
                {
                    section.Initialize(templates[i]);
                }
            }

            scorePointPool = new GameObjectPool<ScorePoint>(mapContainer, scorePointPrefab, templates.Length * 50);

            foreach (var scorePoint in scorePointPool.PooledItemsNonAloc)
            {
                scorePoint.Initialize(scorePointPool);
            }
        }

        public void Begin()
        {
            ResetAll();
            BuildMap();

            foreach (var pool in wallSectionPools)
            {
                foreach (var wallSection in pool.ActiveItemsNonAloc)
                {
                    wallSection.OpenCloseSection(false);
                }
            }

            StartCoroutine("SpawnCycle");
        }

        public void End()
        {
            StopAllCoroutines();

            foreach (var pool in wallSectionPools)
            {
                foreach (var wallSection in pool.ActiveItemsNonAloc)
                {
                    wallSection.OpenCloseSection(true);
                }
            }
        }

        #endregion

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
                        }
                    }
                }

                while (lastWallSectionSpawned.EndPoint.x < playerTransform.position.x + playerSeeRadius)
                {
                    WallSection nextWallSection = GetWallSection(Random.value);
                    nextWallSection.Spawn(lastWallSectionSpawned.EndPoint);

                    lastWallSectionSpawned = nextWallSection;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        #region Building the map

        public void RebuildMap()
        {
            ResetAll();
            BuildMap();
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
        }

        private void BuildMap()
        {
            Vector2 current = new Vector2(playerTransform.position.x - playerSeeRadius, 0f);

            WallSection wallSection = wallSectionTemplatePairs[startSectionTemplate].GetItem();
            wallSection.Spawn(current);

            current = wallSection.EndPoint;

            lastWallSectionSpawned = wallSection;

            while (current.x < playerTransform.position.x + playerSeeRadius)
            {
                wallSection = wallSectionTemplatePairs[defaultSectionTemplate].GetItem();
                wallSection.Spawn(current);

                current = wallSection.EndPoint;

                lastWallSectionSpawned = wallSection;
            }
        }

        #endregion

        #region Helpers

        private WallSection GetWallSection(float value)
        {
            int i = wallSectionTemplatePairs.Count;
            foreach (var pair in wallSectionTemplatePairs)
            {
                if (pair.Key.SpawnChance >= value && pair.Key.MinNormalizedDifficulty >= DifficultyController.Singleton.NormalizedDifficulty)
                {
                    var valuablePairs = wallSectionTemplatePairs.Take(i).Where(p => p.Value.HasAvailableItems);

                    return valuablePairs.Count() > 0 ? valuablePairs.ElementAt(Random.Range(2, valuablePairs.Count())).Value.GetItem() : wallSectionTemplatePairs[defaultSectionTemplate].GetItem();
                }

                i--;
            }

            return wallSectionTemplatePairs[defaultSectionTemplate].GetItem();
        }

        public ScorePoint GetScorePoint()
        {
            return scorePointPool.GetItem();
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

            foreach (var scorePoint in scorePointPool.AllItems)
            {
                scorePoint.SetColor(color);
            }
        }

        #endregion

    }

}
