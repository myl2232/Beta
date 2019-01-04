using Galaxy;

namespace SkillSystem
{
  [Effect]
  public class SoundEffect : SkillEffect
  {
    class Instance : SkillEffectInstance<SoundEffect>
    {
      LifetimeController.IInstance m_lifetimeController;

      protected override bool DoUpdate(SkillRuntimeContext context)
      {
        return m_lifetimeController.Update(context);
      }

      protected override void DoDestroy()
      {
        //GalaxyGameModule.GetGameManager<AudioManager>().StopAudio(TheEffect.SoundId);
      }

      public Instance(SoundEffect effect, SkillRuntimeContext context)
        : base(effect)
      {
        m_lifetimeController = effect.LifetimeMode.Obj.Instantiate(context);

        effect.PlayMode.Obj.Play(effect, context);
      }
    }

    protected override bool ShouldInstantiate(SkillRuntimeContext context)
    {
      return true;
    }

    protected override ISkillEffectInstance DoInstantiate(SkillRuntimeContext context)
    {
      return new Instance(this, context);
    }

    public SoundEffect()
    {
      LifetimeMode = new SerializableObject<LifetimeController.IController>(new LifetimeController.Free());

      PlayMode = new SerializableObject<SoundPlayMode.IMode>(new SoundPlayMode.Mode3D());
    }

    public int SoundId
    {
      set;
      get;
    }

    [Polymorphic(typeof(SoundPlayMode.Mode3D),
      typeof(SoundPlayMode.Mode2D))]
    public SerializableObject<SoundPlayMode.IMode> PlayMode
    {
      set;
      get;
    }

    [Polymorphic(typeof(LifetimeController.Free),
      typeof(LifetimeController.EndWhenAnimProgressReached),
      typeof(LifetimeController.EndAfterSpecifiedTime))]
    public SerializableObject<LifetimeController.IController> LifetimeMode
    {
      set;
      get;
    }
  }

  namespace SoundPlayMode
  {
    public interface IMode
    {
      void Play(SoundEffect effect, SkillRuntimeContext context);
    }

    public class Mode3D : IMode
    {
      void IMode.Play(SoundEffect effect, SkillRuntimeContext context)
      {
        //GalaxyGameModule.GetGameManager<AudioManager>().Play3DAudio(effect.SoundId, context.TheGameObject);
      }
    }

    public class Mode2D : IMode
    {
      void IMode.Play(SoundEffect effect, SkillRuntimeContext context)
      {
        //GalaxyGameModule.GetGameManager<AudioManager>().Play2DAudio(effect.SoundId);
      }
    }
  }
}
