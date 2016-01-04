using System;
using UnityEngine;
using System.Collections.Generic;

namespace Atlas
{

    public abstract class AtlasSystem<T> : AtlasSystem where T : IMatcher
    {

    }

    public abstract class AtlasSystem
    {
        public IMatcher Matcher { get { return _matcher; } }
        IMatcher _matcher;
        protected IAtlasGroup _entityGroup;
        protected AtlasSystem()
        {
            Type[] types = GetType().BaseType.GetGenericArguments();
            _matcher = (IMatcher)Activator.CreateInstance(types[0]);
            _entityGroup = AtlasExtensions.GetGroup(_matcher);
		}
    }
}


