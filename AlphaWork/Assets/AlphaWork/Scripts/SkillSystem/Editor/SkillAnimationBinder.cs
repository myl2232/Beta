using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace SkillSystem
{
  class SkillAnimationBinder
  {
    GUIWrapper.PropertyPane m_modeSwitch;

    public interface IBindingMode
    {
      AnimationClip GetAnimationClip(string skillName);
    }

    class LookUpInFolder : IBindingMode
    {
      Dictionary<string, AnimationClip> m_skillNameToAnimClip = new Dictionary<string, AnimationClip>();

      AnimationClip FindAnimationClip(Func<string, AnimationClip> tryLoad)
      {
        foreach (var filePath in Directory.GetFiles(Folder, "*.*", SearchOption.TopDirectoryOnly))
        {
          if (StringUtility.CaseInsensitiveMatchExtension(filePath, "anim", "fbx"))
          {
            var path = StringUtility.FormatFilePath(filePath);
            var animClip = tryLoad(path.Substring(path.IndexOf("Assets/")));
            if (null != animClip)
              return animClip;
          }
        }

        return null;
      }

      AnimationClip IBindingMode.GetAnimationClip(string skillName)
      {
        AnimationClip r;
        if (m_skillNameToAnimClip.TryGetValue(skillName, out r))
          return r;

        var animClip = FindAnimationClip(filePath =>
        {
          if (StringUtility.CaseInsensitiveEquals(Path.GetFileNameWithoutExtension(filePath), skillName))
            return GlobalObj<EditModeAssetManager>.Instance.LoadAsset<AnimationClip>(filePath);
          return null;
        });

        animClip = animClip ??
          FindAnimationClip(filePath =>
            GlobalObj<EditModeAssetManager>.Instance.LoadAsset<AnimationClip>(
              new AssetLocator { FilePath = filePath, ItemName = skillName }));

        if (null == animClip)
          GUIWrapper.MessageBox("Cannot find AnimationClip for skill \"" + skillName + "\" !");
        else
          m_skillNameToAnimClip.Add(skillName, animClip);

        return animClip;
      }

      [Folder]
      public string Folder
      {
        set;
        get;
      }

      public LookUpInFolder()
      {
        Folder = Application.dataPath;
      }
    }

    class ManuallyAssigned : IBindingMode
    {
      AssetLocator m_assetLocator = new AssetLocator();
      AnimationClip m_animClip;

      AnimationClip IBindingMode.GetAnimationClip(string skillName)
      {
        return m_animClip;
      }

      [Asset(typeof(AnimationClip))]
      public AssetLocator Animation
      {
        set
        {
          m_assetLocator = value;
          m_animClip = GlobalObj<EditModeAssetManager>.Instance.LoadAsset<AnimationClip>(value);
        }

        get
        {
          return m_assetLocator;
        }
      }
    }

    [Polymorphic(typeof(LookUpInFolder), typeof(ManuallyAssigned))]
    public IBindingMode PreviewAnimation
    {
      set;
      get;
    }

    public SkillAnimationBinder()
    {
      PreviewAnimation = new ManuallyAssigned();

      m_modeSwitch = new GUIWrapper.PropertyPane(this);
    }

    public AnimationClip GetAnimationClip(string skillName)
    {
      return PreviewAnimation.GetAnimationClip(skillName);
    }

    public void OnGUI()
    {
      m_modeSwitch.OnGUI();
    }
  }
}
