using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace AlphaWork
{
    public class Enemy: NPC
    {
        protected List<GameObject> Targets = new List<GameObject>();
        protected int m_nextTargetId = 0;
        private BehaviourMove m_move;
        // Use this for initialization
        void Start()
        {
            m_move = gameObject.AddComponent<BehaviourMove>();
            m_move.Parent = this;
            
            //GameEntry.Event.Subscribe(MoveToTargetEventArgs.EventId, OnMoveToTarget);
        }

        // Update is called once per frame
        void Update()
        { 
//             if (m_nextTargetId == Targets.Count - 1)
//             {
//                 m_nextTargetId = 0;
//                 return;
//             }
// 
//             if (m_nextTargetId == 0)
//             {
//                 GameEntry.Event.Fire(this, new MoveToTargetEventArgs(Targets[0].transform.position));
//                 m_nextTargetId++;
//             }
//             else if (Vector3.Distance(transform.position, Targets[m_nextTargetId-1].transform.position) < 0.5f)
//             {
//                 GameEntry.Event.Fire(this, new MoveToTargetEventArgs(Targets[m_nextTargetId].transform.position));
//                 m_nextTargetId++;
//             }
            
        }

        protected internal override void OnShow(object userdata)
        {
            base.OnShow(userdata);
            
            //FullfillTargets();
        }

//         protected void FullfillTargets()
//         {
//             GameObject[] hings = FindObjectsOfType(typeof(GameObject)) as GameObject[];
//             for (int i = 0; i < hings.Length; ++i)
//             {
//                 TargetPoint pt = hings[i].GetComponent<TargetPoint>();
//                 if(pt)
//                 {
//                     Targets.Add(hings[i]);
//                 }
//             }
//         }
// 
//         public void OnMoveToTarget(object sender, GameEventArgs e)
//         {
//             GameObject gb = GameEntry.Entity.GetEntity(Id).Handle as GameObject;
//             MoveToTargetEventArgs mvArgs = e as MoveToTargetEventArgs;
//             //start move new position
//             MoveTarget ctl = GetComponentInParent<MoveTarget>();
//             if(ctl)
//                 ctl.Move(gb.transform.position, mvArgs.MovePos);
//             //start move state
//             Animator animator = GetComponentInParent<Animator>();
//             if(animator)
//                 animator.SetBool("Move", true);
//         }

    }
}
