using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace Assets.Editor
{
    public class CancelableProgressBarExample : EditorWindow
    {
        static int secs = 0;
        static double startVal = 0;
        static double progress = 0;


        [MenuItem("Example/Cancelable Progress Bar")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            CancelableProgressBarExample window = (CancelableProgressBarExample)EditorWindow.GetWindow(typeof(CancelableProgressBarExample));
            window.Show();
        }

        void OnGUI()
        {
            if (secs > 0)
            {
                if (GUILayout.Button("Display bar"))
                    startVal = EditorApplication.timeSinceStartup;

                progress = EditorApplication.timeSinceStartup - startVal;

                if (progress < secs)
                {
                    if (EditorUtility.DisplayCancelableProgressBar(
                        "Simple Progress Bar",
                        "Shows a progress bar for the given seconds",
                        (float)(progress / secs)))
                    {
                        Debug.Log("Progress bar canceled by the user");
                        startVal = 0;
                    }
                }
                else
                    EditorUtility.ClearProgressBar();
            }
            else
                secs = EditorGUILayout.IntField("Time to wait:", secs);
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}