using UnityEngine;
using System.Collections;

namespace Atlas {
	public class DelayedActionSystem : AtlasSystem<Requires<DelayedAction>>, IUpdateSystem {

        public DelayedActionSystem()
            : base()
        {
            _entityGroup.EnableComponentCachingFor(typeof(DelayedAction));
        }

        void IUpdateSystem.Update(float deltaTime)
        {
            var entities = _entityGroup.GetCachedEntities();
            for (var i = 0; i < entities.Count; ++i)
            {
                DelayedAction delayedAction = (DelayedAction)entities[i].Components[0];
                if (!delayedAction.Running)
                {
                    continue;
                }

                delayedAction.Delay -= Time.deltaTime;

                if (delayedAction.Delay > 0)
                {
                    if (delayedAction.OnUpdate != null)
                    {
                        delayedAction.OnUpdate();
                    }
                }
                else
                {
                    if (delayedAction.OnComplete != null)
                    {
                        delayedAction.OnComplete();
                    }

                    // TODO: This may be troublesome
                    // whe should add this to a list of components in the pool that need to be destroyed
                    // at the end of the current update/frame
                    GameObject.Destroy(delayedAction);
                }
            }
        }
	}
}