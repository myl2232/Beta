using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    public class MoveTarget : MonoBehaviour
    {
        public float Speed = 5;

        private int pathNum = 0;
        private Vector3[] smoothPath;
        private int m_curIdx = 0;
        private Transform m_trans;
        private bool m_movePause = true;
        public bool MovePause
        {
            get { return m_movePause; }
            set { m_movePause = value; }
        }

        // Use this for initialization
        void Start()
        {
            m_trans = GetComponentInParent<Transform>();
            smoothPath = new Vector3[512];
        }
        
        // Update is called once per frame 
        private void LateUpdate()
        {
            if (pathNum > 0)
            {
                Vector3 pCur = m_trans.transform.position;
                if (m_curIdx > (pathNum - 1))
                {
                    return;
                }

                Vector3 pNext = smoothPath[m_curIdx];
                Vector3 dir = Vector3.Normalize(pNext - pCur);
                Vector3 newPos = pCur + dir * Speed * Time.deltaTime;
                Vector3 newDir = Vector3.Normalize(pNext - newPos);
                Vector3 realPos;
                if (Vector3.Dot(dir, newDir) > 0 && IsWalkable(newPos))//not go to target yet
                {
                    realPos = newPos;
                }
                else
                {
                    realPos = pNext;
                    m_curIdx++;
                }
                m_trans.transform.forward = realPos - m_trans.transform.position;
                m_trans.transform.position = realPos;
            }
            //else
            //{
            //    if (m_movePause)
            //        GameEntry.Event.Fire(this, new MoveToTargetEndEventArgs(GetComponentInParent<Entity>().Id));
            //}


            for (int i = 0; i < smoothPath.Length; ++i)
            {
                if (i < smoothPath.Length - 1)
                    Debug.DrawLine(smoothPath[i], smoothPath[i + 1], Color.blue);
            }
        }

        public void Pause()
        {
            pathNum = 0;            
            //GameEntry.Event.Fire(this, new MoveToTargetEndEventArgs(GetComponentInParent<Entity>().Id));
            StartCoroutine(_LockMovement(0.1f));
        }

        public void Move(Vector3 startPos, Vector3 endPos)
        {
            pathNum = 0;

            if (GameEntry.UseNavGrid)
            {
                smoothPath = GameEntry.NavGrid.FindPath(startPos, endPos);
                pathNum = smoothPath.Length;

                //TargetableObject etParent = GetComponentInParent<TargetableObject>();
                //GameEntry.Event.Fire(this, new MoveToTargetEventArgs(etParent.Id));
                
            }
            else
            {
                smoothPath = new Vector3[512];
                RecastNavigationDllImports.PathFind(startPos, endPos, ref pathNum, ref smoothPath);
                if (pathNum == 0)
                {
                    pathNum = 1;
                    smoothPath[0] = endPos;
                }
            }
        }

        private bool IsWalkable(Vector3 pos)
        {
            if (GameEntry.UseNavGrid)
            {
                return GameEntry.NavGrid.IsWalkable(pos);
            }
            else
            {
                return true;
            }
        }

        //留足时间从移动动作切换到其他动作
        protected IEnumerator _LockMovement(float lockTime)
        {
            yield return new WaitForSeconds(lockTime);
        }

        //temp
        public void SetTarget(Vector3 goal)
        {
            NavMeshAgent ag = GetComponent<NavMeshAgent>();
            if(ag)
                ag.destination = goal;
        }
    }
}

