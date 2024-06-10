using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AnimationAssetData : SerializedScriptableObject
{
    [TitleGroup("FacialAnimation",HorizontalLine = true)]
    [HorizontalGroup("FacialAnimation/1")]
    [FolderPath(ParentFolder = "Assets")]
    public string facialAnimationFolderPath= "Upsytools/CustomModels/Models/Female_23/Animation/Expression";///FacialModels";///
    [HorizontalGroup("FacialAnimation/1")]
    [Button(ButtonSizes.Medium,Name = "Refresh")]
    private void FacialRefresh()
    {
        GetFacialAnimationData();
    }
    [Space(20)]
    [ShowInInspector]
    [TableList(ShowIndexLabels = true,DrawScrollView = true)]
    public static List<AnimationAsset> FacialAnimationClipData;
    [TitleGroup("LimbAnimation", HorizontalLine = true)]
    [HorizontalGroup("LimbAnimation/2")]
    [FolderPath(ParentFolder = "Assets")]
    public string limbAnimationFolderPath = "Upsytools/CustomModels/Models/Female_23/Animation/Skeleton";//LimbModels";//

    [HorizontalGroup("LimbAnimation/2")]
    [Button(ButtonSizes.Medium,Name = "Refresh")]
    private void LimbRefresh()
    {
        GetLimbAnimationData();
    }

    [Space(20)]
    [ShowInInspector]
    [TableList(ShowIndexLabels = true)]
    public static List<AnimationAsset> LimbAnimationClipData;

    [Title("Model")]
    [LabelText("CurrentSelectedModel:")]
    [ShowInInspector, Space(10)]
    [PreviewField(60), ReadOnly]
    public static GameObject CurrentSelectModel;

    [LabelText("CurrentSelectedFacialAnimation:")] 
    [ShowInInspector, Space(10)] 
    [PreviewField(60), ReadOnly]
    public static AnimationClip CurrentSelectFacialData;

    [LabelText("CurrentSelectedLimbAnimation:")] 
    [ShowInInspector, Space(10)] 
    [PreviewField(60), ReadOnly]
    public static AnimationClip CurrentSelectLimbData;

    [Button(ButtonSizes.Medium)]
    public void PreviewAnimation()
    {
        if (CurrentSelectModel == null || (CurrentSelectFacialData == null && CurrentSelectLimbData == null))
        {
            Debug.Log("models and animations cannot be empty");
            return;
        }
        ModelAssetData.OnCancelPreview();
        Close();

        var controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Upsytools/Mecanim/test.controller");
        var idle = SetAnimatorController(controller, false);
        var previewParent = ModelAssetData.OnPreview(CurrentSelectModel);
        EditorCoroutineService.StartCoroutine(WaitForTwoSeconds(() =>
        {
            Debug.Log(previewParent);
            var child = previewParent.transform.GetChild(0).gameObject;
            Selection.activeObject = child;
            if (previewParent != null)
            {
                Animator animator = child.GetComponent<Animator>();
                animator.runtimeAnimatorController = controller;
                AnimatorBake(animator, idle, CurrentSelectLimbData);
            }
        }));
    }

    public static AnimatorState SetAnimatorController(AnimatorController controller,bool isPlay)
    {
        controller.AddLayer("Limb");
        controller.AddParameter("Play", AnimatorControllerParameterType.Bool);
        var par = controller.parameters.First(x => x.name == "Play");
        par.defaultBool = isPlay;
        controller.parameters = new[] {par};
        AnimatorCondition ac = new AnimatorCondition { mode = AnimatorConditionMode.If, parameter = "Play" };

        //set the blending method
        var limbLayer = controller.layers[0];
        limbLayer.blendingMode = AnimatorLayerBlendingMode.Additive;
        limbLayer.defaultWeight = 1.0f;

        var limbIdle = limbLayer.stateMachine.AddState("Idle");
        limbLayer.stateMachine.defaultState = limbIdle;

        var limb = limbLayer.stateMachine.AddState(CurrentSelectLimbData.name);
        limb.motion = CurrentSelectLimbData;
        //var limbSt = limbLayer.stateMachine.AddAnyStateTransition(limb);
        var limbSt = limbLayer.stateMachine.AddEntryTransition(limb);
        limbLayer.stateMachine.defaultState = limb;
        //limbSt.canTransitionToSelf = false;
        //limbSt.hasExitTime = true;
        //limbSt.exitTime = 0;
        //limbSt.hasFixedDuration = false;
        //limbSt.duration = 0;
        //limbSt.offset = 0;

        //set conversion parameters
        limbSt.conditions = new[] { ac };


        //set default state
        var baseLayer = controller.layers[1];
        baseLayer.blendingMode = AnimatorLayerBlendingMode.Additive;
        baseLayer.defaultWeight = 1.0f;
        var idle = baseLayer.stateMachine.AddState("Idle");
        baseLayer.stateMachine.defaultState = idle;
        //add other status
        var facial = baseLayer.stateMachine.AddState(CurrentSelectFacialData.name);
        facial.motion = CurrentSelectFacialData;
        // connect the newly created state with any state，and set the conversion parameters
        //var st = baseLayer.stateMachine.AddAnyStateTransition(facial);
        var st = baseLayer.stateMachine.AddEntryTransition(facial);
        baseLayer.stateMachine.defaultState = facial;
        //st.canTransitionToSelf = false;
        //st.hasExitTime = true;
        //st.exitTime = 0;
        //st.hasFixedDuration = false;
        //st.duration = 0;
        //st.offset = 0;

        //set conversion parameters
        st.conditions = new[] { ac };

        controller.layers = new[] { limbLayer ,baseLayer };
        return limb;
    }

    [Button(ButtonSizes.Medium)]
    public void CancelPreview()
    {
        AnimationAssetData.playtime = false;
        AnimationAssetData.Close();
        ModelAssetData.OnCancelPreview();
    }
    
    private IEnumerator WaitForTwoSeconds( Action action)
    { 
        yield return new WaitForSeconds(1f);
        action?.Invoke();
    }
    //private static int count;
    public static Animator mAnimator;
    public static bool playtime = false;
    private static bool hongPei = false;

    /// <summary>
    /// Delta time
    /// </summary>
    private static double delta;

    /// <summary>
    /// Current running time
    /// </summary>
    private static float m_RunningTime;

    /// <summary>
    /// Last system time
    /// </summary>
    private static double m_PreviousTime;

    /// <summary>
    /// Longest animation time
    /// </summary>
    private static float aniTime = 0.0f;

    private static float RecordEndTime = 0;

    //private bool isInspectorUpdate = false;
    private void OnEnable()
    {
        GetFacialAnimationData();
        GetLimbAnimationData();
    }

    void GetFacialAnimationData()
    {
        FacialAnimationClipData=new List<AnimationAsset>();
        var assetPath = "Assets/" + facialAnimationFolderPath;
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
        m_PreviousTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += inspectorUpdate;
        //isInspectorUpdate = true;
    }
    void GetLimbAnimationData()
    {
        LimbAnimationClipData=new List<AnimationAsset>();
        var assetPath = "Assets/" + limbAnimationFolderPath;
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
        m_PreviousTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += inspectorUpdate;
        //isInspectorUpdate = true;
    }
    private void OnDestroy()
    {
        EditorApplication.update -= inspectorUpdate;
        //isInspectorUpdate = false;
        Close();
    }

    public static void OnSelectChanged(AnimationAsset data,bool isSelected)
    {
        if (isSelected)
        {
            if (FacialAnimationClipData.Contains(data))
            {
                CurrentSelectFacialData = data.AnimationClip;
                //ModelAssetData.CurrentSelectFacialData = data.AnimationClip;
                CurrentSelectData.CurrentSelectFacialData = data.AnimationClip;
                FacialAnimationClipData.ForEach(x => { if (x != data) { x.IsSelected = false; } });
            }
            else
            {
                CurrentSelectLimbData = data.AnimationClip;
                //ModelAssetData.CurrentSelectLimbData = data.AnimationClip;
                CurrentSelectData.CurrentSelectLimbData = data.AnimationClip;
                LimbAnimationClipData.ForEach(x => { if (x != data) { x.IsSelected = false; } });
            }       
        }
        else
        {
            if (FacialAnimationClipData.Contains(data))
            {
                CurrentSelectFacialData = null;
                ModelAssetData.CurrentSelectFacialData.Clear();
                CurrentSelectData.CurrentSelectFacialData = null;
            }
            else
            {
                CurrentSelectLimbData = null;
                ModelAssetData.CurrentSelectLimbData.Clear();
                CurrentSelectData.CurrentSelectLimbData = null;
            }
        }
    }

    public static void OnResetSelect()
    {
        CurrentSelectFacialData = null;
        ModelAssetData.CurrentSelectFacialData.Clear();
        CurrentSelectData.CurrentSelectFacialData = null;
        FacialAnimationClipData.ForEach(x => { x.IsSelected = false; });
        CurrentSelectLimbData = null;
        ModelAssetData.CurrentSelectLimbData.Clear();
        CurrentSelectData.CurrentSelectLimbData = null;
        LimbAnimationClipData.ForEach(x => { x.IsSelected = false; });
    }

    public static void AnimatorBake(Animator animator,AnimatorState state,AnimationClip clip)
    {
        if (Application.isPlaying || state == null)
        {
            return;
        }
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

        hongPei = true;
        playtime = true;
    }
    
    /// <summary>
    /// Updates in Preview
    /// </summary>
    void update()
    {
        if (Application.isPlaying)
        {
            return;
        }

        // if (aniState == null)
        //     return;

        if (mAnimator != null && mAnimator.runtimeAnimatorController == null)
            return;

        if (!hongPei) 
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

        //  else {
        //     m_RunningTime = clipTime;
        //     animator.playbackTime = m_RunningTime;
        //     animator.Update(0);
        // }
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
}
public class AnimationAsset
{
    [TableColumnWidth(100,Resizable = false)]
    [OnValueChanged("OnSelectChanged")]
    public bool IsSelected;
    [TableColumnWidth(100,Resizable = false)]
    [PreviewField(100)]
    [AssetsOnly,ReadOnly]
    public AnimationClip AnimationClip;
    [TextArea(2,10)]
    [LabelWidth(50),ReadOnly]
    public string AnimationClipName;
    [HideInInspector]
    public GameObject Model;
    [ResponsiveButtonGroup("Actions")]
    [Button("Preview")]
    public void PreviewAction()
    {
        if(AnimationAssetData.CurrentSelectModel != null)
        {
            AnimationAssetData.Close();
            var controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Upsytools/Mecanim/Test.controller");
            var idle = controller.layers[0].stateMachine.AddState(AnimationClipName);
            idle.motion = AnimationClip;
            ModelAssetData.OnCancelPreview();
            //ModelAssetData.OnPreview(Model);
            ModelAssetData.OnPreview(AnimationAssetData.CurrentSelectModel);
            EditorCoroutineService.StartCoroutine(WaitForTwoSeconds(() =>
            {
                var previewParent = GameObject.Find("PreviewModel");
                var child = previewParent.transform.GetChild(0).gameObject;
                Selection.activeObject = child;
                if (previewParent != null)
                {
                    Animator animator = child.GetComponent<Animator>();
                    animator.runtimeAnimatorController = controller;
                    AnimationAssetData.AnimatorBake(animator, idle, AnimationClip);
                }
            }));
        }
    }
    
    private IEnumerator WaitForTwoSeconds( Action action)
    { 
        yield return new WaitForSeconds(1f);
        action?.Invoke();
    }
    
    private AnimationClip GetClip(RuntimeAnimatorController aniCons,string name)
    {
        foreach (var clip in aniCons.animationClips)
        {
            if (clip.name.Equals(name))
                return clip;
        }

        return null;
    }

    [ResponsiveButtonGroup("Actions")]
    [Button("Cancel Preview")]
    public void CloseAction()
    {
        AnimationAssetData.playtime = false;
        AnimationAssetData.Close();
        ModelAssetData.OnCancelPreview();
    }
    
    public static void ShowEditorWindowWithTypeName(string windowTypeName)
    {
        var windowType = typeof(Editor).Assembly.GetType(windowTypeName);
        EditorWindow.GetWindow(windowType);
    }
    private void OnSelectChanged()
    {
        ModelAssetData.OnAnimationSelectChanged(this, IsSelected);
        //AnimationAssetData.OnSelectChanged(this,IsSelected);
    }
}