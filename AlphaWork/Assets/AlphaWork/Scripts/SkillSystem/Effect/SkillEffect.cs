using System;

namespace SkillSystem
{
  public abstract class SkillEffect : SkillTreeNode, ISkillEffect
  {
    class NullInstance : ISkillEffectInstance
    {
      bool ISkillEffectInstance.Update(SkillRuntimeContext context)
      {
        return true;
      }

      bool ISkillEffectInstance.Abandon()
      {
        return true;
      }

      void IDisposable.Dispose()
      {

      }

      void ISkillEffectInstance.Pause()
      {

      }

      void ISkillEffectInstance.Resume()
      {

      }
    }

    protected abstract bool ShouldInstantiate(SkillRuntimeContext context);
    protected abstract ISkillEffectInstance DoInstantiate(SkillRuntimeContext context);
    protected virtual void DoPreloadResource()
    {

    }

    public ISkillEffectInstance Instantiate(SkillRuntimeContext context)
    {
      if (PlayerFilter.Obj.Check(context.TheGameObject) && ShouldInstantiate(context))
        return DoInstantiate(context);

      return new NullInstance();
    }

    void ISkillEffect.PreloadResources()
    {
      DoPreloadResource();
    }

    public SkillEffect()
    {
      PlayerFilter = new SerializableObject<PlayerDifferentiator.IDifferentiator>(new PlayerDifferentiator.AllPlayers());
    }

    [Polymorphic(typeof(PlayerDifferentiator.AllPlayers),
      typeof(PlayerDifferentiator.LocalPlayerOnly),
      typeof(PlayerDifferentiator.NonLocalPlayersOnly))]
    public SerializableObject<PlayerDifferentiator.IDifferentiator> PlayerFilter
    {
      set;
      get;
    }

    public bool StopWhenSkillStopped
    {
      set;
      get;
    }
  }
}
