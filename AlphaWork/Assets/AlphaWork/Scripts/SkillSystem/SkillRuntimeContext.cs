using System;
using System.Collections.Generic;

using UnityEngine;

namespace SkillSystem
{
  public class SkillRuntimeContext
  {
    SharedState m_sharedState;
    Predicate<ISkillEffectInstance> m_updateSkillEffectInstance;
    AnimationProgressQuery.IQuery m_animationProgressQuery;

    public class SharedState : IDisposable
    {
      NonAbandonableEffectInstances m_nonAbandonableEffectInstances = new NonAbandonableEffectInstances();

      public SharedState(GameObject gameObject)
      {
        TheGameObject = gameObject;
      }

      public GameObject TheGameObject
      {
        private set;
        get;
      }

      public void AddNonAbandonableEffectInstance(ISkillEffectInstance effect,
        Predicate<ISkillEffectInstance> updateSkillEffectInstance)
      {
        m_nonAbandonableEffectInstances.Add(effect, updateSkillEffectInstance);
      }

      public void Update()
      {
        m_nonAbandonableEffectInstances.Update();
      }

      public void Dispose()
      {
        m_nonAbandonableEffectInstances.Dispose();
      }

      public void Pause()
      {
        m_nonAbandonableEffectInstances.Pause();
      }

      public void Resume()
      {
        m_nonAbandonableEffectInstances.Resume();
      }
    }

    public SkillRuntimeContext(SharedState sharedState, AnimationProgressQuery.IQuery query, Skill skill)
    {
      m_sharedState = sharedState;

      m_updateSkillEffectInstance = inst => inst.Update(this);
      m_animationProgressQuery = query;

      TheSkill = skill;
    }

    public Skill TheSkill
    {
      private set;
      get;
    }

    public GameObject TheGameObject
    {
      get
      {
        return m_sharedState.TheGameObject;
      }
    }

    public float AnimProgress
    {
      get
      {
        return m_animationProgressQuery.GetProgress();
      }
    }

    public void UpdateSkillEffectInstance(List<ISkillEffectInstance> instances)
    {
      instances.RemoveAll(m_updateSkillEffectInstance);
    }

    public void AddNonAbandonableEffect(ISkillEffectInstance effect)
    {
      m_sharedState.AddNonAbandonableEffectInstance(effect, m_updateSkillEffectInstance);
    }
  }
}
