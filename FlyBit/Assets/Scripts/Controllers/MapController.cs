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
        [SerializeField] private float screenHeight    = 6f;

        #endregion

        #region Public properties

        public float PlayerSeeRadius
        {
            get
            {
                return playerSeeRadius;
            }
        }

        public float ScreenHeight
        {
            get
            {
                return screenHeight;
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

        #region Life cycle

        public override void OnAwake()
        {
            // Create a new array with the default section template added in and sort it by spawn chance.
            var templates            = (new SectionTemplate[] { startSectionTemplate, defaultSectionTemplate }).Concat(sectionTemplates.OrderBy(st => st.SpawnChance)).ToArray();
            wallSectionTemplatePairs = new Dictionary<SectionTemplate, GameObjectPool<WallSection>>(templates.Length);
            wallSectionPools         = new GameObjectPool<WallSection>[templates.Length];

            for (int i = 0; i < templates.Length; i++)
            {
                var pool = new GameObjectPool<WallSection>(mapContainer, wallSectionPrefab, templates[i] == startSectionTemplate ? 2 : 4);
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
            RebuildMap(0f);
        }

        public void End()
        {
            StopAllCoroutines();

            OpenCloseMap(true);
        }

        #endregion

        public void MoveMap(Vector3 delta)
        {
            if (GameController.Singleton.IsMatchRunning)
            {
                foreach (var pool in wallSectionPools)
                {
                    foreach (var wallSection in pool.ActiveItems)
                    {
                        wallSection.transform.position -= delta;

                        if (wallSection.CanDespawn(-playerSeeRadius))
                        {
                            wallSection.Despawn();
                            pool.PoolItem(wallSection);
                        }
                    }
                }

                while (Mathf.FloorToInt(lastWallSectionSpawned.EndPoint.x) < playerSeeRadius)
                {
                    WallSection nextWallSection = GetWallSection(Random.value);
                    nextWallSection.Spawn(lastWallSectionSpawned.EndPoint);

                    lastWallSectionSpawned = nextWallSection;
                }

                foreach (var scorePoint in scorePointPool.ActiveItemsNonAloc)
                {
                    scorePoint.transform.position -= delta;
                }
            }
        }

        #region Building the map

        public void RebuildMap(float offset, SectionTemplate startSection = null)
        {
            ResetAll();
            BuildMap(offset, startSection ?? startSectionTemplate);

            OpenCloseMap(false);
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

        private void BuildMap(float offset, SectionTemplate startSection)
        {
            // Start the current at half the length of the start section.
            Vector2 current = new Vector2(-20f + offset, 0f);

            WallSection wallSection = wallSectionTemplatePairs[startSection].GetItem();
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
                if (pair.Key.SpawnChance >= value)
                {
                    var valuablePairs = wallSectionTemplatePairs.Take(i).Where(p => p.Value.HasAvailableItems && p.Key.MinNormalizedDifficulty <= DifficultyController.Singleton.NormalizedDifficulty);

                    return valuablePairs.Count() > 2 ? valuablePairs.ElementAt(Random.Range(2, valuablePairs.Count())).Value.GetItem() : wallSectionTemplatePairs[defaultSectionTemplate].GetItem();
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

        public void OpenCloseMap(bool open)
        {
            foreach (var pool in wallSectionPools)
            {
                foreach (var wallSection in pool.ActiveItemsNonAloc)
                {
                    wallSection.OpenCloseSection(open);
                }
            }
        }

        #endregion

    }

}
