using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace AlphaWork
{
    public class Ethan : TargetableObject
	{
        // Use this for initialization
        void Start()
		{
            gameObject.GetOrAddComponent<BehaviourShakeHit>();
            gameObject.GetComponent<BaseCharacter>().ParentId = Id;
        }

        // Update is called once per frame
        void Update()
        {

        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnShow(object userdata)
#else
        protected internal override void OnShow(object userdata)
#endif
        {
            base.OnShow(userdata);

            if(GameBase.MainEthan == null)
            {
                GameBase.MainEthan = Entity;
                GameObject gb = GameBase.MainEthan.Handle as GameObject;
                gb.name = "MainEthan";

                //主相机要处理遮挡半透的情况
                OccTransparent oc = Camera.main.GetComponent<OccTransparent>();
                if(oc)
                {
                    oc.m_Hero = gb;
                }
            }

            AddDefeatCamp(CampType.Enemy);
            AddDefeatCamp(CampType.Enemy2);
        }

        private void LateUpdate()
        {

        }
        
        //public void OnShakeHit()
        //{
        //    GetComponent<BehaviourShakeHit>().OnShake();
        //}

    }
}

