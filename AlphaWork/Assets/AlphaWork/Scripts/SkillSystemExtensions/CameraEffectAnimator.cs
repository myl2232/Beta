using System;
using UnityEngine;

using SkillSystem.CameraShakePattern;

namespace SkillSystem
{
    [Effect]
    public class CameraEffectAnimator : SkillEffect
    {
        class Instance : SkillEffectInstance<CameraEffectAnimator>
        {
            LifetimeController.IInstance m_lifeTimeController;

            public GameObject CameraObj = null;
            public Animator animator = null;

            protected override bool DoUpdate(SkillRuntimeContext context)
            {
                return m_lifeTimeController.Update(context);
            }

            protected override void DoDestroy()
            {
                CameraObj.SetActive(false);
            }

            public Instance(CameraEffectAnimator effect, SkillRuntimeContext context)
              : base(effect)
            {
                /* CameraObj = Galaxy.GalaxyGameModule.GetGameManager<Galaxy.GalaxyCameraManager>().Camera_Skill;
                if (CameraObj != null)
                {
                    animator = CameraObj.GetComponent<Animator>();
                    if (animator != null)
                    {
                        CameraObj.SetActive(true);
                        CameraObj.transform.position = context.TheGameObject.transform.position;
                        CameraObj.transform.rotation = context.TheGameObject.transform.rotation;
                        animator.SetTrigger("camera001_" + context.TheSkill.Name);
                    }
                } */
 
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

        public CameraEffectAnimator()
        {
            LifetimeMode = new SerializableObject<LifetimeController.IController>(new LifetimeController.EndWhenAnimProgressReached());
            PlayerFilter = new SerializableObject<PlayerDifferentiator.IDifferentiator>(new PlayerDifferentiator.LocalPlayerOnly());
        }

        [Polymorphic(typeof(LifetimeController.EndWhenAnimProgressReached))]
        public SerializableObject<LifetimeController.IController> LifetimeMode
        {
            set;
            get;
        }
    }
}
