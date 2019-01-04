using System;
using System.IO;

namespace SkillSystem
{
  public class AssetLocator
  {
    public AssetLocator()
    {
      FilePath = "";
      ItemName = "";
    }

    public AssetLocator(string filePath, string itemName)
    {
      FilePath = filePath;
      ItemName = itemName;
    }

    public string FilePath
    {
      set;
      get;
    }

    public string ItemName
    {
      set;
      get;
    }

    public bool IsEmpty()
    {
      return FilePath.Length == 0;
    }
  }

  public abstract class AbstractAssetManager
  {
    // can be polled multiple times
    public abstract bool Request(AssetLocator locator, Type assetType);

    // must always return a valid object
    public abstract UnityEngine.Object LoadAsset(AssetLocator locator, Type assetType);

    public T LoadAsset<T>(AssetLocator locator) where T : UnityEngine.Object
    {
      return LoadAsset(locator, typeof(T)) as T;
    }

    public interface IInstanceHandle<T> : IDisposable
    {
      T Instance
      {
        get;
      }
    }

    // pass in an object returned by LoadAsset and get back a "clone"
    public abstract IInstanceHandle<T> Instantiate<T>(T original) where T : UnityEngine.Object;

    public abstract Stream OpenFile(string filePath);
  }
}
