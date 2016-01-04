using UnityEngine;
using System.Collections.Generic;

namespace Atlas
{
    public static class AtlasExtensions
    {
        private static IAtlasPool _entityPool;
        private static ISystemAggregator _systemAggregator;
        private static IEventAggregator _eventAgregator;

        #region Entity Instantiation and Pooling
        private static Transform _poolHolder = null;
        private static readonly Dictionary<System.Int32, Stack<GameObject>> _poolsByHash = new Dictionary<int, Stack<GameObject>>();
        private static readonly Dictionary<GameObject, GameObject> _poolInstances = new Dictionary<GameObject, GameObject>();
        #endregion

        public static void SetEntityPool(IAtlasPool pool)
        {
            _entityPool = pool;
        }

        public static IAtlasGroup GetGroup(IMatcher matcher)
        {
            return _entityPool.GetGroup(matcher);
        }

        public static void SetEventAggregator(IEventAggregator eventAggregator)
        {
            _eventAgregator = eventAggregator;
        }

        public static void SetSystemAggregator(ISystemAggregator systems)
        {
            _systemAggregator = systems;
        }

        public static void RegisterComponent(this AtlasComponent component)
        {
            _entityPool.RegisterComponent(component);
        }

        public static void UnregisterComponent(this AtlasComponent component)
        {
            _entityPool.UnregisterComponent(component);
        }

        public static void PublishEvent<TEventType>()
        {
            _eventAgregator.PublishEvent<TEventType>();
        }

        public static void PublishEvent<TEventType>(TEventType eventToPublish)
        {
            _eventAgregator.PublishEvent<TEventType>(eventToPublish);
        }

        public static void SubscribeEvent(object subscriber)
        {
            _eventAgregator.SubscribeEvent(subscriber);
        }

        public static void UnsubscribeEvent(object subscriber)
        {
            _eventAgregator.UnsubscribeEvent(subscriber);
        }

        public static void AddSystem(this ISystem system)
        {
            _systemAggregator.AddSystem(system);
        }

        public static void RemoveSystem(ISystem system)
        {
            _systemAggregator.RemoveSystem(system);
        }

        public static void ToggleSystem(ISystem system)
        {
            _systemAggregator.ToggleSystem(system);
        }

        #region Entity Instantiation and Pooling

        public static void CreatePool(GameObject prefab, int size)
        {
            if (_poolHolder == null)
            {
                _poolHolder = new GameObject("_PoolHolder").transform;
            }

            Stack<GameObject> stack = new Stack<GameObject>(size);
            for (int i = 0; i < size; ++i)
            {
                GameObject instance = GameObject.Instantiate<GameObject>(prefab);
                instance.name = prefab.name;
                instance.transform.parent = _poolHolder;
                instance.SetActive(false);
                stack.Push(instance);
            }
            _poolsByHash.Add(prefab.name.GetHashCode(), stack);
        }


        public static bool IsPooled(this GameObject go)
        {
            return _poolInstances.ContainsKey(go);
        }

        public static GameObject GetInstance(int hash, Vector3? position = null, Quaternion? rotation = null)
        {
            GameObject instance = null;
            Vector3 pos = position ?? Vector3.zero;
            Quaternion rot = rotation ?? Quaternion.identity;
            if (_poolsByHash.ContainsKey(hash))
            {
                Stack<GameObject> pool = _poolsByHash[hash];
                // always keep an instance in pool for instancing new one based on hash
                // if the pool runs out
                if (pool.Count > 1)
                {
                    instance = pool.Pop();
                }
                else
                {
                    GameObject prefab = pool.Peek();
                    string prefabName = prefab.name;
                    Debug.LogWarning(string.Format("Pool [{0}] ran out of instances!", prefabName));
                    instance = GameObject.Instantiate<GameObject>(prefab);
                    instance.name = prefabName;
                }
                instance.SetActive(true);
                _poolInstances.Add(instance, instance);
            }

            return instance;
        }

        public static GameObject Spawn(this GameObject prefab)
        {
            // don't have to worry about registering to entity pool
            // the first AtlasComponent activated on this entity will register it
            GameObject instance = GetInstance(prefab.name.GetHashCode());
            if (instance == null)
                instance = GameObject.Instantiate<GameObject>(prefab);

            return instance;
        }

        public static GameObject Spawn(this GameObject prefab, Vector3 position, Quaternion rotation)
        {
            // don't have to worry about registering to entity pool
            // the first AtlasComponent activated on this entity will register it
            GameObject instance = GetInstance(prefab.name.GetHashCode(), position, rotation);
            if (instance == null)
            {
                instance = GameObject.Instantiate<GameObject>(prefab);
            }

            Transform tr = instance.transform;
            tr.position = position;
            tr.rotation = rotation;

            return instance;
        }

        public static void Destroy(this GameObject go)
        {
            if (go.IsPooled())
            {
                // remove from object pool
                _poolInstances.Remove(go);
                _poolsByHash[go.name.GetHashCode()].Push(go);
                Transform tr = go.transform;
                tr.parent = _poolHolder;
                tr.position = Vector3.zero;
                tr.rotation = Quaternion.identity;
                go.SendMessage("OnDestroy", SendMessageOptions.DontRequireReceiver);
                go.SetActive(false);
                return;
            }

            // remove for entities pool
            _entityPool.RemoveEntity(go);

            // actually destroy it
            GameObject.Destroy(go);
        }
        #endregion

    }
}


