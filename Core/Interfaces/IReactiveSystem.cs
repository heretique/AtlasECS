using UnityEngine;
using System.Collections;

namespace Atlas
{

    public interface IReactiveSystem: ISystem
    {

    }

    public interface IReactiveSystem<TEventType> : ISubscriber<TEventType>, IReactiveSystem
    {

    }
}

