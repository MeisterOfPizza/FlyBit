using FlyBit.Extensions;
using FlyBit.PowerUps;
using FlyBit.Templates;
using System.Linq;
using UnityEngine;

#pragma warning disable 0649

namespace FlyBit.Map
{

    class WallSection : MonoBehaviour
    {

        #region Editor

        [Header("Values")]
        [SerializeField] private float wallColumnWidth          = 1f;
        [SerializeField] private float minWallColumnSpaceHeight = 2.5f;
        [SerializeField] private float maxWallColumnSpaceHeight = 5f;

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
        private GameObjectPool<ScorePoint> scorePointPool;
        private ProbabilityPool<PowerUp>   powerUpPool;

        private Vector2 endPoint;

        #endregion

        public void Initialize(SectionTemplate sectionTemplate)
        {
            this.template = sectionTemplate;

            wallPool       = new GameObjectPool<WallColumn>(transform, template.WallColumnPrefab, template.MaxColumnCount);
            scorePointPool = new GameObjectPool<ScorePoint>(transform, template.ScorePointPrefab, Mathf.CeilToInt(template.MaxScorePointFrequency * template.MaxColumnCount));
            powerUpPool    = new ProbabilityPool<PowerUp>(transform, template.GetPrefabProbabilityPairs(), template.MaxPowerUpCount);

            foreach (var scorePoint in scorePointPool.PooledItemsNonAloc)
            {
                scorePoint.Initialize(scorePointPool.PoolItem);
            }

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
            scorePointPool.PoolAllItems();
            powerUpPool.PoolAll();
        }

        public bool CanDespawn(float despawnThresholdX)
        {
            return transform.position.x + wallPool.ActiveItemCount * wallColumnWidth < despawnThresholdX;
        }

        private void CreateColumnFormation()
        {
            int columnCount        = Random.Range(template.MinColumnCount, template.MaxColumnCount + 1);
            var possibleFormations = System.Enum.GetValues(typeof(SectionTemplate.SectionWallFormation)).Cast<byte>().Where(f => ((byte)template.PossibleFormations & f) == f);
            var formation          = possibleFormations.ElementAt(Random.Range(0, possibleFormations.Count()));

            var positions = CalculateWallColumnPositions(columnCount, (SectionTemplate.SectionWallFormation)formation);

            for (int i = 0; i < columnCount; i++)
            {
                var column = wallPool.GetItem();
                column.Spawn(positions[i * 3], positions[i * 3 + 1], positions[i * 3 + 2]);
            }
        }

        private Vector2[] CalculateWallColumnPositions(int columnCount, SectionTemplate.SectionWallFormation formation)
        {
            // Make the array three times as big as columnCount to accommodate for the wall column, top wall and bottom wall position.
            Vector2[] positions = new Vector2[columnCount * 3];

            switch (formation)
            {
                case SectionTemplate.SectionWallFormation.Line:
                case SectionTemplate.SectionWallFormation.Climb:
                case SectionTemplate.SectionWallFormation.Drop:
                case SectionTemplate.SectionWallFormation.Edge:
                    for (int i = 0; i < columnCount; i++)
                    {
                        float spacing = Random.Range(minWallColumnSpaceHeight, maxWallColumnSpaceHeight) / 2f;

                        positions[i * 3]     = new Vector2(wallColumnWidth * i, 0f);
                        positions[i * 3 + 1] = new Vector2(0f, spacing);
                        positions[i * 3 + 2] = new Vector2(0f, -spacing);
                    }
                    break;
                case SectionTemplate.SectionWallFormation.Wave:
                    {
                        float minSpacing = Random.Range(minWallColumnSpaceHeight, maxWallColumnSpaceHeight) / 2f;
                        float direction  = 1 - Random.Range(1, 3) / 2 * 2; // -1 or 1

                        for (int i = 0; i < columnCount; i++)
                        {
                            float spacing = Mathf.Sin(i / (float)positions.Length * direction) / 2f + minSpacing;

                            positions[i * 3]     = new Vector2(wallColumnWidth * i, 0f);
                            positions[i * 3 + 1] = new Vector2(0f, spacing);
                            positions[i * 3 + 2] = new Vector2(0f, -spacing);
                        }
                    }
                    break;
                case SectionTemplate.SectionWallFormation.Circle:
                    {
                        float minSpacing = Random.Range(minWallColumnSpaceHeight, maxWallColumnSpaceHeight) / 2f;

                        for (int i = 0; i < columnCount; i++)
                        {
                            float spacing = Mathf.Sin(i / (float)positions.Length) / 2f + minSpacing;

                            positions[i * 3]     = new Vector2(wallColumnWidth * i, 0f);
                            positions[i * 3 + 1] = new Vector2(0f, spacing);
                            positions[i * 3 + 2] = new Vector2(0f, -spacing);
                        }
                    }
                    break;
                default:
                    break;
            }

            endPoint = (Vector2)transform.position + positions[positions.Length - 3];

            return positions;
        }

        public void SetColor(Color color)
        {
            foreach (var wall in wallPool.AllItems)
            {
                wall.SetColor(color);
            }

            foreach (var scorePoint in scorePointPool.AllItems)
            {
                scorePoint.SetColor(color);
            }

            foreach (var pool in powerUpPool.GetPools())
            {
                foreach (var powerUp in pool.AllItems)
                {
                    powerUp.SetColor(color);
                }
            }
        }

    }

}
