using UnityEngine;
using System.Collections;

namespace Atlas {
	public class ExpirableSystem : AtlasSystem<Requires<Expirable>>, IUpdateSystem {

        public ExpirableSystem()
            : base()
        {
            _entityGroup.EnableComponentCachingFor(typeof(Expirable));
        }

        void IUpdateSystem.Update(float deltaTime)
        {
            var entities = _entityGroup.GetCachedEntities();
            for (var i = 0; i < entities.Count; ++i)
            {
                Expirable expirable = (Expirable)entities[i].Components[0];
                expirable.TimeRemaining -= Time.deltaTime;

                if (expirable.TimeRemaining <= 0)
                {
                    expirable.Target.Destroy();
                }
            }
        }
	}
}