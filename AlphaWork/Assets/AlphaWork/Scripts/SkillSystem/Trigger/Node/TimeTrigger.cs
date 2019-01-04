using System;

using UnityEngine;

namespace SkillSystem
{
  [Trigger]
  public class TimeTrigger : SkillEffectTrigger
  {
    class Instance : SkillEffectTriggerInstance<TimeTrigger>
    {
      Func<bool> m_shouldTrigger;

      public Instance(TimeTrigger trigger, SkillRuntimeContext context)
        : base(trigger)
      {
        var timer = 0.0f;

        m_shouldTrigger = () =>
        {
          if (timer >= trigger.Time)
          {
            m_shouldTrigger = () => false;
            return true;
          }

          timer += UnityEngine.Time.deltaTime;
          return false;
        };
      }

      protected override bool ShouldTrigger(SkillRuntimeContext context)
      {
        return m_shouldTrigger();
      }
    }

    protected override ISkillEffectTriggerInstance DoInstantiate(SkillRuntimeContext context)
    {
      return new Instance(this, context);
    }

    float m_time;
    public float Time
    {
      set
      {
        m_time = Mathf.Max(0.0f, value);
      }

      get
      {
        return m_time;
      }
    }
  }
}
