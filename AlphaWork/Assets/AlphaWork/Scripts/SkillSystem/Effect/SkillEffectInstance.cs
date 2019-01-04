using System;

namespace SkillSystem
{
  abstract class SkillEffectInstance<Effect> : ISkillEffectInstance
    where Effect : SkillEffect
  {
    protected Effect TheEffect
    {
      private set;
      get;
    }

    protected abstract bool DoUpdate(SkillRuntimeContext context);
    protected abstract void DoDestroy();

    protected virtual void DoPause()
    {

    }

    protected virtual void DoResume()
    {

    }

    bool ISkillEffectInstance.Update(SkillRuntimeContext context)
    {
      if (DoUpdate(context))
      {
        DoDestroy();
        return true;
      }

      return false;
    }

    bool ISkillEffectInstance.Abandon()
    {
      if (TheEffect.StopWhenSkillStopped)
      {
        DoDestroy();
        return true;
      }

      return false;
    }

    void ISkillEffectInstance.Pause()
    {
      DoPause();
    }

    void ISkillEffectInstance.Resume()
    {
      DoResume();
    }

    void IDisposable.Dispose()
    {
      DoDestroy();
    }

    public SkillEffectInstance(Effect effect)
    {
      TheEffect = effect;
    }
  }
}
