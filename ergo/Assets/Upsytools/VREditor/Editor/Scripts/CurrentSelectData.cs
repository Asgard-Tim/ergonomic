using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CurrentSelectData : SerializedScriptableObject
{
    [Title("Scene")]
    [ShowInInspector, Space(10)]
    [PreviewField(60), ReadOnly, LabelText("CurrentSelectedScene:")]
    public static SceneAsset CurrentSelectScene;
    public static string ScenePath;

    [Title("Model")]
    [LabelText("CurrentSelectedModel:")]
    [ShowInInspector, Space(10)]
    [PreviewField(60), ReadOnly]
    public static List<GameObject> CurrentSelectModels = new List<GameObject>();

    //[Title("AnimationClip")]
    //[LabelText("CurrentSelectedFacialAnimation:")]
    //[ShowInInspector, Space(10)]
    //[PreviewField(60), ReadOnly]
    public static AnimationClip CurrentSelectFacialData;

    //[Title("AnimationClip")]
    //[LabelText("CurrentSelectedLimbAnimation:")]
    //[ShowInInspector, Space(10)]
    //[PreviewField(60), ReadOnly]
    public static AnimationClip CurrentSelectLimbData;

    //[Title("CurrentCreateModel")]
    //[LabelText("CurrentCreateModel:")]
    //[ShowInInspector]
    //[TableList(ShowIndexLabels = true)]
    public static List<ScenePreviewModelAsset> ScenePreviewModelDatas;
    public static List<ScenePreviewModelAsset> _tscenePreviewModelAsset = new List<ScenePreviewModelAsset>();

    [Button("Apply", ButtonSizes.Medium)]
    public void ApplyButton()
    {
        var tempScenePath = "";
        var scene = OpenScene(out tempScenePath);
        if (CurrentSelectModels.Count > 0)
        {
            ScenePreviewModelDatas = new List<ScenePreviewModelAsset>();
            for (int i = 0; i < CurrentSelectModels.Count; i++)
            {
                ScenePreviewModelAsset _tScenePreviewModelAsset = new ScenePreviewModelAsset();
                var model = AddModel(CurrentSelectModels[i]);
                AddAnimation(model);
                _tScenePreviewModelAsset.Model = model;
                ScenePreviewModelDatas.Add(_tScenePreviewModelAsset);
                PreviewAnimation(ref model, CurrentSelectModels[i].name);
            }
        }
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, tempScenePath); ;
    }
    public void PreviewAnimation(ref GameObject m,string n)
    {
        if ((ModelAssetData.CurrentSelectFacialData.Count <= 0) &&
            (ModelAssetData.CurrentSelectLimbData.Count <= 0))
        {
            //Debug.Log("models and animations cannot be empty");
            return;
        }
        //var controller = AssetDatabase.LoadAssetAtPath("Assets/Upsytools/Mecanim/ModelPreviewController.controller", typeof(AnimatorController)) as AnimatorController;
        var controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Upsytools/Mecanim/"+ n + System.DateTime.Now.ToString("yyyymmddhhmmss") + ".controller");
        var idle = SetAnimatorController(controller, false);
        Animator animator = m.GetComponent<Animator>();
        animator.runtimeAnimatorController = controller;
    }
    public static AnimatorState SetAnimatorController(AnimatorController controller, bool isPlay)
    {

        controller.AddLayer("limb");
        var limbLayer = controller.layers[1];
        limbLayer.blendingMode = AnimatorLayerBlendingMode.Additive;
        limbLayer.defaultWeight = 1.0f;
        var limb = limbLayer.stateMachine.defaultState;
        if (ModelAssetData.CurrentSelectLimbData.Count >= 1)
        {
            if (limb == null)
            {
                limb = limbLayer.stateMachine.AddState(ModelAssetData.CurrentSelectLimbData[0].name);
                limbLayer.stateMachine.defaultState = limb;
            }
            var limbSt = limbLayer.stateMachine.AddEntryTransition(limb);
            limbLayer.stateMachine.defaultState.motion = ModelAssetData.CurrentSelectLimbData[0];
            for (int i = 1; i < ModelAssetData.CurrentSelectLimbData.Count; i++)
            {
                var _limbst = limbLayer.stateMachine.AddState(ModelAssetData.CurrentSelectLimbData[1].name);
                _limbst.motion = ModelAssetData.CurrentSelectLimbData[i];
            }
        }



        controller.AddLayer("facial");
        var facialLayer = controller.layers[2];
        facialLayer.blendingMode = AnimatorLayerBlendingMode.Override;
        facialLayer.defaultWeight = 1.0f;
        var facial = facialLayer.stateMachine.defaultState;
        if (ModelAssetData.CurrentSelectFacialData.Count >= 1)
        {
            if (facial == null)
            {
                facial = facialLayer.stateMachine.AddState(ModelAssetData.CurrentSelectFacialData[0].name);
                facialLayer.stateMachine.defaultState = facial;
            }
            var facialSt = facialLayer.stateMachine.AddEntryTransition(facial);
            facialLayer.stateMachine.defaultState.motion = ModelAssetData.CurrentSelectFacialData[0];
            for (int i = 1; i < ModelAssetData.CurrentSelectFacialData.Count; i++)
            {
                var _limbst = facialLayer.stateMachine.AddState(ModelAssetData.CurrentSelectFacialData[1].name);
                _limbst.motion = ModelAssetData.CurrentSelectFacialData[i];
            }
        }

        return null;
    }

    [Button("Reset", ButtonSizes.Medium)]
    public void ResetButton()
    {
        SceneAssetData.OnResetSelect();
        ModelAssetData.OnResetSelect();
        AnimationAssetData.OnResetSelect();
        CurrentSelectScene = null;
        ScenePath = "";
        CurrentSelectModels.Clear();
        CurrentSelectFacialData = null;
        CurrentSelectLimbData = null;
        if (ScenePreviewModelDatas.Count > 0)
        {
            for (int i = 0; i < ScenePreviewModelDatas.Count; i++)
            {
                if (ScenePreviewModelDatas[i].Model != null)
                {
                    DestroyImmediate(ScenePreviewModelDatas[i].Model);
                }
            }
            ScenePreviewModelDatas.Clear();
        }
    }
    private Scene OpenScene(out string tempScenePath)
    {
        tempScenePath = "";
        if (string.IsNullOrEmpty(ScenePath)) return default;
        tempScenePath = CloneAssets(ScenePath);
        var openScene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        SceneManager.SetActiveScene(openScene);
        //var count = SceneManager.sceneCount;
        //for (int i = 0; i < count - 1; i++)
        //{
        //    var scene = SceneManager.GetSceneAt(i);
        //    EditorSceneManager.CloseScene(scene, true);
        //}
        return openScene;
    }

    private GameObject AddModel(GameObject currentSelectModel)
    {
        if (currentSelectModel == null) return null;
        var model = Instantiate(currentSelectModel);
        var camera = FindObjectOfType<Camera>();
        //model.transform.localPosition = new Vector3(model.transform.position.x, model.transform.position.y, model.transform.position.z - 10);
        //if (camera != null) model.transform.forward = -camera.transform.forward;
        SceneView.lastActiveSceneView.LookAt(model.transform.position);
        return model;
    }

    private void AddAnimation(GameObject model)
    {
        //if (model == null || CurrentSelectFacialData == null || CurrentSelectLimbData == null)
        if (model == null)
        {
            Debug.Log("models and animations cannot be empty");
            return;
        }
        var animatorName = model.name + "AnimatorController";
        //var controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Upsytools/Apply/" + animatorName + ".controller");
        //ModelAssetData.SetAnimatorController(controller, false);
        model.GetComponent<Animator>().runtimeAnimatorController = null;
        //model.GetComponent<Animator>().runtimeAnimatorController = controller;
    }

    public string CloneAssets(string assetPath)
    {
        var str = assetPath.Split('/');
        var fullName = str[str.Length - 1];
        var assetName = fullName.Split('.')[0];
        var suffix = fullName.Split('.')[1];
        var newPath = "Assets/" + assetName + "_temp_" + System.DateTime.Now.ToFileTime() + "." + suffix;
        return newPath;
    }

    public static void CloseSingleModelAsset(ScenePreviewModelAsset scenePreviewModelAsset)
    {
        if (scenePreviewModelAsset != null)
        {
            if (scenePreviewModelAsset.Model != null)
            {
                DestroyImmediate(scenePreviewModelAsset.Model);
            }
        }
        //_tscenePreviewModelAsset.Clear();
        //for (int i = 0; i < ScenePreviewModelDatas.Count; i++)
        //{
        //    if (!ScenePreviewModelDatas.Contains(scenePreviewModelAsset))
        //    {
        //        _tscenePreviewModelAsset.Add(ScenePreviewModelDatas[i]);
        //    }
        //}
        EditorCoroutineService.StartCoroutine(WaitForTwoSeconds(() =>
        {

            //ScenePreviewModelDatas = new List<ScenePreviewModelAsset>();
            for (int i = ScenePreviewModelDatas.Count - 1; i >= 0; i--)
            {
                if(ScenePreviewModelDatas[i].Model == null)
                {
                    ScenePreviewModelDatas.Remove(ScenePreviewModelDatas[i]);
                }
            }
            //ScenePreviewModelDatas = _tscenePreviewModelAsset;
        }));
    }
    private static IEnumerator WaitForTwoSeconds(System.Action action)
    {
        yield return new WaitForSeconds(0.1f);
        action?.Invoke();
    }
}
public class ScenePreviewModelAsset
{
    [TableColumnWidth(80, Resizable = false)]
    [PreviewField(80)]
    [ReadOnly]
    public GameObject Model;

    [ResponsiveButtonGroup("Actions")]
    //[TableColumnWidth(200, Resizable = false)]
    [Button("Close Model")]
    public void CloseModelAction()
    {
        CurrentSelectData.CloseSingleModelAsset(this);
    }
}
