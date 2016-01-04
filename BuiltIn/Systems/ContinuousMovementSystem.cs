using UnityEngine;
using System.Collections;

namespace Atlas
{
    public class ContinuousMovementSystem : AtlasSystem<Requires<ContinuousMovement>>, IUpdateSystem
    {

        public ContinuousMovementSystem()
            : base()
        {
            _entityGroup.EnableComponentCachingFor(typeof(ContinuousMovement));
        }

        void IUpdateSystem.Update(float deltaTime)
        {
            var entities = _entityGroup.GetCachedEntities();
            for (var i = 0; i < entities.Count; ++i)
            {
                ContinuousMovement comp = (ContinuousMovement)entities[i].Components[0];
                entities[i].Entity.transform.Translate(comp.Speed * comp.Direction * deltaTime);
            }
        }
    }
}