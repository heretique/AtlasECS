using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atlas
{

    public class AtlasPool : MonoBehaviour, IAtlasPool
    {
        public event GroupChanged OnGroupCreated = delegate { };
        public delegate void GroupChanged(IAtlasGroup group);

        readonly HashSet<GameObject> _entities = new HashSet<GameObject>();
        List<GameObject> _entitiesList = new List<GameObject>();
        readonly Dictionary<IMatcher, IAtlasGroup> _groups = new Dictionary<IMatcher, IAtlasGroup>();
        public List<GameObject> Entities { get { return _entitiesList; } }
        private Queue<GameObject> _unregisterComponents = new Queue<GameObject>();

        void Awake()
        {
            AtlasExtensions.SetEntityPool(this);
            StartCoroutine(UnregisterComponentsCoroutine());
        }

        public void RegisterComponent(AtlasComponent component)
        {
            if (!component.enabled)
                return;

            GameObject entity = component.gameObject;
            if (!_entities.Contains(entity))
            {
                _entities.Add(entity);
                _entitiesList.Add(entity);
            }
                

            var groups = _groups.Values.GetEnumerator();
            while (groups.MoveNext())
            {
                groups.Current.HandleEntity(entity);
            }
        }

        public void UnregisterComponent(AtlasComponent component)
        {
            GameObject entity = component.gameObject;
            _unregisterComponents.Enqueue(entity);
        }

        void HandleUnregisterComponent(GameObject entity)
        {
            if (entity == null)
            {
                RemoveEntity(entity);
            }
            else
            {
                Atlas.AtlasComponent otherComponent = entity.GetComponent<AtlasComponent>();
                if (otherComponent != null)
                {
                    var groups = _groups.Values.GetEnumerator();
                    while (groups.MoveNext())
                    {
                        groups.Current.HandleEntity(entity);
                    }
                }
                else
                {
                    RemoveEntity(entity);
                }
            }
        }
        
        public void RemoveEntity(GameObject entity)
        {
            _entities.Remove(entity);
            _entitiesList.Remove(entity);

            var groups = _groups.Values.GetEnumerator();
            while (groups.MoveNext())
            {
                groups.Current.RemoveEntity(entity);
            }
        }


        public IAtlasGroup GetGroup(IMatcher matcher)
        {
            IAtlasGroup group;
            if (!_groups.TryGetValue(matcher, out group))
            {
                group = new AtlasGroup(matcher);
                for (int i = 0, entitiesLength = Entities.Count; i < entitiesLength; i++)
                {
                    group.HandleEntity(Entities[i]);
                }
                _groups.Add(matcher, group);
                 OnGroupCreated(group);
            }

            return group;
        }


        //void Update()
        //{
        //    while (_unregisterComponents.Count > 0)
        //    {
        //        HandleUnregisterComponent(_unregisterComponents.Dequeue());
        //        _unregisterComponents.Clear();
        //    }
        //}

        IEnumerator UnregisterComponentsCoroutine()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                while (_unregisterComponents.Count > 0)
                {
                    HandleUnregisterComponent(_unregisterComponents.Dequeue());
                }
            }
        }

    }
}


