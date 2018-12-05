using GameFramework;
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
            gameObject.GetComponent<BaseCharacter>().ParentId = 0/*Id*/;
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected override void OnHide(object userData)
        {
            base.OnHide(userData);
            Destroy(this);
        }
        
        protected override void OnShow(object userdata)
        {
            base.OnShow(userdata);

            if(GameEntry.Config.GameSetting.gameContrller.MainActor == null)
            {
                GameEntry.Config.GameSetting.gameContrller.MainActor = Entity;
                GameObject gb = GameEntry.Config.GameSetting.gameContrller.MainActor.Handle as GameObject;
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

