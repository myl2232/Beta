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
            GameEntry.Event.Subscribe(UIAttack1EventArgs.EventId, OnAttack1);
            GameEntry.Event.Subscribe(UIAttack2EventArgs.EventId, OnAttack2);
            GameEntry.Event.Subscribe(UIAlphaEventArgs.EventId, OnKick1);
            GameEntry.Event.Subscribe(UIBetaEventArgs.EventId, OnKick2);
        }

        // Update is called once per frame
        void Update()
		{

        }

        protected internal override void OnShow(object userdata)
        {
            base.OnShow(userdata);

            if(GameBase.MainEthan == null)
            {
                Vector3 offset = new Vector3(8, 5, 0);
                Camera.main.transform.position = Entity.transform.position + offset;
                Camera.main.transform.LookAt(Entity.transform);
                GameBase.MainEthan = Entity;
                GameObject gb = GameBase.MainEthan.Handle as GameObject;
                gb.name = "MainEthan";
                //RPGChr作为主角的情况
                RPGCharacterControllerFREE ctl = gb.GetComponent<RPGCharacterControllerFREE>();
                if (ctl)
                {
                    ctl.sceneCamera = Camera.main;
                    ctl.runSpeed = 12;
                }
            }
        }

        private void LateUpdate()
        {

        }
        
        public void OnHit()
        {
            GetComponent<BehaviourShakeHit>().OnShake();
        }

        void OnAttack1(object sender, GameEventArgs arg)
        {
            GameObject gb = GameBase.MainEthan.Handle as GameObject;
            RPGCharacterControllerFREE ctl = gb.GetComponent<RPGCharacterControllerFREE>();
            if (ctl)
            {
                ctl.inputAttackL = true;
            }
        }
        void OnAttack2(object sender, GameEventArgs arg)
        {
            GameObject gb = GameBase.MainEthan.Handle as GameObject;
            RPGCharacterControllerFREE ctl = gb.GetComponent<RPGCharacterControllerFREE>();
            if (ctl)
            {
                ctl.inputAttackR = true;
            }
        }
        void OnKick1(object sender, GameEventArgs arg)
        {
            GameObject gb = GameBase.MainEthan.Handle as GameObject;
            RPGCharacterControllerFREE ctl = gb.GetComponent<RPGCharacterControllerFREE>();
            if (ctl)
            {
                ctl.inputCastL = true;
            }
        }
        void OnKick2(object sender, GameEventArgs arg)
        {
            GameObject gb = GameBase.MainEthan.Handle as GameObject;
            RPGCharacterControllerFREE ctl = gb.GetComponent<RPGCharacterControllerFREE>();
            if (ctl)
            {
                ctl.inputCastR = true;
            }
        }
        protected override void OnDead()
        {
            GameObject gb = GameBase.MainEthan.Handle as GameObject;
            RPGCharacterControllerFREE ctl = gb.GetComponent<RPGCharacterControllerFREE>();
            if (ctl)
            {
                ctl.inputDeath = true;
            }
            else
            {
                Animator gAnimator = Entity.gameObject.GetComponent<Animator>();
                if (gAnimator)
                {
                    gAnimator.SetBool("Dead", true);
                }
            }
        }

        protected override void OnHurt()
        {
            GameObject gb = GameBase.MainEthan.Handle as GameObject;
            RPGCharacterControllerFREE ctl = gb.GetComponent<RPGCharacterControllerFREE>();
            if (ctl)
            {
                ctl.inputLightHit = true;
            }
            else
            {
                Animator gAnimator = Entity.gameObject.GetComponent<Animator>();
                if (gAnimator)
                {

                }
            }
        }
        //public override ImpactData GetImpactData()
        //{
        //    throw new System.NotImplementedException();
        //}
    }
}

