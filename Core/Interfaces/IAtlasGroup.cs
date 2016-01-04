using UnityEngine;
using System;
using System.Collections.Generic;

namespace Atlas
{

    public class EntityAndComponents : IEquatable<EntityAndComponents>
    {
        public GameObject Entity;
        public Component[] Components;


        public bool Equals(EntityAndComponents other)
        {
            return this.Entity == other.Entity;
        }
    }

    public interface IAtlasGroup
    {
        void EnableComponentCachingFor(params Type[] types);
        void HandleEntity(GameObject entity);
        void RemoveEntity(GameObject entity);

        List<GameObject> GetEntities();
        List<EntityAndComponents> GetCachedEntities();
    }
}

