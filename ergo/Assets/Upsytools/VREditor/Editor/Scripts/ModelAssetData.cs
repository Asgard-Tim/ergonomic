using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;

[CreateAssetMenu(fileName = "ModelAssetData", menuName = "Game/ModelData")]
public class ModelAssetData : SerializedScriptableObject
{
    [HorizontalGroup("1")]
    [FolderPath(ParentFolder = "Assets")]
    public string folderPath = "Upsytools/CustomModels/HumanModels";
    [HorizontalGroup("1")]
    [Button(ButtonSizes.Medium)]
    private void Refresh()
    {
        GetModelData();
    }
    [ShowInInspector]
    [TableList(ShowIndexLabels = true)]
    public static List<ModelAsset> ModelDatas;
    public static Camera[] cameras;
    public static List<ModelAsset> CurrentSelectDatas = new List<ModelAsset>();
    private static int count;

    #region Animations
    [TitleGroup("FacialAnimation", HorizontalLine = true)]
    //[HorizontalGroup("FacialAnimation/1")]
    //[FolderPath(ParentFolder = "Assets")]
    [HideInInspector] public string facialAnimationFolderPath = "Upsytools/CustomModels/Models/Female_23/Animation/Expression";///FacialModels";///
    public static string facialAnimationFolderPath_F = "Upsytools/CustomModels/Models/";///FacialModels";///
    public static string facialAnimationFolderPath_B = "/Animation/Expression";///FacialModels";///
    //[HorizontalGroup("FacialAnimation/1")]
    //[Button(ButtonSizes.Medium, Name = "Refresh")]
    //private void FacialRefresh()
    //{
    //    //GetFacialAnimationData();
    //}
    [Space(20)]
    [ShowInInspector]
    [TableList(ShowIndexLabels = true, DrawScrollView = true)]
    public static List<AnimationAsset> FacialAnimationClipData;
    [TitleGroup("LimbAnimation", HorizontalLine = true)]
    //[HorizontalGroup("LimbAnimation/2")]
    //[FolderPath(ParentFolder = "Assets")]
    [HideInInspector] public string limbAnimationFolderPath = "Upsytools/CustomModels/Models/Female_23/Animation/Skeleton";//LimbModels";//
    public static string limbAnimationFolderPath_F = "Upsytools/CustomModels/Models/";//LimbModels";//
    public static string limbAnimationFolderPath_B = "/Animation/Skeleton";//LimbModels";//

    //[HorizontalGroup("LimbAnimation/2")]
    //[Button(ButtonSizes.Medium, Name = "Refresh")]
    //private void LimbRefresh()
    //{
    //    //GetLimbAnimationData();
    //}

    [Space(20)]
    [ShowInInspector]
    [TableList(ShowIndexLabels = true)]
    public static List<AnimationAsset> LimbAnimationClipData;

    [Title("AnimationPreviewInfo")]
    [LabelText("CurrentSelectedModel:")]
    [ShowInInspector, Space(10)]
    [PreviewField(60), ReadOnly]
    public static GameObject CurrentSelectModel;

    //[LabelText("CurrentSelectedFacialAnimation:")]
    //[ShowInInspector, Space(10)]
    //[PreviewField(60), ReadOnly]
    //[TableList(ShowIndexLabels = true, DrawScrollView = true)]
    public static List<AnimationClip> CurrentSelectFacialData = new List<AnimationClip>();

