using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetsHelper : Editor
{

    public struct SceneObject
    {
        public SceneAsset sceneAsset;
        public string scenePath;
        public string sceneName;
        public Texture sceneTexture;

        public SceneObject(SceneAsset asset,string path,string sceneName, Texture sceneTexture)
        {
            this.sceneAsset = asset;
            this.scenePath = path;
            this.sceneName = sceneName;
            this.sceneTexture = sceneTexture;
        }
        
    }

    public static string CloneAssets(string assetPath)
    {
        var str = assetPath.Split('/');
        var fullName= str[str.Length - 1];
        var assetName = fullName.Split('.')[0];
        var suffix = fullName.Split('.')[1];
        var newPath = "Assets/" + assetName + "_temp" + "." + suffix;
        AssetDatabase.CopyAsset(assetPath, newPath);
        return newPath;
    }

    public static void DeleteAsset(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        AssetDatabase.DeleteAsset(path);
    }

    public static SceneObject[] GetSceneObjectsByPath(string folderPath)
    {
       var guid =AssetDatabase.FindAssets("t:Scene",new []{folderPath});
       var length = guid.Length;
       var sceneObject=new  SceneObject[length];
       for (int i = 0; i < length; i++)
       {
           var path= AssetDatabase.GUIDToAssetPath(guid[i]);
           var sa= AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
           sceneObject[i].sceneAsset = sa;
           sceneObject[i].sceneName = sa.name;
           sceneObject[i].scenePath = path;
           sceneObject[i].sceneTexture = AssetDatabase.LoadAssetAtPath(folderPath+"/"+ sa.name + ".png", typeof(Texture2D)) as Texture2D;
       }
       return sceneObject;
    }
    
    
    public static SceneObject[] GetSceneObjects()
    {
        var guid= AssetDatabase.FindAssets("t:Scene");
        var length = guid.Length;
        var SceneObject=new  SceneObject[length];
        for (int i = 0; i < length; i++)
        {
            var path= AssetDatabase.GUIDToAssetPath(guid[i]);
            var sa= AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            SceneObject[i].sceneAsset = sa;
            SceneObject[i].sceneName = sa.name;
            SceneObject[i].scenePath = path;
        }
        return SceneObject;
    }
    public static SceneAsset[] GetSceneAssets()
    {
        var guid= AssetDatabase.FindAssets("t:Scene");
        var length = guid.Length;
        var sceneAssets=new  SceneAsset[length];
        for (int i = 0; i < length; i++)
        {
            var path= AssetDatabase.GUIDToAssetPath(guid[i]);
            var sa= AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            sceneAssets[i] = sa;
        }
        return sceneAssets;
    }
    public static GameObject[] GetModelObjectsByPath(string folderPath)
    {
        var guid =AssetDatabase.FindAssets("t:GameObject",new []{folderPath});
        var length = guid.Length;
        var sceneAssets=new  GameObject[length];
        for (int i = 0; i < length; i++)
        {
            var path= AssetDatabase.GUIDToAssetPath(guid[i]);
            var sa= AssetDatabase.LoadAssetAtPath<GameObject>(path);
            sceneAssets[i] = sa;
        }
        return sceneAssets;
    }
    public static GameObject[] GetModelAssets()
    {
        var folder = new[] {"Assets/Models/Animations"};
        var guid= AssetDatabase.FindAssets("t:GameObject",folder);
        var length = guid.Length;
        var sceneAssets=new  GameObject[length];
        for (int i = 0; i < length; i++)
        {
            var path= AssetDatabase.GUIDToAssetPath(guid[i]);
            var sa= AssetDatabase.LoadAssetAtPath<GameObject>(path);
            sceneAssets[i] = sa;
        }
        return sceneAssets;
    }
    public struct AnimationObject
    {
        public GameObject clipGo;
        public AnimationClip clip;
        public string clipName;
         
        public AnimationObject(GameObject go,AnimationClip asset,string clipName)
        {
            this.clipGo = go;
            this.clip = asset;
            this.clipName = clipName;
        }
        
    }
    public static AnimationObject[] GetAnimationAssets(string folderPath)
    {
        var folder = new[] {folderPath};
        var guid= AssetDatabase.FindAssets("t:Animation",folder);
        var length = guid.Length;
        var sceneAssets=new  AnimationObject[length];
        for (int i = 0; i < length; i++)
        {
            var path= AssetDatabase.GUIDToAssetPath(guid[i]);
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var sa= AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            sceneAssets[i].clipGo = go;
            sceneAssets[i].clip = sa;
            sceneAssets[i].clipName = go.name;
        }
        return sceneAssets;
    }
}
