using System;

namespace SkillSystem
{
  abstract class SkillEffectTriggerInstance<Trigger> : SkillTreeNonLeafNodeInstance<ISkillEffect, ISkillEffectInstance>, ISkillEffectTriggerInstance
    where Trigger : SkillTreeNonLeafNode<ISkillEffect>
  {
    protected Trigger TheTrigger
    {
      private set;
      get;
    }

    protected abstract bool ShouldTrigger(SkillRuntimeContext context);

    void DoInstantiateChildren(SkillRuntimeContext context)
    {
      InstantiateChildren(TheTrigger, context);
    }

    void ISkillEffectTriggerInstance.Update(SkillRuntimeContext context)
    {
      if (ShouldTrigger(context))
        DoInstantiateChildren(context);

      context.UpdateSkillEffectInstance(m_children);
    }

    void ISkillEffectTriggerInstance.Abandon(SkillRuntimeContext context)
    {
      m_children.ForEach(effect =>
      {
        if (!effect.Abandon())
          context.AddNonAbandonableEffect(effect);
      });
    }

    void ISkillEffectTriggerInstance.Pause()
    {
      m_children.ForEach(effect => effect.Pause());
    }

    void ISkillEffectTriggerInstance.Resume()
    {
      m_children.ForEach(effect => effect.Resume());
    }

    void IDisposable.Dispose()
    {
      m_children.ForEach(effect => effect.Dispose());
    }

    public SkillEffectTriggerInstance(Trigger trigger)
    {
      TheTrigger = trigger;
    }
  }
}
