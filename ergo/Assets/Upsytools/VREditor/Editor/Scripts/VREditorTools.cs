using System;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VREditorTools : OdinMenuEditorWindow
{

    private static Type[] types=new Type[1];
    private static VREditorTools window;
    public static SceneView sceneWindow,uiWindow;
    [MenuItem("UpsyTools/Asset Hub")]
    private static void OpenWindow()
    {
        window = GetWindow<VREditorTools>("Upsytools");
        window.UseScrollView = true;
        window.Show();
        
        types[0] = typeof(VREditorTools);
        //sceneWindow = GetWindow<SceneWindow>( "PreviewScene",types );
        //sceneWindow.Show();
        
        //uiWindow= GetWindow<UiWindow>( "UIScene",types );
        //uiWindow.in2DMode = true;
        //uiWindow.Show();
        //uiWindow.titleContent=new GUIContent("UIScene");
        // var path = Application.dataPath + "/Upsytools/Layout/NormalLayout.wlt";
        // if (File.Exists(path))
        // {
        //     LayoutUtilityTool.LoadLayout("Assets/Upsytools/Layout/NormalLayout.wlt");
        // }
        window.Focus();
    }
    

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree =new OdinMenuTree();
        tree.Selection.SupportsMultiSelect = false;
        //TestExampleHelper.GetScriptableObject<SceneAssetData>();
        //TestExampleHelper.GetScriptableObject<ModelAssetData>();
        //TestExampleHelper.GetScriptableObject<AnimationAssetData>();
        var scene = CreateInstance<SceneAssetData>(); 
        var human = CreateInstance<ModelAssetData>(); 
        var animation = CreateInstance<AnimationAssetData>();
        var selectData = CreateInstance<CurrentSelectData>();
        //var ui = CreateInstance<UiAssetData>();
        //var layout = CreateInstance<LayoutSetting>();
        // layout.Init();
        //ModelAssetData.CurrentSelectDatas = new System.Collections.Generic.List<ModelAsset>();
        //if (CurrentSelectData.CurrentSelectModels != null)
        //{
        //    CurrentSelectData.CurrentSelectModels = new System.Collections.Generic.List<GameObject>();
        //}
        //AnimationAssetData.CurrentSelectModels = new System.Collections.Generic.List<GameObject>();
        tree.Add("Scene Assets",scene);
        tree.Add("Model & Animation Assets", human);
        //tree.Add("Animation Assets",animation);
        tree.Add("Apply",selectData);
        //tree.Add("UI Assets",ui);
        //tree.Add("Layout Setting", layout);
        //tree.AddAllAssetsAtPath("Test", "Assets/Test/", typeof(ScriptableObject));
        return tree;
    }
}

