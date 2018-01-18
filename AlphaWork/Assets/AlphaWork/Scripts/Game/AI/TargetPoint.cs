using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameFramework;

namespace AlphaWork
{
    public class TargetPoint : MonoBehaviour
    {
        private Vector3 m_size = new Vector3(1, 1, 1);

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(transform.position, m_size);   
        }
#endif

    }
}
