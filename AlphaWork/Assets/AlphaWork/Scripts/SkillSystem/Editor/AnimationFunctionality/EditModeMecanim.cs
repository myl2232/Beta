using System;

using UnityEngine;

namespace SkillSystem
{
  partial class AnimationFunctionality
  {
    public class EditModeMecanim : IAccess
    {
      IAccess m_impl;
      Animator m_animator;
      Func<float> m_deltaTime;
      bool m_paused;

      void IAccess.StartAutoPlaying()
      {
        int delayFrames = 5;
        m_deltaTime = () =>
        {
          if (delayFrames > 0)
          {
            --delayFrames;
            return 0.0f;
          }
          return m_paused ? 0.0f : Time.deltaTime;
        };

        m_impl.StartAutoPlaying();
        m_animator.Update(0);
      }

      void IAccess.PauseAutoPlaying()
      {
        m_paused = true;
      }

      void IAccess.ResumeAutoPlaying()
      {
        m_paused = false;
      }

      void IAccess.StopAutoPlaying()
      {
        m_impl.StopAutoPlaying();
        m_deltaTime = null;
      }

      void IAccess.StartUserControlledPlaying()
      {
        m_deltaTime = () => 0;
        m_impl.StartUserControlledPlaying();
        m_animator.Update(0);
      }

      void IAccess.StopUserControlledPlaying()
      {
        m_impl.StopUserControlledPlaying();
        m_deltaTime = null;
      }

      float IAccess.Progress
      {
        set
        {
          m_impl.Progress = value;
        }

        get
        {
          return m_impl.Progress;
        }
      }

      void Update()
      {
        if (null != m_deltaTime)
          m_animator.Update(m_deltaTime());
      }

      void IDisposable.Dispose()
      {
        m_impl.Dispose();

        GlobalObj<UpdateEventForwarder>.Instance.OnUpdate -= Update;
      }

      public EditModeMecanim(GameObject gameObj, AnimationClip animClip)
      {
        m_impl = new PlayModeMecanim(gameObj, animClip);
        m_animator = gameObj.GetComponent<Animator>();

        GlobalObj<UpdateEventForwarder>.Instance.OnUpdate += Update;
      }
    }
  }
}