    //[LabelText("CurrentSelectedLimbAnimation:")]
    //[ShowInInspector, Space(10)]
    //[PreviewField(60), ReadOnly]
    //[TableList(ShowIndexLabels = true, DrawScrollView = true)]
    public static List<AnimationClip> CurrentSelectLimbData = new List<AnimationClip>();
    [Button(ButtonSizes.Medium)]
    public void PreviewAnimation()
    {
        if ((CurrentSelectModel == null) ||
            ((CurrentSelectFacialData.Count <= 0) &&
            (CurrentSelectLimbData.Count <= 0)))
        {
            Debug.Log("models and animations cannot be empty");
            return;
        }
        ModelAssetData.OnCancelPreview();
        Close();
        
        var controller = AssetDatabase.LoadAssetAtPath("Assets/Upsytools/Mecanim/ModelPreviewController.controller", typeof(AnimatorController)) as AnimatorController;
        //var controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Upsytools/Mecanim/test.controller");
        var idle = SetAnimatorController(controller, false);
        var previewParent = ModelAssetData.OnPreview(CurrentSelectModel);
        EditorCoroutineService.StartCoroutine(WaitForTwoSeconds(() =>
        {
            var child = previewParent.transform.GetChild(0).gameObject;
            Selection.activeObject = child;
            if (previewParent != null)
            {
                Animator animator = child.GetComponent<Animator>();
                animator.runtimeAnimatorController = controller;
                AnimatorBake(animator, idle, CurrentSelectLimbData[0]);
            }
        }));
    }
    public static AnimatorState SetAnimatorController(AnimatorController controller, bool isPlay)
    {
        var par = controller.parameters.First(x => x.name == "Play");
        par.defaultBool = false;
        controller.parameters = new[] { par };
        AnimatorCondition ac = new AnimatorCondition { mode = AnimatorConditionMode.If, parameter = "Play" };

        var limbLayer = controller.layers[0];
        limbLayer.blendingMode = AnimatorLayerBlendingMode.Additive;
        limbLayer.defaultWeight = 1.0f;
        var limb = limbLayer.stateMachine.defaultState;
        if(limb == null)
        {
            limb = limbLayer.stateMachine.AddState(CurrentSelectLimbData[0].name);
            limbLayer.stateMachine.defaultState = limb;
        }
        var limbSt = limbLayer.stateMachine.AddEntryTransition(limb);
        limbLayer.stateMachine.defaultState.motion = CurrentSelectLimbData[0];
        limbSt.conditions = new[] { ac };

        var facialLayer = controller.layers[1];
        facialLayer.blendingMode = AnimatorLayerBlendingMode.Override;
        facialLayer.defaultWeight = 1.0f;
        var facial = facialLayer.stateMachine.defaultState;
        if (facial == null)
        {
            facial = facialLayer.stateMachine.AddState(CurrentSelectFacialData[0].name);
            facialLayer.stateMachine.defaultState = facial;
        }
        var facialSt = facialLayer.stateMachine.AddEntryTransition(facial);
        facialLayer.stateMachine.defaultState.motion = CurrentSelectFacialData[0];
        facialSt.conditions = new[] { ac };

        return limb;

        //controller.layers[0].name = "Limb";
        //controller.AddLayer("Facial");
        //controller.AddParameter("Play", AnimatorControllerParameterType.Bool);
        //var par = controller.parameters.First(x => x.name == "Play");
        //par.defaultBool = isPlay;
        //controller.parameters = new[] { par };
        //AnimatorCondition ac = new AnimatorCondition { mode = AnimatorConditionMode.If, parameter = "Play" };

        ////set the blending method
        //var limbLayer = controller.layers[0];
        //limbLayer.blendingMode = AnimatorLayerBlendingMode.Additive;
        //limbLayer.defaultWeight = 1.0f;
        ////var limbIdle = limbLayer.stateMachine.AddState("limb");
        ////limbLayer.stateMachine.defaultState = limbIdle;
        ////add other status
        //var limb = limbLayer.stateMachine.AddState(CurrentSelectLimbData.name);
        //limb.motion = CurrentSelectLimbData;
        //// connect the newly created state with any state，and set the conversion parameters
        //var limbSt = limbLayer.stateMachine.AddEntryTransition(limb);
        //limbLayer.stateMachine.defaultState = limb;
        ////set conversion parameters
        //limbSt.conditions = new[] { ac };


        ////set default state
        //var baseLayer = controller.layers[1];
        //baseLayer.blendingMode = AnimatorLayerBlendingMode.Additive;
        //baseLayer.defaultWeight = 1.0f;
        ////var idle = baseLayer.stateMachine.AddState("Idle");
        ////baseLayer.stateMachine.defaultState = idle;
        ////add other status
        //var facial = baseLayer.stateMachine.AddState(CurrentSelectFacialData.name);
        //facial.motion = CurrentSelectFacialData;
        //// connect the newly created state with any state，and set the conversion parameters
        //var facialst = baseLayer.stateMachine.AddEntryTransition(facial);
        //baseLayer.stateMachine.defaultState = facial;
        ////set conversion parameters
        //facialst.conditions = new[] { ac };

        //controller.layers = new[] { limbLayer, baseLayer };
        //return limb;
    }

