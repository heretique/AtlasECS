using UnityEngine;
using System.Collections;

namespace Atlas
{
    public interface IFixedUpdateSystem : ISystem
    {
        void FixedUpdate(float fixedDelta);
    }
}


