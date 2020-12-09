using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Pipeline
{
    class SceneLoadManager
    {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
        }

        private static void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var gameObjects = scene.GetRootGameObjects();
            foreach (var gameObject in gameObjects)
            {
                var lifecycles = gameObject.GetComponents<ILifecycleObject>();
                foreach (var lifecycle in lifecycles)
                {
                    lifecycle.OnSceneLoaded();
                }
            }
        }

        private static void SceneManager_sceneUnloaded(Scene scene)
        {
            var gameObjects = scene.GetRootGameObjects();
            foreach (var gameObject in gameObjects)
            {
                var lifecycles = gameObject.GetComponents<ILifecycleObject>();
                foreach (var lifecycle in lifecycles)
                {
                    lifecycle.OnSceneUnloaded();
                }
            }
        }
    }
}
