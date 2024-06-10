using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class LayoutSetting : ScriptableObject
{
    [BoxGroup("SaveSetting")]
    [InfoBox("Normal Path:Assets/Upsytools/Layout")]
    public string SaveName="NormalLayout";
    [BoxGroup("SaveSetting")]
    [Button(ButtonSizes.Large)]
    public void SaveLayoutToPath()
    {
        LayoutUtilityTool.SaveLayout("Assets/Upsytools/Layout/"+SaveName+".wlt");
        AssetDatabase.Refresh();
    }
    
    [BoxGroup("LoadSetting")]
    [Sirenix.OdinInspector.FilePath(ParentFolder = "Assets/Upsytools/Layout",Extensions = "wlt")]
    public string LayoutLoadFile;
    [BoxGroup("LoadSetting")]
    [Button(ButtonSizes.Large)]
    public void LoadLayout()
    {
        //Debug.Log("Assets/Layout"+"/"+LayoutLoadFile);
        LayoutUtilityTool.LoadLayout("Assets/Upsytools/Layout"+"/"+LayoutLoadFile);
    }

    
    public void OnEnable()
    {

        var path = Application.dataPath+"/Upsytools/Layout";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }
    }
}
