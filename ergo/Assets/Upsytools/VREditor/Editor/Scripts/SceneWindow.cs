using UnityEditor;
using UnityEngine;

public class SceneWindow : SceneView
{
    static SceneView myWindow;
    [MenuItem("Window/SceneWindow")]
    static void Init()
    {
        myWindow = (SceneWindow)GetWindow(typeof(SceneWindow), false, "Preview Scene", false);
        myWindow.Show();

    }
}
