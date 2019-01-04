using System;

using UnityEngine;

namespace SkillSystem
{
  partial class AnimationFunctionality
  {
    public class PlayModeLegacy : IAccess
    {
      const string c_tmpAnimClipName = "TmpAnimClip";

      Animation m_animation;

      void DoPlay()
      {
        m_animation.Play(c_tmpAnimClipName);
      }

      void DoStop()
      {
        m_animation.Stop();
      }

      void IAccess.StartAutoPlaying()
      {
        DoPlay();
      }

      void IAccess.PauseAutoPlaying()
      {
        m_animation[c_tmpAnimClipName].speed = 0.0f;
      }

      void IAccess.ResumeAutoPlaying()
      {
        m_animation[c_tmpAnimClipName].speed = 1.0f;
      }

      void IAccess.StopAutoPlaying()
      {
        DoStop();
      }

      void IAccess.StartUserControlledPlaying()
      {
        m_animation[c_tmpAnimClipName].speed = 0.0f;
        DoPlay();
      }

      void IAccess.StopUserControlledPlaying()
      {
        DoStop();
        m_animation[c_tmpAnimClipName].speed = 1.0f;
      }

      float IAccess.Progress
      {
        set
        {
          m_animation[c_tmpAnimClipName].normalizedTime = value;
        }

        get
        {
          return AnimationProgressQuery.NormalizedAnimTimeToProgress(m_animation[c_tmpAnimClipName].normalizedTime);
        }
      }

      public PlayModeLegacy(GameObject gameObj, AnimationClip animClip)
      {
        m_animation = GameObjectUtility.TryAddComponent<Animation>(gameObj);
        m_animation.AddClip(animClip, c_tmpAnimClipName);
      }

      void IDisposable.Dispose()
      {
        m_animation.RemoveClip(c_tmpAnimClipName);
        UnityEngine.Object.DestroyImmediate(m_animation);
      }
    }
  }
}
