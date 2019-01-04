using System;
using UnityEngine;

namespace SkillSystem
{
  [Effect]
  public class SnapshotSequenceEffect : SkillEffect
  {
    class Instance : SkillEffectInstance<SnapshotSequenceEffect>
    {
      LifetimeController.IInstance m_lifetimeController;
      SnapshotSequence m_seq;
      Func<bool> m_doUpdate;

      public Instance(SnapshotSequenceEffect effect, SkillRuntimeContext context)
        : base(effect)
      {
        m_lifetimeController = effect.GenerationEndMode.Obj.Instantiate(context);

        var p = new SnapshotSequence.Parameters
        {
          TheGameObject = context.TheGameObject,
          SnapshotMaterial = GlobalObj<AbstractAssetManager>.Instance.LoadAsset<Material>(effect.SnapshotMaterial),
          SnapshotLifetime = effect.SnapshotLifetime,
          SpacingMode = effect.SpacingMode.Obj.Instantiate()
        };

        m_seq = new SnapshotSequence(p);

        m_doUpdate = () =>
        {
          m_seq.Update();

          if (m_lifetimeController.Update(context))
          {
            m_seq.Stop();
            m_doUpdate = m_seq.Update;
          }

          return false;
        };
      }

      protected override bool DoUpdate(SkillRuntimeContext context)
      {
        return m_doUpdate();
      }

      protected override void DoDestroy()
      {
        m_seq.Dispose();
      }
    }

    protected override bool ShouldInstantiate(SkillRuntimeContext context)
    {
      return GlobalObj<AbstractAssetManager>.Instance.Request(SnapshotMaterial, typeof(Material));
    }

    protected override ISkillEffectInstance DoInstantiate(SkillRuntimeContext context)
    {
      return new Instance(this, context);
    }

    public SnapshotSequenceEffect()
    {
      SnapshotMaterial = new AssetLocator();
      GenerationEndMode = new SerializableObject<LifetimeController.IController>(new LifetimeController.EndWhenAnimProgressReached());
      SpacingMode = new SerializableObject<SnapshotSequenceSpacingMode.IMode>(new SnapshotSequenceSpacingMode.ByTimeInterval());
    }

    [Asset(typeof(Material))]
    public AssetLocator SnapshotMaterial
    {
      set;
      get;
    }

    float m_snapshotLifetime = 2.0f;
    public float SnapshotLifetime
    {
      set
      {
        m_snapshotLifetime = Mathf.Max(0.0f, value);
      }

      get
      {
        return m_snapshotLifetime;
      }
    }

    [Polymorphic(typeof(SnapshotSequenceSpacingMode.ByTimeInterval))]
    public SerializableObject<SnapshotSequenceSpacingMode.IMode> SpacingMode
    {
      set;
      get;
    }

    [Polymorphic(typeof(LifetimeController.EndWhenAnimProgressReached),
      typeof(LifetimeController.EndAfterSpecifiedTime))]
    public SerializableObject<LifetimeController.IController> GenerationEndMode
    {
      set;
      get;
    }
  }

  namespace SnapshotSequenceSpacingMode
  {
    public interface IMode
    {
      SnapshotSequence.ISpacingMode Instantiate();
    }

    public class ByTimeInterval : IMode
    {
      SnapshotSequence.ISpacingMode IMode.Instantiate()
      {
        return new SnapshotSequenceSpacingModeImpl.ByTimeInterval(Interval);
      }

      float m_interval = 0.3f;
      public float Interval
      {
        set
        {
          m_interval = Mathf.Max(0.0f, value);
        }

        get
        {
          return m_interval;
        }
      }
    }
  }
}
