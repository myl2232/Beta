using System;
using System.Collections.Generic;

using UnityEngine;
using SkillSystem;

namespace Galaxy
{
  [Effect]
  public class PauseSkillEffect : SkillEffect
  {
    class PausedPeriod
    {
      SkillComponent m_skillComponent;
      float m_duration;
      float m_timer;

      public PausedPeriod(SkillComponent skillComponent, float duration)
      {
        m_skillComponent = skillComponent;
        m_duration = duration;
      }

      public void DuringPaused()
      {
        if (m_timer >= m_duration)
        {
          if (m_skillComponent)
            m_skillComponent.Resume();

          GlobalObj<UpdateEventForwarder>.Instance.OnUpdate -= DuringPaused;
        }
        else
          m_timer += Time.deltaTime;
      }
    }

    protected override bool ShouldInstantiate(SkillRuntimeContext context)
    {
      Func<float, bool> pred;
      if (s_beforePauseChecks.TryGetValue(context.TheGameObject, out pred))
      {
        if (pred(Duration))
        {
          context.TheGameObject.GetComponent<SkillComponent>().Pause();

          GlobalObj<UpdateEventForwarder>.Instance.OnUpdate +=
            new PausedPeriod(context.TheGameObject.GetComponent<SkillComponent>(), Duration).DuringPaused;
        }
      }

      return false;
    }

    protected override ISkillEffectInstance DoInstantiate(SkillRuntimeContext context)
    {
      return null;
    }

    float m_duration;
    public float Duration
    {
      set
      {
        m_duration = Mathf.Max(0.0f, value);
      }
      get
      {
        return m_duration;
      }
    }

    static Dictionary<GameObject, Func<float, bool>> s_beforePauseChecks = new Dictionary<GameObject, Func<float, bool>>();

    public static void RegisterBeforePauseCheck(GameObject gameObj, Func<float, bool> pred)
    {
      s_beforePauseChecks.Add(gameObj, pred);
    }

    public static void UnregisterBeforePauseCheck(GameObject gameObj)
    {
      s_beforePauseChecks.Remove(gameObj);
    }
  }
}
