using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class UITool : OdinMenuEditorWindow
{
    
    private static Type[] types=new Type[1];
    private static UITool window;
    //[MenuItem("VrEditorTool/UiToolWindow")]
    public static void OpenUiEditor()
    {
        window = GetWindow<UITool>("VrTool");
        window.UseScrollView = true;
        window.Show();
        
        types[0] = typeof(UITool);
        
        EditorWindow uiWindow = GetWindow<UiWindow>( "UI",types );
        uiWindow.Show();
        var s = uiWindow as SceneView;
        s.in2DMode = true;
        
        window.Focus();
    }
    
    
    protected override OdinMenuTree BuildMenuTree()
    {
        var tree =new OdinMenuTree();
        tree.Selection.SupportsMultiSelect = false;
        var ui = CreateInstance<UiAssetData>();
        tree.Add("UI",ui);
        return tree;
    }
}
