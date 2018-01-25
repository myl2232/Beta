using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    //legecy格式的角色，动画和mesh在一起
    public class BarbarianCharacter : BaseCharacter
    {
        public Animation anim;
        public Rigidbody rbody;

        private void Start()
        {
            anim = GetComponent<Animation>();
            rbody = GetComponent<Rigidbody>();

            rbody.mass = 100;
        }

        private void Update()
        {
        }

        public void Move()
        {
            if (anim != null)
                anim.Play("walk");
        }

        public void Track()
        {
            if (anim != null)
                anim.Play("run");
        }
        public void Idle()
        {
            if (anim != null)
                anim.Play("idle");
        }

        public void Attack()
        {
            if (anim != null)
            {
                string strAttack = "attack" + UnityEngine.Random.Range(1, 3);
                anim.Play(strAttack);
            }
        }

        public override void SyncStatus(int status)
        {
            LogicStatus st = (LogicStatus)status;
            if (st == LogicStatus.ELogic_ATTACK)
                Attack();
            else if (st == LogicStatus.ELogic_PATROL)
                Move();
            else if (st == LogicStatus.ELogic_TRACK)
                Track();
            else if (st == LogicStatus.ELogic_IDLE)
                Idle();
        }

        public override void ActionAttack(float attackParam)
        {
        }
        public override void ActionPatrol(float speed)
        {
        }
        public override void ActionIdle()
        { }
        public override void ActionHurt()
        { }
        public override void ActionDead()
        { }
    }

}
