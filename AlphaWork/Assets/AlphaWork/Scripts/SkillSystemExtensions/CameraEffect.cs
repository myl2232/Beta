using System;
using UnityEngine;

using SkillSystem.CameraShakePattern;

namespace SkillSystem
{
    [Effect]
    public class CameraEffect : SkillEffect
    {
        class Instance : SkillEffectInstance<CameraEffect>
        {
            LifetimeController.IInstance m_lifeTimeController;

            public GameObject CameraObj = null;
            TransformUpdater.IInstance m_transformUpdater;
            protected override bool DoUpdate(SkillRuntimeContext context)
            {
                m_transformUpdater.UpdateTransform(CameraObj, context);
                return m_lifeTimeController.Update(context);
            }

            protected override void DoDestroy()
            {
                CameraObj.SetActive(false);
                GameObject.DestroyImmediate(CameraObj);
            }

            public Instance(CameraEffect effect, SkillRuntimeContext context)
              : base(effect)
            {
                CameraObj = new GameObject("SkillCameraObj");
                CameraObj.AddComponent<Camera>();
                CameraObj.SetActive(true);
                m_transformUpdater = effect.TransformMode.Obj.Instantiate(context);
                m_lifeTimeController = effect.LifetimeMode.Obj.Instantiate(context);

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

        public CameraEffect()
        {
            TransformMode = new SerializableObject<TransformUpdater.IUpdater>(new TransformUpdater.BindToBone());
            LifetimeMode = new SerializableObject<LifetimeController.IController>(new LifetimeController.EndWhenAnimProgressReached());
            PlayerFilter = new SerializableObject<PlayerDifferentiator.IDifferentiator>(new PlayerDifferentiator.LocalPlayerOnly());
        }

        [Polymorphic(typeof(TransformUpdater.BindToBone))]
        public SerializableObject<TransformUpdater.IUpdater> TransformMode
        {
            set;
            get;
        }

        [Polymorphic(typeof(LifetimeController.EndWhenAnimProgressReached))]
        public SerializableObject<LifetimeController.IController> LifetimeMode
        {
            set;
            get;
        }
    }
}
