using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[ShowInfo]
public class Test : ShowData
{
    public int inter;
    public string strings;
    [SerializeField]
    private bool boolTest;

    public GameObject go;
    public Vector3 v=Vector3.one;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < DataClass.Count; i++)
        {
            var type = DataClass[i].GetType();
            var proInfos = Reflection.GetSerializableFields(type);
            foreach (var propertie in proInfos)
            {
                Debug.Log(propertie.GetValue(DataClass[i]));
            }
        }
        var types = Reflection.GetAllFromCurrentAssembly();
        for (int i = 0; i < types.Length; i++)
        {
            var t = types[i];
            var att= Attribute.GetCustomAttribute(types[i], typeof(ShowInfo));
            if (att == null)
            {
                Debug.Log("不存在");
            }
            else
            {
                var  instance= Activator.CreateInstance(t);
                var proInfos = Reflection.GetSerializableFields(instance.GetType());
                foreach (var propertie in proInfos)
                {
                    Debug.Log(propertie.GetValue(instance));
                }
            }
        }
        
    }

}
