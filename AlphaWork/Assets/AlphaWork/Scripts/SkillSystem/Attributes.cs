using System;

namespace SkillSystem
{
  [AttributeUsage(AttributeTargets.Property)]
  public class AssetAttribute : Attribute
  {
    public AssetAttribute()
    {
      AssetType = typeof(UnityEngine.Object);
    }

    public AssetAttribute(Type assetType)
    {
      AssetType = assetType;
    }

    public Type AssetType
    {
      private set;
      get;
    }
  }

  [AttributeUsage(AttributeTargets.Property)]
  public class FolderAttribute : Attribute
  {

  }

  [AttributeUsage(AttributeTargets.Property)]
  public class PolymorphicAttribute : Attribute
  {
    public PolymorphicAttribute(params Type[] typeList)
    {
      TypeList = typeList;
    }

    public Type[] TypeList
    {
      private set;
      get;
    }
  }

  [AttributeUsage(AttributeTargets.Property)]
  public class SliderTweakableAttribute : Attribute
  {
    public SliderTweakableAttribute(float min, float max)
    {
      Min = min;
      Max = max;
    }

    public float Min
    {
      private set;
      get;
    }

    public float Max
    {
      private set;
      get;
    }
  }

  [AttributeUsage(AttributeTargets.Property)]
  public class AnimationProgressAttribute : Attribute
  {

  }

  [AttributeUsage(AttributeTargets.Class)]
  public class TriggerAttribute : Attribute
  {

  }

  [AttributeUsage(AttributeTargets.Class)]
  public class EffectAttribute : Attribute
  {

  }
}
