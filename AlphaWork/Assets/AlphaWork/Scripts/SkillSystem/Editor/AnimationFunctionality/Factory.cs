using System;
using UnityEngine;
using UnityEditor;

namespace SkillSystem
{
  partial class AnimationFunctionality
  {
    public interface IAccess : IDisposable
    {
      void StartAutoPlaying();
      void PauseAutoPlaying();
      void ResumeAutoPlaying();
      void StopAutoPlaying();

      void StartUserControlledPlaying();
      void StopUserControlledPlaying();

      float Progress
      {
        set;
        get;
      }
    }

    public static IAccess MakeAccess(GameObject gameObj, AnimationClip animClip)
    {
      if (null == animClip)
        return new NullAccess();

      if (EditorApplication.isPlaying)
      {
        if (animClip.legacy)
          return new PlayModeLegacy(gameObj, animClip);

        return new PlayModeMecanim(gameObj, animClip);
      }

      if (animClip.legacy)
        return new EditModeLegacy(gameObj, animClip);

      return new EditModeMecanim(gameObj, animClip);
    }

    public static IAccess MakeAccess(GameObject gameObj)
    {
      var animator = gameObj.GetComponent<Animator>();
      if (null == animator)
        return new NullAccess();

      if (null == animator.runtimeAnimatorController)
        return new NullAccess();

      var clips = animator.runtimeAnimatorController.animationClips;
      if (clips.Length != 1)
        return new NullAccess();

      if (!EditorApplication.isPlaying)
        return new EditModeLegacy(gameObj, clips[0]);

      return new NullAccess();
    }
  }
}
