using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AlphaWork
{
    public class MoveTarget : MonoBehaviour
    {
        public float Speed = 5;
        public float Scale = 1;
        
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
                Vector3 newPos = pCur + dir * Speed * Scale * Time.deltaTime;
                Vector3 newDir = Vector3.Normalize(pNext - newPos);
                if(Vector3.Dot(dir,newDir) > 0)//not go to target yet
                {
                    m_trans.transform.position = newPos;
                }
                else
                {
                    m_trans.transform.position = pNext;
                    m_curIdx++;
                }                
            }

        }

        public void Pause()
        {
            pathNum = 0;
        }

        public void Move(Vector3 startPos, Vector3 endPos)
        {
            pathNum = 0;

            if (GameEntry.UseNavGrid)
               GameEntry.NavGrid.FindPath(startPos, endPos, ref smoothPath);
            else
                RecastNavigationDllImports.PathFind(startPos, endPos, ref pathNum, ref smoothPath);
            
            if(pathNum == 0)
            {
                pathNum = 1;
                smoothPath[0] = endPos;
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

