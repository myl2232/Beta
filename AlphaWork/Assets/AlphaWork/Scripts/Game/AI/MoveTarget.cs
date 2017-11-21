using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AlphaWork
{
    public class MoveTarget : MonoBehaviour
    {
        public float Speed = 1;
        private Vector3 m_movePos;
        private Vector3 m_startPos;
        
        int pathNum = 0;
        Vector3[] smoothPath = new Vector3[512];
        private int m_curIdx = 0;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (pathNum > 0)
            {                
                Vector3 pCur = GetComponentInParent<Transform>().transform.position;
                if (m_curIdx > (pathNum - 1))
                    return;

                Vector3 pNext = smoothPath[m_curIdx];
                Vector3 dir = Vector3.Normalize(pNext - pCur);
                Vector3 newPos = pCur + dir * Speed * Time.deltaTime;
                Vector3 newDir = Vector3.Normalize(pNext - newPos);
                if(Vector3.Dot(dir,newDir) > 0)//not go to target yet
                {
                    GetComponentInParent<Transform>().transform.position = newPos;
                }
                else
                {
                    GetComponentInParent<Transform>().transform.position = pNext;
                    m_curIdx++;
                }                
            }

        }

        public void Move(Vector3 startPos, Vector3 movePos)
        {
            m_startPos = startPos;
            m_movePos = movePos;
            pathNum = 0;
            
            RecastNavigationDllImports.PathFind(m_startPos, m_movePos, ref pathNum, ref smoothPath);

            if(pathNum == 0)
            {
                pathNum = 1;
                smoothPath[0] = movePos;
            }
        }

        public void SetTarget(Vector3 goal)
        {
            NavMeshAgent ag = GetComponent<NavMeshAgent>();
            if(ag)
                ag.destination = goal;
        }
    }
}