    [Button(ButtonSizes.Medium)]
    public void CancelPreview()
    {
        playtime = false;
        Close();
        ModelAssetData.OnCancelPreview();
    }

    private IEnumerator WaitForTwoSeconds(Action action)
    {
        yield return new WaitForSeconds(1f);
        action?.Invoke();
    }
    #endregion

    #region Variables used in playing default animations
    //[Title("AnimationClipIdle")]
    //[LabelText("AnimationClipIdle:")]
    //[ShowInInspector, Space(10)]
    //[PreviewField(60), ReadOnly]
    public static AnimationClip AnimationClipIdle;
    //[Title("AnimationClipNeutral")]
    //[LabelText("AnimationClipNeutral:")]
    //[ShowInInspector, Space(10)]
    //[PreviewField(60), ReadOnly]
    public static AnimationClip AnimationClipNeutral;
    //[Title("Animator")]
    //[LabelText("Animator:")]
    //[ShowInInspector, Space(10)]
    //[PreviewField(60), ReadOnly]
    public static Animator mAnimator;
    //[Title("PlayTime")]
    //[LabelText("PlayTime:")]
    //[ShowInInspector, Space(10)]
    public static bool playtime = false;
    //[Title("AnimationBake")]
    //[LabelText("AnimationBake:")]
    //[ShowInInspector, Space(10)]
    public static bool animationBake = false;
    /// Delta of time
    private static double delta;
    /// Current running time
    private static float m_RunningTime;
    /// Previous system time
    private static double m_PreviousTime;
    /// Longest animation time
    private static float aniTime = 0.0f;
    private static float RecordEndTime = 0;
    //private bool isInspectorUpdate = false;
    #endregion

