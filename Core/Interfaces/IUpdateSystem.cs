using UnityEngine;
using System.Collections;

namespace Atlas
{
    public interface IUpdateSystem : ISystem
    {
        void Update(float deltaTime);
    }
}


