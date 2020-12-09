using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Editor
{
    public class Pathfinding
    {
        [MenuItem("Tools/Pathfinding/Test")]
        public static void Test()
        {
            var scene = EditorSceneManager.GetActiveScene();
            var navNodes = ReadNavNodes(scene);



            Debug.Log(navNodes.Length);
        }

        private static NavNode[] ReadNavNodes(Scene scene)
        {
            var gameObjects = scene.GetRootGameObjects();
            var navNodeLength = 0;
            foreach (var gameObject in gameObjects)
                if (gameObject.GetComponent<NavNode>() != null)
                    navNodeLength += 1;

            var i = 0;
            var navNodes = new NavNode[navNodeLength];
            foreach (var gameObject in gameObjects)
            {
                var navNode = gameObject.GetComponent<NavNode>();
                if (navNode != null)
                    navNodes[i++] = navNode;
            }
            return navNodes;
        }
    }
}