    private void OnEnable()
    {
        GetModelData();
        //GetFacialAnimationData();
        //GetLimbAnimationData();
        GetAnimationData();
        EditorApplicationUpdate();
    }
    private void OnDestroy()
    {
        EditorApplication.update -= inspectorUpdate;
        //isInspectorUpdate = false;
        Close();
    }
    public static void GetFacialAnimationData(string modelname = "Female_23")
    {
        FacialAnimationClipData = new List<AnimationAsset>();
        var assetPath = "Assets/" + facialAnimationFolderPath_F + modelname + facialAnimationFolderPath_B;
        var models = AssetsHelper.GetAnimationAssets(assetPath);
        for (int i = 0; i < models.Length; i++)
        {
            var md = new AnimationAsset()
            {
                AnimationClip = models[i].clip,
                AnimationClipName = models[i].clipName,
                Model = models[i].clipGo
            };
            FacialAnimationClipData.Add(md);
        }
    }
    public static void GetLimbAnimationData(string modelname = "Female_23")
    {
        LimbAnimationClipData = new List<AnimationAsset>();
        //var assetPath = "Assets/" + limbAnimationFolderPath;
        var assetPath = "Assets/" + limbAnimationFolderPath_F + modelname + limbAnimationFolderPath_B;
        var models = AssetsHelper.GetAnimationAssets(assetPath);
        for (int i = 0; i < models.Length; i++)
        {
            var md = new AnimationAsset()
            {
                AnimationClip = models[i].clip,
                AnimationClipName = models[i].clipName,
                Model = models[i].clipGo
            };
            LimbAnimationClipData.Add(md);
        }
    }
    void EditorApplicationUpdate()
    {
        m_PreviousTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += inspectorUpdate;
        //isInspectorUpdate = true;
    }
    private void GetModelData()
    {
        ModelDatas = new List<ModelAsset>();
        var assetPath = "Assets/" + folderPath;
        var modelObjects = AssetsHelper.GetModelObjectsByPath(assetPath);
        for (int i = 0; i < modelObjects.Length; i++)
        {
            var md = new ModelAsset()
            {
                Model = modelObjects[i],
                ModelName = modelObjects[i].name
            };
            ModelDatas.Add(md);
        }
        count = ModelDatas.Count;
    }
    public static GameObject OnPreview(GameObject modelGo,bool needani = false)
    {
        GetCamera();
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] != null)
            { 
                cameras[i].gameObject.SetActive(false);
            }
        }
        var previewParent = new GameObject("PreviewModel");
        var model = Instantiate(modelGo, previewParent.transform, true);
        //var previewCamera = new GameObject("PreviewCamera");
        //previewCamera.AddComponent<Camera>();
        //previewCamera.transform.SetParent(previewParent.transform);
        //previewCamera.transform.localPosition = new Vector3(model.transform.position.x, model.transform.position.y, model.transform.position.z - 10);
        //model.transform.forward = -previewCamera.transform.forward;
        SceneView.lastActiveSceneView.LookAt(model.transform.position);
        return previewParent;
    }
    public static void OnCancelPreview()
    {
        var previewParent = GameObject.Find("PreviewModel");
        while(previewParent != null)
        {
            DestroyImmediate(previewParent);
            previewParent = GameObject.Find("PreviewModel");
        }
        if (cameras == null) return;
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] != null) cameras[i].gameObject.SetActive(true);
        }
    }
    public static void GetCamera()
    {
        var goes = EditorSceneManager.GetActiveScene().GetRootGameObjects();
        var cameraList = new List<Camera>();
        for (int i = 0; i < goes.Length; i++)
        {
            var ca = goes[i].transform.GetComponent<Camera>();
            if (ca)
            {
                cameraList.Add(ca);
            }
        }
        cameras = cameraList.ToArray();
    }
    public static void OnSelectChanged(ModelAsset data, bool isSelected)
    {
        if (isSelected)
        {
            if (!ModelAssetData.CurrentSelectDatas.Contains(data))
            {
                ModelAssetData.CurrentSelectDatas.Add(data);
            }
            AnimationAssetData.CurrentSelectModel = data.Model;
            ModelAssetData.CurrentSelectModel = data.Model;
            if (!CurrentSelectData.CurrentSelectModels.Contains(data.Model))
            {
                CurrentSelectData.CurrentSelectModels.Add(data.Model);
            }
            //CurrentSelectData.CurrentSelectModel = data.Model;
            //for (int i = 0; i < count; i++)
            //{
            //    if (ModelDatas[i] != data)
            //    {
            //        ModelDatas[i].IsSelected = false;
            //    }
            //}

            ModelAssetData.GetFacialAnimationData(data.ModelName);
            ModelAssetData.GetLimbAnimationData(data.ModelName);
        }
        else
        {
            if (ModelAssetData.CurrentSelectDatas.Contains(data))
            {
                ModelAssetData.CurrentSelectDatas.Remove(data);
            }
            if (CurrentSelectData.CurrentSelectModels.Contains(data.Model))
            {
                CurrentSelectData.CurrentSelectModels.Remove(data.Model);
            }
            var hasSelceted = ModelDatas.Any(x => x.IsSelected);
            if (!hasSelceted)
            {
                ModelAssetData.CurrentSelectDatas.Clear();
                ModelAssetData.CurrentSelectModel = null;
                AnimationAssetData.CurrentSelectModel = null;
                CurrentSelectData.CurrentSelectModels.Clear();
            }
            LimbAnimationClipData.Clear();
            FacialAnimationClipData.Clear();
        }
        CurrentSelectFacialData.Clear();
        CurrentSelectLimbData.Clear();
    }

    public static void OnAnimationSelectChanged(AnimationAsset data, bool isSelected)
    {
        if (isSelected)
        {
            if (FacialAnimationClipData.Contains(data))
            {
                if (!ModelAssetData.CurrentSelectFacialData.Contains(data.AnimationClip))
                {
                    ModelAssetData.CurrentSelectFacialData.Add(data.AnimationClip);
                }
                CurrentSelectData.CurrentSelectFacialData = data.AnimationClip;
                //FacialAnimationClipData.ForEach(x => { if (x != data) { x.IsSelected = false; } });
            }
            else if (LimbAnimationClipData.Contains(data))
            {
                if (!ModelAssetData.CurrentSelectLimbData.Contains(data.AnimationClip))
                {
                    ModelAssetData.CurrentSelectLimbData.Add(data.AnimationClip);
                }
                CurrentSelectData.CurrentSelectLimbData = data.AnimationClip;
                //LimbAnimationClipData.ForEach(x => { if (x != data) { x.IsSelected = false; } });
            }
        }
        else
        {
            if (FacialAnimationClipData.Contains(data))
            {
                if (ModelAssetData.CurrentSelectFacialData.Contains(data.AnimationClip))
                {
                    ModelAssetData.CurrentSelectFacialData.Remove(data.AnimationClip);
                }
                CurrentSelectData.CurrentSelectFacialData = null;
            }
            else if(LimbAnimationClipData.Contains(data))
            {
                if (ModelAssetData.CurrentSelectLimbData.Contains(data.AnimationClip))
                {
                    ModelAssetData.CurrentSelectLimbData.Remove(data.AnimationClip);
                }
                CurrentSelectData.CurrentSelectLimbData = null;
            }
        }
    }

    public static void OnResetSelect()
    {
        ModelAssetData.CurrentSelectDatas.Clear();
        ModelAssetData.CurrentSelectModel = null;
        AnimationAssetData.CurrentSelectModel = null;
        CurrentSelectData.CurrentSelectModels.Clear();
        for (int i = 0; i < ModelDatas.Count; i++)
        {
            ModelDatas[i].IsSelected = false;
        }
    }

    #region Methods used in playing default animations
    void GetAnimationData()
    {
        m_PreviousTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += inspectorUpdate;
        //isInspectorUpdate = true;
    }
    public static void AnimatorBake(Animator animator, AnimatorState state, AnimationClip clip)
    {
        if (Application.isPlaying)
        {
            return;
        }
        //AnimationClipIdle = clip;
        mAnimator = animator;
        aniTime = 0.0f;
        aniTime = clip.length;
        float frameRate = 30f;
        int frameCount = (int)((aniTime * frameRate) + 2);
        animator.Rebind();
        animator.StopPlayback();
        animator.recorderStartTime = 0;

        animator.StartRecording(frameCount);

        for (var j = 0; j < frameCount - 1; j++)
        {
            if (j == 0)
            {
                animator.SetBool("Play", true);
            }
            animator.Update(1.0f / frameRate);
        }

        animator.StopRecording();
        RecordEndTime = animator.recorderStopTime;
        animator.StartPlayback();

        animationBake = true;
        playtime = true;
    }
    public static void Close()
    {
        if (mAnimator != null)
        {
            playtime = false;
            m_RunningTime = 0;
            mAnimator.playbackTime = m_RunningTime;
            mAnimator.Update(0);
        }
    }
    private void inspectorUpdate()
    {
        delta = EditorApplication.timeSinceStartup - m_PreviousTime;
        m_PreviousTime = EditorApplication.timeSinceStartup;

        if (!Application.isPlaying)
        {
            m_RunningTime = m_RunningTime + (float)delta;
            update();
        }
    }
    void update()
    {
        if (Application.isPlaying)
        {
            return;
        }
        if (mAnimator != null && mAnimator.runtimeAnimatorController == null)
        {
            return;
        }
        if (!animationBake)
        {
            return;
        }
        if (playtime)
        {
            if (m_RunningTime <= aniTime)
            {
                if (mAnimator != null)
                {
                    mAnimator.playbackTime = m_RunningTime;
                    mAnimator.Update(0);
                }
            }
            if (m_RunningTime >= aniTime)
            {
                m_RunningTime = 0.0f;
            }
        }
    }
    #endregion
}


