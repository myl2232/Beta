using System;
using UnityEngine;

using UnityEngine.Playables;
using UnityEngine.Timeline;

using SkillSystem.CameraShakePattern;

namespace SkillSystem
{
    [Effect]
    public class CameraEffectTimeLine : SkillEffect
    {
        class Instance : SkillEffectInstance<CameraEffectTimeLine>
        {
            LifetimeController.IInstance m_lifeTimeController;

            public PlayableDirector director = null;
            public GameObject TimeLineTestRes = null;
            public GameObject TimeLineTest = null;

            protected override bool DoUpdate(SkillRuntimeContext context)
            {
                return m_lifeTimeController.Update(context);
            }

            protected override void DoDestroy()
            {
                if (director != null)
                {
                    director.Stop();
                }
               
                GameObject.Destroy(TimeLineTest);
            }

            public Instance(CameraEffectTimeLine effect, SkillRuntimeContext context)
              : base(effect)
            {
                m_lifeTimeController = effect.LifetimeMode.Obj.Instantiate(context);

                string strPathTimeLineTest = "Prefabs/TimeLine/" + context.TheSkill.Name;
                TimeLineTestRes = Resources.Load<GameObject>(strPathTimeLineTest);

                if (TimeLineTestRes == null) return;

                TimeLineTest = GameObject.Instantiate(TimeLineTestRes);
                TimeLineTest.transform.position = context.TheGameObject.transform.position;
                TimeLineTest.transform.rotation = context.TheGameObject.transform.rotation;

                Transform MainCamPosStart = TimeLineTest.transform.Find("CM vcam MainCamPosStart");
                if (MainCamPosStart != null)
                {
                    MainCamPosStart.position = Camera.main.transform.position;
                    MainCamPosStart.rotation = Camera.main.transform.rotation;
                }
                Transform MainCamPosEnd = TimeLineTest.transform.Find("CM vcam MainCamPosStop");
                if (MainCamPosEnd != null)
                {
                    MainCamPosEnd.position = Camera.main.transform.position;
                    MainCamPosEnd.rotation = Camera.main.transform.rotation;
                }

                director = TimeLineTest.GetComponent<PlayableDirector>();
                if (director != null)
                {
                    director.time = 0;
                    director.Evaluate();
                }
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

        public CameraEffectTimeLine()
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
