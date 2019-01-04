using System;
using System.Collections.Generic;

using UnityEngine;

namespace SkillSystem
{
  [Effect]
  public class ParticleEffect : SkillEffect
  {
    class Instance : SkillEffectInstance<ParticleEffect>
    {
      AbstractAssetManager.IInstanceHandle<GameObject> m_rootObj;
      Func<bool> m_updateRootObjInternalState;
      TransformUpdater.IInstance m_transformUpdater;
      LifetimeController.IInstance m_lifetimeController;

      void UpdateTransform(SkillRuntimeContext context)
      {
        m_transformUpdater.UpdateTransform(m_rootObj.Instance, context);
      }

      protected override bool DoUpdate(SkillRuntimeContext context)
      {
        if (m_updateRootObjInternalState())
          return true;

        if (m_lifetimeController.Update(context))
          return true;

        UpdateTransform(context);
        return false;
      }

      protected override void DoPause()
      {
        foreach (var ps in m_rootObj.Instance.GetComponentsInChildren<ParticleSystem>())
          ps.Pause();
      }

      protected override void DoResume()
      {
        foreach (var ps in m_rootObj.Instance.GetComponentsInChildren<ParticleSystem>())
          ps.Play();
      }

      protected override void DoDestroy()
      {
        m_rootObj.Dispose();
      }

      static Func<bool> All(List<Func<bool>> fns)
      {
        return () =>
        {
          var result = true;
          foreach (var fn in fns)
            result = fn() && result;
          return result;
        };
      }

      static void MakeParticleSystemsUpdater(GameObject obj, List<Func<bool>> output)
      {
        var partSysUpdaters = new List<Func<bool>>();

        foreach (var ps in obj.GetComponentsInChildren<ParticleSystem>())
          partSysUpdaters.Add(GlobalObj<IParticleSystemUpdaterFactory>.Instance.MakeParticleSystemUpdater(ps));

        if (0 != partSysUpdaters.Count)
          output.Add(All(partSysUpdaters));
      }

      static void MakeAnimTimeChecker(GameObject obj, List<Func<bool>> output)
      {
        var maxTime = 0.0f;

        foreach (var animator in obj.GetComponentsInChildren<Animator>())
        {
          if (null != animator.runtimeAnimatorController)
          {
            var clips = animator.runtimeAnimatorController.animationClips;
            if (clips.Length == 1)
            {
              if (clips[0].isLooping)
              {
                output.Add(() => false);
                return;
              }

              maxTime = Mathf.Max(maxTime, clips[0].length);
            }
          }
        }

        if (0.0f != maxTime)
        {
          var timer = 0.0f;

          output.Add(() =>
          {
            if (timer >= maxTime)
              return true;

            timer += Time.deltaTime;
            return false;
          });
        }
      }

      static Func<bool> MakeInternalStateUpdater(GameObject obj)
      {
        var subStateUpdaters = new List<Func<bool>>();
        MakeParticleSystemsUpdater(obj, subStateUpdaters);
        MakeAnimTimeChecker(obj, subStateUpdaters);

        if (0 == subStateUpdaters.Count)
          return () => false;

        return All(subStateUpdaters);
      }

      public Instance(ParticleEffect particleEffect, SkillRuntimeContext context)
        : base(particleEffect)
      {
        var assetMgr = GlobalObj<AbstractAssetManager>.Instance;
        var prefab = assetMgr.LoadAsset<GameObject>(particleEffect.Prefab);
        m_rootObj = assetMgr.Instantiate(prefab);

        m_updateRootObjInternalState = MakeInternalStateUpdater(m_rootObj.Instance);

        m_transformUpdater = particleEffect.TransformMode.Obj.Instantiate(context);

        m_lifetimeController = particleEffect.LifetimeMode.Obj.Instantiate(context);

        UpdateTransform(context);
      }
    }

    bool RequestResource()
    {
      return GlobalObj<AbstractAssetManager>.Instance.Request(Prefab, typeof(GameObject));
    }

    protected override bool ShouldInstantiate(SkillRuntimeContext context)
    {
      return RequestResource();
    }

    protected override ISkillEffectInstance DoInstantiate(SkillRuntimeContext context)
    {
      return new Instance(this, context);
    }

    protected override void DoPreloadResource()
    {
      RequestResource();
    }

    public ParticleEffect()
    {
      Prefab = new AssetLocator();
      TransformMode = new SerializableObject<TransformUpdater.IUpdater>(new TransformUpdater.Inplace());
      LifetimeMode = new SerializableObject<LifetimeController.IController>(new LifetimeController.Free());
    }

    [Asset]
    public AssetLocator Prefab
    {
      set;
      get;
    }

    [Polymorphic(typeof(TransformUpdater.BindToBone),
      typeof(TransformUpdater.Inplace))]
    public SerializableObject<TransformUpdater.IUpdater> TransformMode
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
}
