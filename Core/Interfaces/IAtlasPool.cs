using UnityEngine;
using System.Collections;

namespace Atlas
{
    public interface IAtlasPool
    {
        void RegisterComponent(AtlasComponent component);
        void UnregisterComponent(AtlasComponent component);
        void RemoveEntity(GameObject entity);
        IAtlasGroup GetGroup(IMatcher matcher);
    }
}


