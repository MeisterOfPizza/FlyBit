using FlyBit.Controllers;
using FlyBit.Extensions;
using FlyBit.PowerUps;
using FlyBit.Templates;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable 0649

namespace FlyBit.Map
{

    class WallSection : MonoBehaviour
    {

        #region Editor

        [Header("Values")]
        [SerializeField] private float wallColumnWidth = 1f;

        #endregion

        #region Public properties

        public Vector2 EndPoint
        {
            get
            {
                return endPoint;
            }
        }

        #endregion

        #region Private variables

        private SectionTemplate template;

        private GameObjectPool<WallColumn> wallPool;
        private ProbabilityPool<PowerUp>   powerUpPool;

        private Vector2 endPoint;

        private ScorePoint[] scorePoints = new ScorePoint[0];

        #endregion

        #region Life cycle

        public void Initialize(SectionTemplate sectionTemplate)
        {
            this.template = sectionTemplate;

#if UNITY_EDITOR
            gameObject.name = template.name;
#endif

            wallPool    = new GameObjectPool<WallColumn>(transform, template.WallColumnPrefab, template.MaxColumnCount);
            powerUpPool = new ProbabilityPool<PowerUp>(transform, template.GetPrefabProbabilityPairs(), template.MaxPowerUpCount);

            foreach (var pool in powerUpPool.GetPools())
            {
                foreach (var powerUp in pool.PooledItemsNonAloc)
                {
                    powerUp.Initialize(pool.PoolItem);
                }
            }
        }

        public void Spawn(Vector2 startPoint)
        {
            transform.position = startPoint;

            CreateColumnFormation();
        }

        public void Despawn()
        {
            wallPool.PoolAllItems();
            powerUpPool.PoolAll();

            foreach (var scorePoint in scorePoints)
            {
                scorePoint.Despawn();
            }

            powerUpPool.PoolAll();
        }

        public void OpenCloseSection(bool open)
        {
            foreach (var wallColumn in wallPool.ActiveItemsNonAloc)
            {
                wallColumn.OpenCloseColumn(open);
            }

            // The player has died: hide all interactables.
            if (open)
            {
                powerUpPool.PoolAll();

                foreach (var scorePoint in scorePoints)
                {
                    scorePoint.Despawn();
                }
            }
        }

        #endregion

        #region Section creation

        private void CreateColumnFormation()
        {
            int columnCount        = Random.Range(template.MinColumnCount, template.MaxColumnCount + 1);
            var possibleFormations = System.Enum.GetValues(typeof(SectionTemplate.SectionWallFormation)).Cast<byte>().Where(f => ((byte)template.PossibleFormations & f) == f);
            var formation          = possibleFormations.ElementAt(Random.Range(0, possibleFormations.Count()));

            var columnPositions = CreateWallColumns(columnCount, (SectionTemplate.SectionWallFormation)formation);
            SpawnSectionInteractables(columnPositions);
        }

