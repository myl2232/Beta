using System;
using UnityEngine;

namespace SkillSystem
{
  [Effect]
  public class CameraShakeEffect : SkillEffect
  {
    class Instance : SkillEffectInstance<CameraShakeEffect>
    {
      LifetimeController.IInstance m_lifetimeController;
      IDisposable m_instHandle;

      protected override bool DoUpdate(SkillRuntimeContext context)
      {
        return m_lifetimeController.Update(context);
      }

      protected override void DoDestroy()
      {
        m_instHandle.Dispose();
      }

      public Instance(CameraShakeEffect effect, SkillRuntimeContext context)
        : base(effect)
      {
        m_lifetimeController = effect.LifetimeMode.Obj.Instantiate(context);

        m_instHandle = GlobalObj<CameraShakeManager>.Instance.MakeEffect(builder =>
        {
          effect.PositionX.Obj.Instantiate(() => effect.Space.Obj.XAxis(context), builder.BuildPositionalShake, effect.Damping.Obj);
          effect.PositionY.Obj.Instantiate(() => effect.Space.Obj.YAxis(context), builder.BuildPositionalShake, effect.Damping.Obj);
          effect.PositionZ.Obj.Instantiate(() => effect.Space.Obj.ZAxis(context), builder.BuildPositionalShake, effect.Damping.Obj);

          effect.RotationX.Obj.Instantiate(() => effect.Space.Obj.XAxis(context), builder.BuildRotationalShake, effect.Damping.Obj);
          effect.RotationY.Obj.Instantiate(() => effect.Space.Obj.YAxis(context), builder.BuildRotationalShake, effect.Damping.Obj);
          effect.RotationZ.Obj.Instantiate(() => effect.Space.Obj.ZAxis(context), builder.BuildRotationalShake, effect.Damping.Obj);
        });
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

    public CameraShakeEffect()
    {
      Damping = new SerializableObject<CameraShakeDamping.IDamping>(new CameraShakeDamping.Undamped());

      LifetimeMode = new SerializableObject<LifetimeController.IController>(new LifetimeController.EndWhenAnimProgressReached());

      Space = new SerializableObject<CoordinateSystem.ISpace>(new CoordinateSystem.CurrentCamera());

      Func<SerializableObject<CameraShakeMode.IMode>> makeNoShake =
        () => new SerializableObject<CameraShakeMode.IMode>(new CameraShakeMode.NoShake());

      PositionX = makeNoShake();
      PositionY = makeNoShake();
      PositionZ = makeNoShake();

      RotationX = makeNoShake();
      RotationY = makeNoShake();
      RotationZ = makeNoShake();
    }

    [Polymorphic(typeof(CameraShakeMode.NoShake), typeof(CameraShakeMode.SimpleHarmonicMotion))]
    public SerializableObject<CameraShakeMode.IMode> PositionX
    {
      set;
      get;
    }

    [Polymorphic(typeof(CameraShakeMode.NoShake), typeof(CameraShakeMode.SimpleHarmonicMotion))]
    public SerializableObject<CameraShakeMode.IMode> PositionY
    {
      set;
      get;
    }

    [Polymorphic(typeof(CameraShakeMode.NoShake), typeof(CameraShakeMode.SimpleHarmonicMotion))]
    public SerializableObject<CameraShakeMode.IMode> PositionZ
    {
      set;
      get;
    }

    [Polymorphic(typeof(CameraShakeMode.NoShake), typeof(CameraShakeMode.SimpleHarmonicMotion))]
    public SerializableObject<CameraShakeMode.IMode> RotationX
    {
      set;
      get;
    }

    [Polymorphic(typeof(CameraShakeMode.NoShake), typeof(CameraShakeMode.SimpleHarmonicMotion))]
    public SerializableObject<CameraShakeMode.IMode> RotationY
    {
      set;
      get;
    }

    [Polymorphic(typeof(CameraShakeMode.NoShake), typeof(CameraShakeMode.SimpleHarmonicMotion))]
    public SerializableObject<CameraShakeMode.IMode> RotationZ
    {
      set;
      get;
    }

    [Polymorphic(typeof(CameraShakeDamping.Undamped), typeof(CameraShakeDamping.Linear))]
    public SerializableObject<CameraShakeDamping.IDamping> Damping
    {
      set;
      get;
    }

    [Polymorphic(typeof(LifetimeController.EndWhenAnimProgressReached),
      typeof(LifetimeController.EndAfterSpecifiedTime))]
    public SerializableObject<LifetimeController.IController> LifetimeMode
    {
      set;
      get;
    }

    [Polymorphic(typeof(CoordinateSystem.CurrentCamera),
     typeof(CoordinateSystem.World),
     typeof(CoordinateSystem.CurrentGameObject))]
    public SerializableObject<CoordinateSystem.ISpace> Space
    {
      set;
      get;
    }
  }

  namespace CameraShakeMode
  {
    public interface IMode
    {
      void Instantiate(Func<Vector3> axisGetter,
        Action<Func<Vector3>, CameraShakePattern.IPattern> builder,
        CameraShakeDamping.IDamping damping);
    }

    public class NoShake : IMode
    {
      void IMode.Instantiate(Func<Vector3> axisGetter,
        Action<Func<Vector3>, CameraShakePattern.IPattern> builder,
        CameraShakeDamping.IDamping damping)
      {

      }
    }

    public class SimpleHarmonicMotion : IMode
    {
      void IMode.Instantiate(Func<Vector3> axisGetter,
        Action<Func<Vector3>, CameraShakePattern.IPattern> builder,
        CameraShakeDamping.IDamping damping)
      {
        builder(axisGetter, damping.Make(new CameraShakePattern.SHM(Amplitude, Frequency)));
      }

      public float Amplitude
      {
        set;
        get;
      }

      public float Frequency
      {
        set;
        get;
      }
    }
  }
}
