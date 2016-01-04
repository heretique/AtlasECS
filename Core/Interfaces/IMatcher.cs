using UnityEngine;
using System.Collections;

namespace Atlas
{
    public interface IMatcher
    {
        bool Matches(GameObject entity);
    }
}
