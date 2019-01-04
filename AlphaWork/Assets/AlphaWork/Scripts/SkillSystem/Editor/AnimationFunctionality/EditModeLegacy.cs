using System;

using UnityEngine;
using UnityEditor;

namespace SkillSystem
{
  partial class AnimationFunctionality
  {
    class EditModeLegacy : IAccess
    {
      static int s_animModeNestLevel;

      GameObject m_gameObj;
      AnimationClip m_animClip;
      float m_animProgress;
      IAnimProgressUpdater m_animProgressUpdater;
      bool m_paused;

      interface IAnimProgressUpdater : IDisposable
      {
        float Progress
        {
          set;
        }
      }

      class AutoPlayingMode : IAnimProgressUpdater
      {
        EditModeLegacy m_owner;
        Action m_doUpdate;

        float IAnimProgressUpdater.Progress
        {
          set
          {

          }
        }

        void IDisposable.Dispose()
        {
          GlobalObj<UpdateEventForwarder>.Instance.OnUpdate -= UpdateProgress;
        }

        void UpdateProgress()
        {
          m_doUpdate();
        }

        public AutoPlayingMode(EditModeLegacy owner)
        {
          m_owner = owner;
          owner.m_animProgress = 0.0f;

          var timer = 0.0f;

          m_doUpdate = () =>
          {
            if (m_owner.m_paused)
              return;

            var clipLength = m_owner.m_animClip.length;

            if (timer >= clipLength)
            {
              if (m_owner.m_animClip.isLooping)
                timer = Mathf.Repeat(timer, clipLength);
              else
              {
                timer = 0.0f;
                m_doUpdate = () => { };
              }
            }

            m_owner.m_animProgress = timer / clipLength;
            m_owner.UpdateAnimation();

            timer += Time.deltaTime;
          };

          GlobalObj<UpdateEventForwarder>.Instance.OnUpdate += UpdateProgress;
        }
      }

      class UserControlledMode : IAnimProgressUpdater
      {
        EditModeLegacy m_owner;

        float IAnimProgressUpdater.Progress
        {
          set
          {
            m_owner.m_animProgress = value;
            m_owner.UpdateAnimation();
          }
        }

        void IDisposable.Dispose()
        {

        }

        public UserControlledMode(EditModeLegacy owner)
        {
          m_owner = owner;
        }
      }

      void UpdateAnimation()
      {
        AnimationMode.SampleAnimationClip(m_gameObj, m_animClip, m_animProgress * m_animClip.length);
      }

      void Start(IAnimProgressUpdater progressUpdater)
      {
        if (0 == s_animModeNestLevel)
        {
          AnimationMode.StartAnimationMode();
          AnimationMode.BeginSampling();
        }

        ++s_animModeNestLevel;

        m_animProgressUpdater = progressUpdater;

        UpdateAnimation();
      }

      void Stop()
      {
        m_animProgressUpdater.Dispose();

        --s_animModeNestLevel;

        if (0 == s_animModeNestLevel)
        {
          AnimationMode.EndSampling();
          AnimationMode.StopAnimationMode();
        }
      }

      void IAccess.StartAutoPlaying()
      {
        Start(new AutoPlayingMode(this));
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
        Stop();
      }

      void IAccess.StartUserControlledPlaying()
      {
        Start(new UserControlledMode(this));
      }

      void IAccess.StopUserControlledPlaying()
      {
        Stop();
      }

      float IAccess.Progress
      {
        set
        {
          m_animProgressUpdater.Progress = value;
        }

        get
        {
          return m_animProgress;
        }
      }

      public EditModeLegacy(GameObject obj, AnimationClip animClip)
      {
        m_gameObj = obj;
        m_animClip = animClip;
      }

      void IDisposable.Dispose()
      {

      }
    }
  }
}