        private Vector2[] CreateWallColumns(int columnCount, SectionTemplate.SectionWallFormation formation)
        {
            Vector2[] columnPositions = new Vector2[columnCount];
            // Make the array two times as big as columnCount to accommodate for top wall and bottom wall positions.
            Vector2[] wallPositions = new Vector2[columnCount * 2];

            switch (formation)
            {
                case SectionTemplate.SectionWallFormation.Line:
                    {
                        for (int i = 0; i < columnCount; i++)
                        {
                            float spacing = Random.Range(template.MinWallColumnSpaceHeight, template.MaxWallColumnSpaceHeight) / 2f;

                            columnPositions[i]       = new Vector2(wallColumnWidth * i, 0f);
                            wallPositions[i * 2]     = new Vector2(0f, spacing);
                            wallPositions[i * 2 + 1] = new Vector2(0f, -spacing);
                        }
                    }
                    break;
                case SectionTemplate.SectionWallFormation.Wave:
                    {
                        float minSpacing = Random.Range(template.MinWallColumnSpaceHeight, template.MaxWallColumnSpaceHeight) / 2f;
                        float direction  = 1 - Random.Range(1, 3) / 2 * 2; // -1 or 1

                        for (int i = 0; i < columnCount; i++)
                        {
                            float spacing = Mathf.Sin(i / (float)(columnCount - 1) * direction * Mathf.PI) * template.WallFormationScale;

                            columnPositions[i]       = new Vector2(wallColumnWidth * i, spacing);
                            wallPositions[i * 2]     = new Vector2(0f, minSpacing);
                            wallPositions[i * 2 + 1] = new Vector2(0f, -minSpacing);
                        }
                    }
                    break;
                case SectionTemplate.SectionWallFormation.Circle:
                    {
                        float minSpacing = Random.Range(template.MinWallColumnSpaceHeight, template.MaxWallColumnSpaceHeight) / 2f;

                        for (int i = 0; i < columnCount; i++)
                        {
                            float spacing = Mathf.Sin(i / (float)(columnCount - 1) * Mathf.PI) * template.WallFormationScale;

                            columnPositions[i]       = new Vector2(wallColumnWidth * i, 0f);
                            wallPositions[i * 2]     = new Vector2(0f, spacing + minSpacing);
                            wallPositions[i * 2 + 1] = new Vector2(0f, -spacing - minSpacing);
                        }
                    }
                    break;
                case SectionTemplate.SectionWallFormation.Climb:
                    {
                        BezierCurve climb   = BezierCurve.CreateSlope(Vector2.zero, new Vector2(columnCount * wallColumnWidth, columnCount * template.WallFormationScale));
                        float       spacing = Random.Range(template.MinWallColumnSpaceHeight, template.MaxWallColumnSpaceHeight) / 2f;

                        for (int i = 0; i < columnCount; i++)
                        {
                            columnPositions[i]       = new Vector2(wallColumnWidth * i, climb.GetPoint(i / (float)(columnCount - 1)).y);
                            wallPositions[i * 2]     = new Vector2(0f, spacing);
                            wallPositions[i * 2 + 1] = new Vector2(0f, -spacing);
                        }
                    }
                    break;
                case SectionTemplate.SectionWallFormation.Drop:
                    {
                        BezierCurve drop    = BezierCurve.CreateSlope(Vector2.zero, new Vector2(columnCount * wallColumnWidth, -columnCount * template.WallFormationScale));
                        float       spacing = Random.Range(template.MinWallColumnSpaceHeight, template.MaxWallColumnSpaceHeight) / 2f;

                        for (int i = 0; i < columnCount; i++)
                        {
                            columnPositions[i]       = new Vector2(wallColumnWidth * i, drop.GetPoint(i / (float)(columnCount - 1)).y);
                            wallPositions[i * 2]     = new Vector2(0f, spacing);
                            wallPositions[i * 2 + 1] = new Vector2(0f, -spacing);
                        }
                    }
                    break;
                case SectionTemplate.SectionWallFormation.Box:
                    {
                        float spacing = Random.Range(template.MinWallColumnSpaceHeight, template.MaxWallColumnSpaceHeight) / 2f * template.WallFormationScale;

                        for (int i = 0; i < columnCount; i++)
                        {
                            columnPositions[i]       = new Vector2(wallColumnWidth * i, 0f);
                            wallPositions[i * 2]     = new Vector2(0f, spacing);
                            wallPositions[i * 2 + 1] = new Vector2(0f, -spacing);
                        }
                    }
                    break;
                case SectionTemplate.SectionWallFormation.Curve:
                    {
                        BezierCurve curve   = new BezierCurve(Vector2.zero, new Vector2(columnCount * wallColumnWidth, columnCount * template.WallFormationScale), new Vector2(0f, columnCount * template.WallFormationScale), new Vector2(columnCount * wallColumnWidth, 0f));
                        float       spacing = Random.Range(template.MinWallColumnSpaceHeight, template.MaxWallColumnSpaceHeight) / 2f;

                        for (int i = 0; i < columnCount; i++)
                        {
                            columnPositions[i]       = new Vector2(wallColumnWidth * i, curve.GetPoint(i / (float)(columnCount - 1)).y);
                            wallPositions[i * 2]     = new Vector2(0f, spacing);
                            wallPositions[i * 2 + 1] = new Vector2(0f, -spacing);
                        }
                    }
                    break;
                default:
                    break;
            }

            endPoint = (Vector2)transform.position + columnPositions[columnPositions.Length - 1] + Vector2.right;

            // Create the wall columns:
            for (int i = 0; i < columnCount; i++)
            {
                var column = wallPool.GetItem();
                column.Spawn(columnPositions[i], wallPositions[i * 2], wallPositions[i * 2 + 1]);
            }

            return columnPositions;
        }

        private void SpawnSectionInteractables(Vector2[] columnPositions)
        {
            int scorePointCount = Mathf.Min(Random.Range(0, Mathf.FloorToInt(columnPositions.Length * template.MaxScorePointFrequency)), MapController.Singleton.ScorePointsAvailableToSpawn);
            int powerUpCount    = Random.Range(0, template.MaxPowerUpCount);

            scorePoints = new ScorePoint[scorePointCount];

            // Make a list of all the available columnPosition indexes:
            List<int> indexes         = Enumerable.Range(0, columnPositions.Length).ToList();
            int       indexesToRemove = Mathf.Min(columnPositions.Length - scorePointCount - powerUpCount, indexes.Count);

            // Randomly remove those that will not be selected:
            for (int i = 0; i < indexesToRemove; i++)
            {
                indexes.RemoveAt(Random.Range(0, indexes.Count));
            }

            // Assign positions to the unpooled score points:
            for (int i = 0; i < scorePointCount; i++)
            {
                scorePoints[i]                    = MapController.Singleton.GetScorePoint();
                scorePoints[i].transform.position = (Vector2)transform.position + columnPositions[indexes[i]];
            }

            // Assign positions to the unpooled power ups:
            for (int i = scorePointCount; i < scorePointCount + powerUpCount; i++)
            {
                PowerUp powerUp = powerUpPool.GetPool(Random.value)?.GetItem() ?? null;

                if (powerUp != null)
                {
                    powerUp.transform.position = (Vector2)transform.position + columnPositions[indexes[i]];
                }
            }
        }

        #endregion

        #region Helpers

        public void SetColor(Color color)
        {
            foreach (var wall in wallPool.AllItems)
            {
                wall.SetColor(color);
            }

            foreach (var pool in powerUpPool.GetPools())
            {
                foreach (var powerUp in pool.AllItems)
                {
                    powerUp.SetColor(color);
                }
            }
        }

        public bool CanDespawn(float despawnThresholdX)
        {
            return transform.position.x + wallPool.ActiveItemCount * wallColumnWidth < despawnThresholdX;
        }

        #endregion

    }

}
