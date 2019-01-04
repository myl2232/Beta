using System;

using UnityEngine;
using UnityEditor.Animations;

namespace SkillSystem
{
  partial class AnimationFunctionality
  {
    public class PlayModeMecanim : IAccess
    {
      Animator m_animator;
      AnimatorController m_animController;
      string m_tmpAnimControllerFilePath;

      void IAccess.StartAutoPlaying()
      {
        m_animator.runtimeAnimatorController = null;
        m_animator.runtimeAnimatorController = m_animController;
        m_animator.speed = 1.0f;
      }

      void IAccess.PauseAutoPlaying()
      {
        m_animator.speed = 0.0f;
      }

      void IAccess.ResumeAutoPlaying()
      {
        m_animator.speed = 1.0f;
      }

      void IAccess.StopAutoPlaying()
      {
        m_animator.speed = 0.0f;
      }

      void IAccess.StartUserControlledPlaying()
      {
        m_animator.runtimeAnimatorController = null;
        m_animator.runtimeAnimatorController = m_animController;
        m_animator.StartPlayback();
      }

      void IAccess.StopUserControlledPlaying()
      {
        m_animator.StopPlayback();
      }

      float IAccess.Progress
      {
        set
        {
          m_animator.playbackTime = Mathf.Clamp(value * m_animator.runtimeAnimatorController.animationClips[0].length,
              m_animator.recorderStartTime,
              m_animator.recorderStopTime);
        }

        get
        {
          return AnimationProgressQuery.GetAnimProgress(m_animator);
        }
      }

      public PlayModeMecanim(GameObject gameObj, AnimationClip animClip)
      {
        m_animator = GameObjectUtility.TryAddComponent<Animator>(gameObj);

        m_tmpAnimControllerFilePath = GlobalObj<EditModeAssetManager>.Instance.GenTmpFilePath("controller");

        m_animController = AnimatorController.CreateAnimatorControllerAtPathWithClip(m_tmpAnimControllerFilePath, animClip);

        var frameCount = 1000;

        m_animator.runtimeAnimatorController = m_animController;
        m_animator.StartRecording(frameCount);

        var deltaTime = animClip.length / frameCount;
        for (var frame = 0; frame != frameCount; ++frame)
          m_animator.Update(deltaTime);

        m_animator.StopRecording();
        m_animator.runtimeAnimatorController = null;
        m_animator.speed = 0.0f;
      }

      void IDisposable.Dispose()
      {
        m_animator.runtimeAnimatorController = null;
        m_animator.speed = 1.0f;

        GlobalObj<EditModeAssetManager>.Instance.DeleteAssetFile(m_tmpAnimControllerFilePath);
      }
    }
  }
}
