using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ObjectFieldAlignment = Sirenix.OdinInspector.ObjectFieldAlignment;

public class UiAssetData : SerializedScriptableObject
{

    [HideInInspector]
    public DefaultControls.Resources res;
    
    [Button(ButtonSizes.Large)]
    [FoldoutGroup("Button",Expanded = true)]
    [HorizontalGroup("Button/Horizontal")]
    [BoxGroup("Button/Horizontal/Style")]
    public void Button() { }

    [Button(ButtonSizes.Large)]
    [BoxGroup("Button/Horizontal/Action")]
    [GUIColor(0, 1, 0)]
    public void CreateButton()
    {
        CreateUi(UiType.Button);
    }

    [Space(20)]
    [FoldoutGroup("Toggle", Expanded = true)]
    [HorizontalGroup("Toggle/Horizontal")]
    [BoxGroup("Toggle/Horizontal/Style")]
    [ToggleLeft,LabelWidth(30)]
    [LabelText("Toggle")]
    public bool mToggle;

    [Button(ButtonSizes.Large)]
    [BoxGroup("Toggle/Horizontal/Action")]
    [GUIColor(0, 1, 0)]
    public void CreateToggle()
    {
        CreateUi(UiType.Toggle);
    }
    
    [Space(20)]
    [FoldoutGroup("Slider", Expanded = true)]
    [HorizontalGroup("Slider/Horizontal")]
    [BoxGroup("Slider/Horizontal/Style")]
    [LabelWidth(30)]
    [ProgressBar(0,100)]
    [HideLabel]
    public int mSlider=50;

    [Button(ButtonSizes.Large)]
    [BoxGroup("Slider/Horizontal/Action")]
    [GUIColor(0, 1, 0)]
    public void CreateSlider()
    {
        CreateUi(UiType.Slider);
    }
    
    [Space(20)]
    [FoldoutGroup("Dropdown", Expanded = true)]
    [HorizontalGroup("Dropdown/Horizontal")]
    [BoxGroup("Dropdown/Horizontal/Style")]
    [LabelWidth(30)]
    [ValueDropdown("dropValue")]
    [HideLabel]
    public int mDropdown=1;

    [Button(ButtonSizes.Large)]
    [BoxGroup("Dropdown/Horizontal/Action")]
    [GUIColor(0, 1, 0)]
    public void CreateDropdown()
    {
        CreateUi(UiType.Dropdown);
    }
    private static int[] dropValue = new[] {1, 2, 3};
    
    
    [Space(20)]
    [FoldoutGroup("InputField", Expanded = true)]
    [HorizontalGroup("InputField/Horizontal")]
    [BoxGroup("InputField/Horizontal/Style")]
    [LabelWidth(30)]
    [HideLabel]
    
    public string mInput="please enter string...";

    [Button(ButtonSizes.Large)]
    [BoxGroup("InputField/Horizontal/Action")]
    [GUIColor(0, 1, 0)]
    public void CreateInput()
    {
        CreateUi(UiType.Input);
    }
    [Space(20)]
    [FoldoutGroup("Image", Expanded = true)]
    [HorizontalGroup("Image/Horizontal")]
    [BoxGroup("Image/Horizontal/Style")]
    [LabelWidth(30)]
    [PreviewField(40,ObjectFieldAlignment.Left)]
    [HideLabel]
    public Texture2D Texture ;
    
    [Button(ButtonSizes.Large)]
    [BoxGroup("Image/Horizontal/Action")]
    [GUIColor(0, 1, 0)]
    public void CreateImage()
    {
        CreateUi(UiType.Image);
    }
    [Space(20)]
    [FoldoutGroup("Text", Expanded = true)]
    [HorizontalGroup("Text/Horizontal")]
    [BoxGroup("Text/Horizontal/Style")]
    [LabelWidth(30)]
    [HideLabel]
    [InfoBox("Text")]
    public string info="Text" ;
    
    [Button(ButtonSizes.Large)]
    [BoxGroup("Text/Horizontal/Action")]
    [GUIColor(0, 1, 0)]
    public void CreateText()
    {
        CreateUi(UiType.Text);
    }

    private void OnEnable()
    {
        Texture= EditorIcons.UnityLogo;
        res = new DefaultControls.Resources();
        res.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        res.background = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        res.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/InputFieldBackground.psd");
        res.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        res.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Checkmark.psd");
        res.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/DropdownArrow.psd");
        res.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UIMask.psd");
    }

    public void CreateUi(UiType uiType)
    {
        var canvas= FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            canvas= new GameObject("Canvas",new Type[]{typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster)}).GetComponent<Canvas>();
        }
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var eventSystem=FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            eventSystem=new GameObject("EventSystem",new Type[]{typeof(EventSystem),typeof(StandaloneInputModule)}).GetComponent<EventSystem>();
        }
        switch (uiType)
        {
            case UiType.Text:
                var text= DefaultControls.CreateText(res);
                text.transform.SetParent(canvas.transform);
                text.transform.GetComponent<RectTransform>().anchoredPosition=Vector3.zero;
                break;
            case UiType.Image:
                var image= DefaultControls.CreateImage(res);
                image.transform.SetParent(canvas.transform);
                image.transform.GetComponent<RectTransform>().anchoredPosition=Vector3.zero;
                break;
            case UiType.Input:
                var input= DefaultControls.CreateInputField(res);
                input.transform.SetParent(canvas.transform);
                input.transform.GetComponent<RectTransform>().anchoredPosition=Vector3.zero;
                break;
            case UiType.Dropdown:
                var drop= DefaultControls.CreateDropdown(res);
                drop.transform.SetParent(canvas.transform);
                drop.transform.GetComponent<RectTransform>().anchoredPosition=Vector3.zero;
                break;
            case UiType.Slider:
                var slider= DefaultControls.CreateSlider(res);
                slider.transform.SetParent(canvas.transform);
                slider.transform.GetComponent<RectTransform>().anchoredPosition=Vector3.zero;
                break;
            case UiType.Button:
                var btn= DefaultControls.CreateButton(res);
                btn.transform.SetParent(canvas.transform);
                btn.transform.GetComponent<RectTransform>().anchoredPosition=Vector3.zero;
                break;
            case UiType.Toggle:
                var toggle= DefaultControls.CreateToggle(res);
                toggle.transform.SetParent(canvas.transform);
                toggle.transform.GetComponent<RectTransform>().anchoredPosition=Vector3.zero;
                break;
        }

        VREditorTools.uiWindow.orthographic = true;
    }

    public enum UiType
    {
        Text,
        Image,
        Input,
        Dropdown,
        Slider,
        Button,
        Toggle
    }
    
    
}

