using System;
using UnityEngine;

using SkillSystem.CameraShakePattern;

namespace SkillSystem
{
  [Effect]
  public class CameraShakeEffectSHM : SkillEffect
  {
    class Instance : SkillEffectInstance<CameraShakeEffectSHM>
    {
      LifetimeController.IInstance m_lifeTimeController;
      IDisposable m_instHandle;

      protected override bool DoUpdate(SkillRuntimeContext context)
      {
        return m_lifeTimeController.Update(context);
      }

      protected override void DoDestroy()
      {
        m_instHandle.Dispose();
      }

      public Instance(CameraShakeEffectSHM effect, SkillRuntimeContext context)
        : base(effect)
      {
        m_lifeTimeController = effect.LifetimeMode.Obj.Instantiate(context);

        Func<float, float, IPattern> makeShakePattern = (amp, freq) =>
        {
          return effect.Damping.Obj.Make(new SHM(amp, freq));
        };

        m_instHandle = GlobalObj<CameraShakeManager>.Instance.MakeEffect(builder =>
        {
          builder.BuildPositionalShake(() => effect.Space.Obj.XAxis(context), makeShakePattern(effect.PositionAmplitude.x, effect.Frequency));
          builder.BuildPositionalShake(() => effect.Space.Obj.YAxis(context), makeShakePattern(effect.PositionAmplitude.y, effect.Frequency));
          builder.BuildPositionalShake(() => effect.Space.Obj.ZAxis(context), makeShakePattern(effect.PositionAmplitude.z, effect.Frequency));

          builder.BuildRotationalShake(() => effect.Space.Obj.XAxis(context), makeShakePattern(effect.RotationAmplitude.x, effect.Frequency));
          builder.BuildRotationalShake(() => effect.Space.Obj.YAxis(context), makeShakePattern(effect.RotationAmplitude.y, effect.Frequency));
          builder.BuildRotationalShake(() => effect.Space.Obj.ZAxis(context), makeShakePattern(effect.RotationAmplitude.z, effect.Frequency));
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

    public CameraShakeEffectSHM()
    {
      Damping = new SerializableObject<CameraShakeDamping.IDamping>(new CameraShakeDamping.Undamped());

      LifetimeMode = new SerializableObject<LifetimeController.IController>(
        new LifetimeController.EndWhenAnimProgressReached());

      Space = new SerializableObject<CoordinateSystem.ISpace>(new CoordinateSystem.CurrentCamera());
    }

    public Vector3 PositionAmplitude
    {
      set;
      get;
    }

    public Vector3 RotationAmplitude
    {
      set;
      get;
    }

    public float Frequency
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
}