public class ModelAsset
{
    [TableColumnWidth(100, Resizable = false)]
    [OnValueChanged("OnSelectChanged")]
    public bool IsSelected;
    [TableColumnWidth(80, Resizable = false)]
    [PreviewField(80)]
    [AssetsOnly, ReadOnly]
    public GameObject Model;
    [TextArea(2, 10)]
    [LabelWidth(50), ReadOnly]
    [TableColumnWidth(200, Resizable = false)]
    public string ModelName;

    [ResponsiveButtonGroup("Actions")]
    [TableColumnWidth(300, Resizable = false)]
    [Button("Preview")]
    public void PreviewAction()
    {
        ModelAssetData.OnPreview(Model);

        var controller = AssetDatabase.LoadAssetAtPath("Assets/Upsytools/CustomModels/HumanModels/" + ModelName +
            "/ModelPreviewController.controller", typeof(AnimatorController)) as AnimatorController;
        //var controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Upsytools/Mecanim/ModelPreviewControllerTest.controller");
        //controller.AddLayer("Limb");

        //controller.AddParameter("Play", AnimatorControllerParameterType.Bool);
        var par = controller.parameters.First(x => x.name == "Play");
        par.defaultBool = false;
        controller.parameters = new[] { par };
        AnimatorCondition ac = new AnimatorCondition { mode = AnimatorConditionMode.If, parameter = "Play" };

        var AnimationClipIdle = AssetDatabase.LoadAssetAtPath("Assets/Upsytools/Mecanim/idle.anim", typeof(AnimationClip)) as AnimationClip;
        var AvatarMaskIdle = AssetDatabase.LoadAssetAtPath("Assets/Upsytools/Mecanim/idle.mask", typeof(AvatarMask)) as AvatarMask;;

        var AnimationClipNeutral = AssetDatabase.LoadAssetAtPath("Assets/Upsytools/Mecanim/neutral.anim", typeof(AnimationClip)) as AnimationClip;
        var AvatarMaskNeutral = AssetDatabase.LoadAssetAtPath("Assets/Upsytools/Mecanim/neutral.mask", typeof(AvatarMask)) as AvatarMask;

        ////set the blending method
        //var limbLayer = controller.layers[0];
        //limbLayer.blendingMode = AnimatorLayerBlendingMode.Additive;
        //limbLayer.defaultWeight = 1.0f;

        ////set default state
        //var facialLayer = controller.layers[1];
        //facialLayer.blendingMode = AnimatorLayerBlendingMode.Additive;
        //facialLayer.defaultWeight = 1.0f;

        //var limb = limbLayer.stateMachine.AddState(AnimationClipIdle.name);
        //limb.motion = AnimationClipIdle;
        ////connect the newly created state with any state，and set the conversion parameters
        //var limbSt = limbLayer.stateMachine.AddEntryTransition(limb);
        //limbLayer.stateMachine.defaultState = limb;
        //limbSt.conditions = new[] { ac };

        //var facial = facialLayer.stateMachine.AddState(AnimationClipNeutral.name);
        //facial.motion = AnimationClipNeutral;
        ////connect the newly created state with any state，and set the conversion parameters
        //var facialSt = facialLayer.stateMachine.AddEntryTransition(facial);
        //facialLayer.stateMachine.defaultState = facial;
        //facialSt.conditions = new[] { ac };

        EditorCoroutineService.StartCoroutine(WaitForTwoSeconds(() =>
        {
            var previewParent = GameObject.Find("PreviewModel");
            var child = previewParent.transform.GetChild(0).gameObject;
            Selection.activeObject = child;
            if (previewParent != null)
            {
                Animator animator = child.GetComponent<Animator>();
                animator.runtimeAnimatorController = controller;
                ModelAssetData.AnimatorBake(animator, null, AnimationClipIdle);
            }
        }));
    }

    private static IEnumerator WaitForTwoSeconds(Action action)
    {
        yield return new WaitForSeconds(1f);
        action?.Invoke();
    }
    [ResponsiveButtonGroup("Actions")]
    [Button("Cancel Preview")]
    public void ClosePreview()
    {
        //Debug.Log("关闭预览");
        // var previewParent= GameObject.Find("预览模型");
        // if(previewParent!=null) GameObject.DestroyImmediate(previewParent);
        // if (ModelAssetData.cameras == null) return;
        // for (int i = 0; i < ModelAssetData.cameras.Length; i++)
        // {
        //     if(ModelAssetData.cameras[i]!=null) ModelAssetData.cameras[i].gameObject.SetActive(true);
        // }
        ModelAssetData.playtime = false;
        ModelAssetData.Close();
        ModelAssetData.OnCancelPreview();
    }
    // [Button("Apply"),ResponsiveButtonGroup("Actions")]
    // public void ApplyAction()
    // {
    //     Debug.Log("应用");
    //     Debug.Log(Model);
    // }

    private void OnSelectChanged()
    {
        ModelAssetData.OnSelectChanged(this, IsSelected);
    }
}