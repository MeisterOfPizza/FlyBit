using FlyBit.Controllers;
using FlyBit.Extensions;
using System.Collections;
using UnityEngine;

#pragma warning disable 0649

namespace FlyBit.Map
{

    sealed class HyperdriveWallSection : MonoBehaviour
    {

        #region Editor

        [Header("References")]
        [SerializeField] private GameObject wallColumnPrefab;
        [SerializeField] private GameObject scorePointPrefab;

        [Header("Values")]
        [SerializeField] private int   wallColumnCount = 50;
        [SerializeField] private int   scorePointCount = 50;
        [SerializeField] private float moveSpeed       = 10f;

        [Space]
        [SerializeField, Range(0f, 1f)] private float scorePointSpawnChance = 0.01f;

        #endregion

        #region Private variables

        private GameObjectPool<WallColumn> wallColumnPool = new GameObjectPool<WallColumn>();
        private GameObjectPool<ScorePoint> scorePointPool = new GameObjectPool<ScorePoint>();

        private WallColumn lastSpawnedWallColumn;

        private bool isPlaying;

        #endregion

        private void Awake()
        {
            wallColumnPool = new GameObjectPool<WallColumn>(transform, wallColumnPrefab, wallColumnCount);
            scorePointPool = new GameObjectPool<ScorePoint>(transform, scorePointPrefab, scorePointCount);

            foreach (var scorePoint in scorePointPool.PooledItemsNonAloc)
            {
                scorePoint.Initialize(scorePointPool);
            }
        }

        public void Play()
        {
            isPlaying = true;

            SpawnWallColumns();

            StopCoroutine("Move");
            StartCoroutine("Move");
        }

        public void Stop()
        {
            isPlaying = false;

            scorePointPool.PoolAllItems();

            StopCoroutine("Move");
        }

        public void ResetSection()
        {
            wallColumnPool.PoolAllItems();
        }

        private void SpawnWallColumns()
        {
            var wallColumn = wallColumnPool.GetItem();

            wallColumn.Spawn(Vector2.left * MapController.Singleton.PlayerSeeRadius, new Vector2(0, MapController.Singleton.ScreenHeight), new Vector2(0, -MapController.Singleton.ScreenHeight));
            lastSpawnedWallColumn = wallColumn;

            while (lastSpawnedWallColumn.transform.position.x < MapController.Singleton.PlayerSeeRadius)
            {
                wallColumn = wallColumnPool.GetItem();

                wallColumn.Spawn(lastSpawnedWallColumn.transform.position + Vector3.right, new Vector2(0, MapController.Singleton.ScreenHeight), new Vector2(0, -MapController.Singleton.ScreenHeight));
                lastSpawnedWallColumn = wallColumn;
            }
        }

        private IEnumerator Move()
        {
            while (isPlaying)
            {
                Vector3 delta = new Vector2(moveSpeed * Time.deltaTime, 0f);

                foreach (var wallColumn in wallColumnPool.ActiveItems)
                {
                    wallColumn.transform.position -= delta;

                    if (wallColumn.transform.position.x < -MapController.Singleton.PlayerSeeRadius)
                    {
                        wallColumnPool.PoolItem(wallColumn);
                    }
                }

                while (lastSpawnedWallColumn.transform.position.x < MapController.Singleton.PlayerSeeRadius)
                {
                    var wallColumn = wallColumnPool.GetItem();

                    wallColumn.Spawn(lastSpawnedWallColumn.transform.position + Vector3.right, Vector2.zero, Vector2.zero);
                    wallColumn.OpenCloseColumn(true);
                    lastSpawnedWallColumn = wallColumn;

                    if (scorePointSpawnChance >= Random.value && scorePointPool.HasAvailableItems)
                    {
                        var scorePoint = scorePointPool.GetItem();
                        scorePoint.transform.position = new Vector3(lastSpawnedWallColumn.transform.position.x, Random.Range(-MapController.Singleton.ScreenHeight / 2f, MapController.Singleton.ScreenHeight / 2f));
                    }
                }

                foreach (var scorePoint in scorePointPool.ActiveItems)
                {
                    scorePoint.transform.position -= delta;

                    if (scorePoint.transform.position.x < -MapController.Singleton.PlayerSeeRadius)
                    {
                        scorePointPool.PoolItem(scorePoint);
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }

        #region Helpers

        public void SetColor(Color color)
        {
            foreach (var wallColumn in wallColumnPool.AllItems)
            {
                wallColumn.SetColor(color);
            }

            foreach (var scorePoint in scorePointPool.AllItems)
            {
                scorePoint.SetColor(color);
            }
        }

        #endregion

    }

}
