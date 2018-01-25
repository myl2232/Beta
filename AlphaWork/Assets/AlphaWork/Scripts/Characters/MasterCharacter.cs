using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public class MasterCharacter : BaseCharacter
    {
        public Animator m_pAnimator;
        public Rigidbody rbody;
        private LogicStatus m_animStatus;

        void Start()
        {
            m_pAnimator = GetComponent<Animator>();
            rbody = GetComponent<Rigidbody>();           
        }

        public override void SyncStatus(int status)
        {
            m_animStatus = (LogicStatus)status;
            m_pAnimator.SetInteger("status", status);
        }
        
        public override void ActionAttack(float attackParam)
        {
            m_pAnimator.SetFloat("BlendAttack", attackParam);
            m_pAnimator.SetTrigger("Attack");
        }

        public override void ActionPatrol(float speed)
        {
            m_pAnimator.SetFloat("MotionBlend", speed);
        }
        public override void ActionIdle()
        {
        }
        public override void ActionHurt()
        { }
        public override void ActionDead()
        { }

        public override bool CheckActionEnd()
        {
            AnimatorStateInfo animatorInfo;
            animatorInfo = m_pAnimator.GetCurrentAnimatorStateInfo(0);
            if (animatorInfo.normalizedTime >= 1.0f)
                return true;
            else
                return false;
        }
    }
}
