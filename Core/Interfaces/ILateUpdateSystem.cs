using UnityEngine;
using System.Collections;

namespace Atlas
{
    public interface ILateUpdateSystem : ISystem
    {
        void LateUpdate(float deltaTime);
    }
}


