using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShowData : MonoBehaviour
{
    public GameObject title, itemGo, panel;
    public List<object> DataClass=new List<object>();
    public virtual void Awake()
    {
        var self = GetComponent<ShowData>();
        if (!DataClass.Contains(self))
        {
            DataClass.Add(self);
        }
    }

    private void Start()
    {
        
    }

    private bool show=false;
    private int index = -1;
    private void OnGUI()
    {
        if(GUI.Button(new Rect(0, 10, 70, 30),  "实时数据"))
        {
            show = !show;
        }
        if (!show) return;
        for (int i = 0; i < DataClass.Count; i++)
        {
            var type = DataClass[i].GetType();
            var proInfos = Reflection.GetSerializableFields(type);
            foreach (var propertie in proInfos)
            {
                index++;
                var value = propertie.GetValue(DataClass[i]);
                GUIStyle style = new GUIStyle();
                style.fontSize = 16;
                style.fontStyle = FontStyle.Normal;
                style.normal.textColor = Color.yellow;
                var y = (index+1) * 40;
                GUI.Label(new Rect(0, y, 100, 40), "名称:"+propertie.Name,style);
                GUI.Label(new Rect(110, y, 250, 40), "类型:"+propertie.FieldType,style);
                GUI.Label(new Rect(360, y, 100, 40), "数据:"+value, style); 
            }

            if (i == DataClass.Count - 1)
            {
                index = -1;
            }
        }

    }
}
