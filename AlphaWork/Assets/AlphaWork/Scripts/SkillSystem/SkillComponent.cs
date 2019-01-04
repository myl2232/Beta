using System;
using System.Collections.Generic;

using UnityEngine;

namespace SkillSystem
{
  [ExecuteInEditMode]
  public class SkillComponent : MonoBehaviour
  {
    static SkillComponent()
    {
      GlobalObj<IParticleSystemUpdaterFactory>.Instance = new PlayModeParticleSystemUpdaterFactory();
      GlobalObj<UpdateEventForwarder>.Instance = new UpdateEventForwarder();
      GlobalObj<CameraShakeManager>.Instance = new CameraShakeManager();
      GlobalObj<SkillDataCache>.Instance = new SkillDataCache();
    }

    static void SkillSystemUpdate()
    {
      GlobalObj<UpdateEventForwarder>.Instance.Update();
    }

    public static void InEditorUpdate()
    {
      SkillSystemUpdate();
    }

    public static void Init(AbstractAssetManager assetManager, ILocalPlayerDifferentiator localPlayerDifferentiator)
    {
      if (!Application.isEditor)
        GlobalObj<AbstractAssetManager>.Instance = assetManager;

      GlobalObj<ILocalPlayerDifferentiator>.Instance = localPlayerDifferentiator;
    }

    public static void InGameUpdate()
    {
      if (!Application.isEditor)
        SkillSystemUpdate();
    }

    public static void Preload(string skillFilePath)
    {
      GlobalObj<SkillDataCache>.Instance.Load(skillFilePath, skill =>
      {
        skill.Traverse(0, (depth, node) =>
        {
          var effect = node as ISkillEffect;
          if (null != effect)
            effect.PreloadResources();
        });
      });
    }

    SkillRuntimeContext.SharedState m_contextSharedState;
    List<SkillInstanceSlot> m_skillInstances;
    bool m_paused;

    class SkillInstanceSlot : IDisposable
    {
      SkillRuntimeContext m_context;
      Skill.Instance m_skillInstance;

      public SkillInstanceSlot(Skill skill,
        SkillRuntimeContext.SharedState sharedState,
        AnimationProgressQuery.IQuery query)
      {
        m_context = new SkillRuntimeContext(sharedState, query, skill);
        m_skillInstance = skill.Instantiate(m_context);
      }

      public void Update()
      {
        m_skillInstance.Update(m_context);
      }

      public void Abandon()
      {
        m_skillInstance.Abandon(m_context);
      }

      public void Dispose()
      {
        m_skillInstance.Dispose();
      }

      public void Pause()
      {
        m_skillInstance.Pause();
      }

      public void Resume()
      {
        m_skillInstance.Resume();
      }
    }

    void PauseFrameEvent()
    {

    }

    void OnEnable()
    {
      m_contextSharedState = new SkillRuntimeContext.SharedState(gameObject);
      m_skillInstances = new List<SkillInstanceSlot>();
    }

    void Update()
    {
      if (m_paused)
        return;

      m_contextSharedState.Update();

      foreach (var inst in m_skillInstances)
        inst.Update();
    }

    void OnDisable()
    {
      Resume();

      m_contextSharedState.Dispose();
      m_contextSharedState = null;

      m_skillInstances.ForEach(inst => inst.Dispose());
      m_skillInstances = null;
    }

    class SkillInstanceHandle : IDisposable
    {
      SkillInstanceSlot m_slot;
      SkillComponent m_skillComponent;

      public SkillInstanceHandle(SkillInstanceSlot slot, SkillComponent skillComponent)
      {
        m_slot = slot;
        m_skillComponent = skillComponent;
      }

      void IDisposable.Dispose()
      {
        if (m_skillComponent)
        {
          var instances = m_skillComponent.m_skillInstances;
          if (null != instances)
          {
            var i = instances.IndexOf(m_slot);
            if (-1 != i)
            {
              m_slot.Abandon();
              instances.RemoveAt(i);
            }
          }
        }
      }
    }

    class NullAnimProgressQuery : AnimationProgressQuery.IQuery
    {
      public float GetProgress()
      {
        return 0.0f;
      }
    }

    public IDisposable Play(Skill skill, AnimationProgressQuery.IQuery query)
    {
      var slot = new SkillInstanceSlot(skill, m_contextSharedState, query);
      m_skillInstances.Add(slot);
      return new SkillInstanceHandle(slot, this);
    }

    public IDisposable Play(string skillFile, string skillName, AnimationProgressQuery.IQuery query)
    {
      return Play(GlobalObj<SkillDataCache>.Instance.GetSkill(skillFile, skillName), query);
    }

    public IDisposable Play(string skillFile, string skillName)
    {
      return Play(skillFile, skillName, new NullAnimProgressQuery());
    }

    public void Pause()
    {
      if (!m_paused)
      {
        m_paused = true;

        foreach (var inst in m_skillInstances)
          inst.Pause();

        m_contextSharedState.Pause();

        if (null != OnPaused)
          OnPaused();
      }
    }

    public void Resume()
    {
      if (m_paused)
      {
        m_paused = false;

        foreach (var inst in m_skillInstances)
          inst.Resume();

        m_contextSharedState.Resume();

        if (null != OnResumed)
          OnResumed();
      }
    }

    public event Action OnPaused;
    public event Action OnResumed;

    public void StopAll()
    {
      if (null != m_skillInstances)
      {
        m_skillInstances.ForEach(inst => inst.Abandon());
        m_skillInstances.Clear();
      }
    }
  }
}
