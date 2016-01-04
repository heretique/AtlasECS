using UnityEngine;
using System;
using System.Collections.Generic;

namespace Atlas
{

    public class AtlasGroup : IAtlasGroup
    {
        readonly IMatcher _matcher;
        readonly HashSet<GameObject> _entities = new HashSet<GameObject>();
        List<GameObject> _entitiesList = new List<GameObject>();
        List<EntityAndComponents> _cache = new List<EntityAndComponents>();
        Type[] _typesToCache;

        public List<GameObject> GetEntities()
        {  
            return _entitiesList; 
        }
        public List<EntityAndComponents> GetCachedEntities()
        { 
            return _cache; 
        }

        public AtlasGroup(IMatcher matcher)
        {
            _matcher = matcher;
        }

        public void EnableComponentCachingFor(params Type[] types)
        {
            _typesToCache = types;
            for (var i = 0; i < _entitiesList.Count; ++i)
            {
                if (_typesToCache != null && _typesToCache.Length > 0)
                {
                    EntityAndComponents e = new EntityAndComponents();
                    e.Entity = _entitiesList[i];
                    e.Components = new Component[_typesToCache.Length];
                    for (var j = 0; j < _typesToCache.Length; ++j)
                    {
                        e.Components[j] = _entitiesList[i].GetComponent(_typesToCache[j]);
                    }
                    _cache.Add(e);
                }
            }
        }

        public void HandleEntity(GameObject entity)
        {
            if (!_entities.Contains(entity) && _matcher.Matches(entity))
            {
                AddEntity(entity);
                return;
            }

            if (_entities.Contains(entity) && _matcher.Matches(entity))
            {
                UpdateCache(entity);
            }
            else if (_entities.Contains(entity) && !_matcher.Matches(entity))
            {
                RemoveEntity(entity);
            }
        }

        void AddEntity(GameObject entity)
        {
            _entities.Add(entity);
            _entitiesList.Add(entity);
            if (_typesToCache != null && _typesToCache.Length > 0)
            {
                EntityAndComponents e = new EntityAndComponents();
                e.Entity = entity;
                e.Components = new Component[_typesToCache.Length];
                for (var i = 0;  i < _typesToCache.Length; ++i)
                {
                    e.Components[i] = entity.GetComponent(_typesToCache[i]);
                }
                _cache.Add(e);
            }
        }

        void UpdateCache(GameObject entity)
        {
            if (_typesToCache != null && _typesToCache.Length > 0)
            {
                EntityAndComponents e = new EntityAndComponents();
                e.Entity = entity;
                e.Components = new Component[_typesToCache.Length];
                for (var i = 0; i < _typesToCache.Length; ++i)
                {
                    e.Components[i] = entity.GetComponent(_typesToCache[i]);
                }
                _cache.RemoveAll(p => p.Entity == entity);
                _cache.Add(e);
            }
        }

        public void RemoveEntity(GameObject entity)
        {
            _entities.Remove(entity);
            _entitiesList.Remove(entity);
            _cache.RemoveAll(e => e.Entity == entity);
        }

    }
}


