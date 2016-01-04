using UnityEngine;
using System.Collections;

namespace Atlas
{
    public class DontDestroyOnLoad : MonoBehaviour
    {

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}


