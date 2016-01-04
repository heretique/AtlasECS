using UnityEngine;
using System.Collections;

namespace Atlas
{
    public interface ISystemAggregator
    {
        void AddSystem(ISystem system);
        void RemoveSystem(ISystem system);
        void ToggleSystem(ISystem system);
        ISystem[] Systems();
    }
}


