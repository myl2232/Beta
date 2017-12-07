using GameFramework.Event;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlphaWork
{
    public class BehaviourMove : MonoBehaviour
    {
        protected List<GameObject> Targets = new List<GameObject>();
        protected int m_nextTargetId = 0;
        protected EntityObject m_Parent;
        private MoveTarget m_moveTarget;

        public EntityObject Parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }

        public BehaviourMove()
        {
            
        }

        public void Start()
        {
            GameEntry.Event.Subscribe(MoveToTargetEventArgs.EventId, OnMoveToTarget);
            m_moveTarget = m_Parent.gameObject.AddComponent<MoveTarget>();
            FullfillTargets();
        }

        // Update is called once per frame
        public void Update()
        {
            if (m_nextTargetId == Targets.Count - 1)
            {
                m_nextTargetId = 0;
                return;
            }

            if (m_nextTargetId == 0)
            {
                GameEntry.Event.Fire(this, new MoveToTargetEventArgs(Targets[0].transform.position));
                m_nextTargetId++;
            }
            else if (Vector3.Distance(m_Parent.transform.position, Targets[m_nextTargetId - 1].transform.position) < 0.5f)
            {
                GameEntry.Event.Fire(this, new MoveToTargetEventArgs(Targets[m_nextTargetId].transform.position));
                m_nextTargetId++;
            }

        }


        public void FullfillTargets()
        {
            GameObject[] hings = FindObjectsOfType(typeof(GameObject)) as GameObject[];
            for (int i = 0; i < hings.Length; ++i)
            {
                TargetPoint pt = hings[i].GetComponent<TargetPoint>();
                if (pt)
                {
                    Targets.Add(hings[i]);
                }
            }
        }

        public void OnMoveToTarget(object sender, GameEventArgs e)
        {
            GameObject gb = GameEntry.Entity.GetEntity(m_Parent.Id).Handle as GameObject;
            MoveToTargetEventArgs mvArgs = e as MoveToTargetEventArgs;
            //start move new position
            MoveTarget ctl = m_Parent.GetComponentInParent<MoveTarget>();
            if (ctl)
                ctl.Move(gb.transform.position, mvArgs.MovePos);
            //start move state
            Animator animator = m_Parent.GetComponentInParent<Animator>();
            if (animator)
                animator.SetBool("Move", true);
        }
    }

}
