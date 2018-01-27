using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AlphaWork
{
    public class MoveTarget : MonoBehaviour
    {
        public float Speed = 5;
        //public float Scale = 1;
        
        private int pathNum = 0;
        private Vector3[] smoothPath; 
        private int m_curIdx = 0;
        private Transform m_trans;

        // Use this for initialization
        void Start()
        {
            m_trans = GetComponentInParent<Transform>();
            smoothPath = new Vector3[512];
        }

        // Update is called once per frame
        void Update()
        {
            if (pathNum > 0)
            {                
                Vector3 pCur = m_trans.transform.position;
                if (m_curIdx > (pathNum - 1))
                    return;

                Vector3 pNext = smoothPath[m_curIdx];
                Vector3 dir = Vector3.Normalize(pNext - pCur);
                Vector3 newPos = pCur + dir * Speed * Time.deltaTime;
                Vector3 newDir = Vector3.Normalize(pNext - newPos);
                Vector3 realPos;
                if(Vector3.Dot(dir,newDir) > 0 && IsWalkable(newPos))//not go to target yet
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
            
            for(int i = 0; i < smoothPath.Length;++i)
            {
                if (i < smoothPath.Length - 1)
                    Debug.DrawLine(smoothPath[i], smoothPath[i + 1],Color.blue);
            }
        }

        public void Pause()
        {
            pathNum = 0;
        }

        public void Move(Vector3 startPos, Vector3 endPos,float baseSpeed = 1)
        {
            pathNum = 0;
            Speed = baseSpeed;

            if (GameEntry.UseNavGrid)
            {
                smoothPath = GameEntry.NavGrid.FindPath(startPos, endPos);
                pathNum = smoothPath.Length;
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

        //temp
        public void SetTarget(Vector3 goal)
        {
            NavMeshAgent ag = GetComponent<NavMeshAgent>();
            if(ag)
                ag.destination = goal;
        }
    }
}

