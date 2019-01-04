using System;
using UnityEngine;

namespace SkillSystem
{
  public class Skill : SkillTreeNonLeafNode<ISkillEffectTrigger>
  {
    public class Instance : SkillTreeNonLeafNodeInstance<ISkillEffectTrigger, ISkillEffectTriggerInstance>, IDisposable
    {
      public void Update(SkillRuntimeContext context)
      {
        foreach (var child in m_children)
          child.Update(context);
      }

      public void Abandon(SkillRuntimeContext context)
      {
        m_children.ForEach(triggerInst => triggerInst.Abandon(context));
      }

      public void Dispose()
      {
        m_children.ForEach(triggerInst => triggerInst.Dispose());
      }

      public Instance(Skill skill, SkillRuntimeContext context)
      {
        InstantiateChildren(skill, context);
      }

      public void Pause()
      {
        m_children.ForEach(triggerInst => triggerInst.Pause());
      }

      public void Resume()
      {
        m_children.ForEach(triggerInst => triggerInst.Resume());
      }
    }

    public Instance Instantiate(SkillRuntimeContext context)
    {
      return new Instance(this, context);
    }
  }
}
