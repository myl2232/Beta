using System;
using System.Collections.Generic;

namespace SkillSystem
{
  class NonAbandonableEffectInstances : IDisposable
  {
    class Slot : IDisposable
    {
      ISkillEffectInstance m_instance;
      Predicate<ISkillEffectInstance> m_updateSkillEffectInstance;

      public Slot(ISkillEffectInstance inst, Predicate<ISkillEffectInstance> updateSkillEffectInstance)
      {
        m_instance = inst;
        m_updateSkillEffectInstance = updateSkillEffectInstance;
      }

      public bool Update()
      {
        return m_updateSkillEffectInstance(m_instance);
      }

      public void Dispose()
      {
        m_instance.Dispose();
      }

      public void Pause()
      {
        m_instance.Pause();
      }

      public void Resume()
      {
        m_instance.Resume();
      }
    }

    List<Slot> m_instances = new List<Slot>();
    Predicate<Slot> m_updateSlot = slot => slot.Update();

    public void Add(ISkillEffectInstance effect, Predicate<ISkillEffectInstance> updateSkillEffectInstance)
    {
      m_instances.Add(new Slot(effect, updateSkillEffectInstance));
    }

    public void Update()
    {
      m_instances.RemoveAll(m_updateSlot);
    }

    public void Pause()
    {
      m_instances.ForEach(inst => inst.Pause());
    }

    public void Resume()
    {
      m_instances.ForEach(inst => inst.Resume());
    }

    public void Dispose()
    {
      m_instances.ForEach(inst => inst.Dispose());
    }
  }
}
