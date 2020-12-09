using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Pipeline
{
    internal interface ILifecycleObject
    {
        void OnSceneLoaded();
        void OnSceneUnloaded();
    }
}