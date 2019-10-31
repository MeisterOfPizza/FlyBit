using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlyBit.Extensions
{

    sealed class GameObjectPool<T> where T : Component
    {

        #region Public properties

        public T[] ActiveItems
        {
            get
            {
                return activeItems.ToArray();
            }
        }

        public T[] PooledItems
        {
            get
            {
                return pooledItems.ToArray();
            }
        }

        public List<T> ActiveItemsNonAloc
        {
            get
            {
                return activeItems;
            }
        }

        public Queue<T> PooledItemsNonAloc
        {
            get
            {
                return pooledItems;
            }
        }

        public T[] AllItems
        {
            get
            {
                return allItems;
            }
        }

        public int ItemCount
        {
            get
            {
                return itemCount;
            }
        }

        public int ActiveItemCount
        {
            get
            {
                return activeItems.Count;
            }
        }

        public int PooledItemCount
        {
            get
            {
                return pooledItems.Count;
            }
        }

        public bool HasAvailableItems
        {
            get
            {
                return PooledItemCount > 0;
            }
        }

        #endregion

        #region Private variables

        private int itemCount;

        private T[]      allItems;
        private List<T>  activeItems;
        private Queue<T> pooledItems;

        #endregion

        #region Constructors

        public GameObjectPool(Transform parent, GameObject prefab, int prefabInstances, bool setActive = false)
        {
            activeItems = new List<T>(prefabInstances + 5);
            pooledItems = new Queue<T>(prefabInstances + 5);
            allItems    = new T[prefabInstances];

            itemCount = prefabInstances;

            for (int i = 0; i < prefabInstances; i++)
            {
                GameObject go = GameObject.Instantiate(prefab, parent);
                go.SetActive(setActive);
                T item = go.GetComponent<T>();

                if (!setActive)
                {
                    pooledItems.Enqueue(item);
                }
                else
                {
                    activeItems.Add(item);
                }

                allItems[i] = item;
            }
        }

        public GameObjectPool(bool setActive = false, params GameObject[] gameObjects)
        {
            activeItems = new List<T>(gameObjects.Length + 5);
            pooledItems = new Queue<T>(gameObjects.Length + 5);
            allItems    = new T[gameObjects.Length];

            itemCount = gameObjects.Length;

            for (int i = 0; i < gameObjects.Length; i++)
            {
                gameObjects[i].SetActive(false);
                T item = gameObjects[i].GetComponent<T>();

                if (!setActive)
                {
                    pooledItems.Enqueue(item);
                }
                else
                {
                    activeItems.Add(item);
                }

                allItems[i] = item;
            }
        }

        #endregion

        #region Pooling

        public void PoolItem(T item)
        {
            if (activeItems.Remove(item))
            {
                pooledItems.Enqueue(item);
                item.gameObject.SetActive(false);
            }
        }

        public T GetItem()
        {
            if (pooledItems.Count > 0)
            {
                T item = pooledItems.Dequeue();
                activeItems.Add(item);
                item.gameObject.SetActive(true);

                return item;
            }

            return null;
        }

        public void PoolAllItems()
        {
            foreach (var item in activeItems.ToList())
            {
                pooledItems.Enqueue(item);
                item.gameObject.SetActive(false);
            }

            activeItems.Clear();
        }

        #endregion

    }

}
