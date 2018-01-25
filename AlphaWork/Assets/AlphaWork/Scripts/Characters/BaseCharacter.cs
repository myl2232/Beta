using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public class BaseCharacter : MonoBehaviour
    {
        private void Awake()
        {
            GameEntry.Event.Subscribe(UIAttack1EventArgs.EventId, OnAttack1);
            GameEntry.Event.Subscribe(UIAttack2EventArgs.EventId, OnAttack2);
            GameEntry.Event.Subscribe(UIAlphaEventArgs.EventId, OnKick1);
            GameEntry.Event.Subscribe(UIBetaEventArgs.EventId, OnKick2);
        }

        protected virtual void OnAttack1(object sender, GameEventArgs arg)
        {
            //GameObject gb = GameBase.MainEthan.Handle as GameObject;
            //RPGCharacterControllerFREE ctl = gb.GetComponent<RPGCharacterControllerFREE>();
            //if (ctl)
            //{
            //    ctl.inputAttackL = true;
            //}
        }

        protected virtual void OnAttack2(object sender, GameEventArgs arg)
        {
            //GameObject gb = GameBase.MainEthan.Handle as GameObject;
            //RPGCharacterControllerFREE ctl = gb.GetComponent<RPGCharacterControllerFREE>();
            //if (ctl)
            //{
            //    ctl.inputAttackR = true;
            //}
        }

        protected virtual void OnKick1(object sender, GameEventArgs arg)
        {
            //GameObject gb = GameBase.MainEthan.Handle as GameObject;
            //RPGCharacterControllerFREE ctl = gb.GetComponent<RPGCharacterControllerFREE>();
            //if (ctl)
            //{
            //    ctl.inputCastL = true;
            //}
        }

        protected virtual void OnKick2(object sender, GameEventArgs arg)
        {
            //GameObject gb = GameBase.MainEthan.Handle as GameObject;
            //RPGCharacterControllerFREE ctl = gb.GetComponent<RPGCharacterControllerFREE>();
            //if (ctl)
            //{
            //    ctl.inputCastR = true;
            //}
        }

        public virtual bool CheckActionEnd()
        {
            return false;
        }

        public virtual void SyncStatus(int status)
        { }
        public virtual void ActionAttack(float attackParam)
        { }
        public virtual void ActionPatrol(float speed)
        { }
        public virtual void ActionIdle()
        { }
        public virtual void ActionHurt()
        { }
        public virtual void ActionDead()
        { }
        public virtual void Move(Vector3 move, bool crouch, bool jump)
        {
        }
    }
}
