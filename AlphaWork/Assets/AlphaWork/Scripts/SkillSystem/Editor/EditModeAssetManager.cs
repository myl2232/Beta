using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace SkillSystem
{
  class EditModeAssetManager : AbstractAssetManager
  {
    const string c_tmpFileFolder = "SkillEditorTmpFiles";

    static EditModeAssetManager()
    {
      if (!AssetDatabase.IsValidFolder("Assets/" + c_tmpFileFolder))
        AssetDatabase.CreateFolder("Assets", c_tmpFileFolder);
    }

    public string GenTmpFilePath(string ext)
    {
      return "Assets/" + c_tmpFileFolder + "/" + Guid.NewGuid().ToString("N") + "." + ext;
    }

    public void DeleteAssetFile(string filePath)
    {
      AssetDatabase.DeleteAsset(filePath);
    }

    public AssetLocator GetAssetLocator(UnityEngine.Object obj)
    {
      return new AssetLocator(AssetDatabase.GetAssetPath(obj), obj.name);
    }

    public override bool Request(AssetLocator locator, Type assetType)
    {
      if (locator.IsEmpty() || null == LoadAsset(locator, assetType))
      {
        Debug.LogError("SkillSystem.EditModeAssetManager cannot load asset \"" + locator.FilePath + "\"");
        return false;
      }

      return true;
    }

    public T LoadAsset<T>(string filePath)
      where T : UnityEngine.Object
    {
      return AssetDatabase.LoadAssetAtPath<T>(filePath);
    }

    public override UnityEngine.Object LoadAsset(AssetLocator locator, Type assetType)
    {
      return Array.Find(AssetDatabase.LoadAllAssetsAtPath(locator.FilePath),
               obj => obj.name == locator.ItemName && assetType.IsAssignableFrom(obj.GetType()));
    }

    public class InstanceHandle<T> : IInstanceHandle<T>
      where T : UnityEngine.Object
    {
      List<AnimationFunctionality.IAccess> m_animFuncs =
        new List<AnimationFunctionality.IAccess>();

      public T Instance
      {
        private set;
        get;
      }

      void IDisposable.Dispose()
      {
        m_animFuncs.ForEach(animFunc =>
        {
          animFunc.StopAutoPlaying();
          animFunc.Dispose();
        });
        UnityEngine.Object.DestroyImmediate(Instance);
      }

      public InstanceHandle(T original)
      {
        Instance = UnityEngine.Object.Instantiate(original);

        if (!EditorApplication.isPlaying)
        {
          var gameObj = Instance as GameObject;
          if (null == gameObj)
            return;

          GameObjectUtility.ForEach(gameObj, obj =>
          {
            var animFunc = AnimationFunctionality.MakeAccess(obj);
            animFunc.StartAutoPlaying();
            m_animFuncs.Add(animFunc);
          });
        }
      }
    }

    public override IInstanceHandle<T> Instantiate<T>(T original)
    {
      return new InstanceHandle<T>(original);
    }

    public override Stream OpenFile(string filePath)
    {
      return File.Open(filePath, FileMode.Open);
    }

    public FileStream CreateFile(string filePath)
    {
      return File.Open(filePath, FileMode.Create);
    }
  }
}
