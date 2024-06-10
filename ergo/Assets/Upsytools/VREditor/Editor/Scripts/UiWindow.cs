using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
// [EditorWindowTitle(title = "Scene", useTypeNameAsIconName = true)]
public class UiWindow : SceneView
{
    static SceneView myWindow;
    // [MenuItem("Window/UiWindow")]
    public static void Init()
    {
        myWindow = (UiWindow)GetWindow(typeof(UiWindow), false, "UIScene", false);
        myWindow.in2DMode = true;
        myWindow.Show();
    }
}
