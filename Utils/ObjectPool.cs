using UnityEngine;
using System.Collections.Generic;

namespace Atlas
{
    public class ObjectPool : MonoBehaviour
    {

        [System.Serializable]
        public class Pool
        {
            public GameObject prefab;
            public int size;
        }

        public Pool[] pools;

        void Awake()
        {
            for (int i = 0; i < pools.Length; ++i)
            {
                CreatePool(pools[i]);
            }
        }

        void CreatePool(Pool pool)
        {
            AtlasExtensions.CreatePool(pool.prefab, pool.size);
        }
    }
}

