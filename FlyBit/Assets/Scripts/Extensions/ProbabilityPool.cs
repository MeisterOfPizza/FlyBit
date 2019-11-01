using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlyBit.Extensions
{

    sealed class ProbabilityPool<T> where T : Component
    {

        #region Private variables

        private Tuple<GameObjectPool<T>, float>[] probabilityPoolPairs;

        #endregion

        public ProbabilityPool(Transform parent, Tuple<GameObject, float>[] prefabProbabilityPairs, int prefabInstances, bool setActive = false)
        {
            probabilityPoolPairs   = new Tuple<GameObjectPool<T>, float>[prefabProbabilityPairs.Length];
            prefabProbabilityPairs = prefabProbabilityPairs.OrderBy(p => p.Item2).ToArray();

            for (int i = 0; i < prefabProbabilityPairs.Length; i++)
            {
                probabilityPoolPairs[i] = new Tuple<GameObjectPool<T>, float>(
                    new GameObjectPool<T>(parent, prefabProbabilityPairs[i].Item1, prefabInstances, setActive),
                    prefabProbabilityPairs[i].Item2
                    );
            }
        }

        public GameObjectPool<T> GetPool(float value)
        {
            for (int i = 0; i < probabilityPoolPairs.Length; i++)
            {
                if (probabilityPoolPairs[i].Item2 >= value)
                {
                    return probabilityPoolPairs[UnityEngine.Random.Range(i, probabilityPoolPairs.Length)].Item1;
                }
            }

            return null;
        }

        public IEnumerable<GameObjectPool<T>> GetPools()
        {
            return probabilityPoolPairs.Select(p => p.Item1);
        }

        public void PoolAll()
        {
            foreach (var pair in probabilityPoolPairs)
            {
                pair.Item1.PoolAllItems();
            }
        }

    }

}
