using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneAssetData", menuName = "Game/SceneData")]
public class SceneAssetData : SerializedScriptableObject
{
    [HorizontalGroup("1")]
    [FolderPath(ParentFolder = "Assets")]
    public string folderPath = "Upsytools/CustomScene";
    [HorizontalGroup("1")]
    [Button(ButtonSizes.Medium)]
    private void Refresh()
    {
        GetSceneData();
    }

    [ShowInInspector]
    [TableList(ShowIndexLabels = true)]
    public static List<SceneData> SceneDatas;
    public static SceneData CurrentSelectData;
    private static int Count;

    private void OnEnable()
    {
        GetSceneData();
    }

    private void GetSceneData()
    {
        SceneDatas = new List<SceneData>();
        var assetPath = "Assets/" + folderPath;
        var sceneObjects = AssetsHelper.GetSceneObjectsByPath(assetPath);
        for (int i = 0; i < sceneObjects.Length; i++)
        {
            var md = new SceneData()
            {
                Scene = sceneObjects[i].sceneAsset,
                SceneAssetName = sceneObjects[i].sceneName,
                ScenePath = sceneObjects[i].scenePath,
                SceneImage = sceneObjects[i].sceneTexture,
                // IsSelected = false
            };
            SceneDatas.Add(md);
        }

        Count = SceneDatas.Count;
    }

    public static void OnSelectChanged(SceneData data, bool isSelected)
    {
        if (isSelected)
        {
            CurrentSelectData = data;
            global::CurrentSelectData.CurrentSelectScene = data.Scene;
            global::CurrentSelectData.ScenePath = data.ScenePath;
            for (int i = 0; i < Count; i++)
            {
                if (SceneDatas[i] != data)
                {
                    SceneDatas[i].IsSelected = false;
                }
            }
        }
        else
        {
            var hasSelceted = SceneDatas.Any(x => x.IsSelected);
            if (!hasSelceted)
            {
                CurrentSelectData = null;
                global::CurrentSelectData.CurrentSelectScene = null;
                global::CurrentSelectData.ScenePath = "";
            }
        }
    }
    public static void OnResetSelect()
    {
        CurrentSelectData = null;
        global::CurrentSelectData.CurrentSelectScene = null;
        global::CurrentSelectData.ScenePath = "";
        for (int i = 0; i < SceneDatas.Count; i++)
        {
            SceneDatas[i].IsSelected = false;
        }
    }
}

public class SceneData
{

    [OnValueChanged("OnSelectChanged")]
    [TableColumnWidth(100, Resizable = false)]
    public bool IsSelected;
    [TableColumnWidth(80, Resizable = false)]
    [PreviewField(80)]
    [AssetsOnly, Sirenix.OdinInspector.ReadOnly]
    public Texture SceneImage;
    [TableColumnWidth(80, Resizable = false)]
    [PreviewField(80)]
    [AssetsOnly, Sirenix.OdinInspector.ReadOnly]
    [HideInInspector]
    public UnityEditor.SceneAsset Scene;
    [TextArea(2, 10)]
    [LabelWidth(30), Sirenix.OdinInspector.ReadOnly]
    [TableColumnWidth(200, Resizable = false)]
    public string SceneAssetName;
    [TextArea(2, 10)]
    [LabelWidth(30), Sirenix.OdinInspector.ReadOnly]
    [HideInInspector]
    public string ScenePath;

    private void OnSelectChanged()
    {
        SceneAssetData.OnSelectChanged(this, IsSelected);
    }

    [ResponsiveButtonGroup("Actions")]
    [TableColumnWidth(200, Resizable = false)]
    [Button("Preview")]
    public void PreviewAction()
    {
        //Debug.Log(AssetsHelper.CloneAssets(ScenePath));
        var tempScenePath = AssetsHelper.CloneAssets(ScenePath);
        var openScene = EditorSceneManager.OpenScene(tempScenePath, OpenSceneMode.Additive);
        SceneManager.SetActiveScene(openScene);
        var count = SceneManager.sceneCount;
        for (int i = 0; i < count - 1; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            EditorSceneManager.CloseScene(scene, false);
        }
        ModelAssetData.GetCamera();
    }
    [ResponsiveButtonGroup("Actions")]
    [Button("Cancel Preview")]
    public void ClosePreviewAction()
    {
        Debug.Log("关闭预览");
        if (SceneManager.sceneCount > 0)
        {
            var lastScene = SceneManager.GetSceneAt(0);
            var scene = EditorSceneManager.OpenScene(lastScene.path, OpenSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            ModelAssetData.GetCamera();
            EditorCoroutineService.StartCoroutine(WaitForTwoSeconds(() =>
            {
                var previewScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                var path = previewScene.path;
                EditorSceneManager.CloseScene(previewScene, true);
                AssetsHelper.DeleteAsset(path);
            }));
        }
    }
    // [ResponsiveButtonGroup("Actions")]
    // [Button("Apply")]
    // public void ApplyAction()
    // {
    //     Debug.Log("应用");
    //     Debug.Log(Scene);
    // }

    private IEnumerator WaitForTwoSeconds(Action action)
    {
        yield return new WaitForSeconds(0.5f);
        action?.Invoke();
    }
}
