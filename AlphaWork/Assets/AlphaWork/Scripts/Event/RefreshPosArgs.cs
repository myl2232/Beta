using UnityEngine;
using System.Collections;
using GameFramework.Event;

namespace AlphaWork
{
    public class RefreshPosArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(RefreshPosArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        private GameObject MainObject;
        public GameObject Gb
        {
            get { return MainObject; }
            set { MainObject = value; }
        }
        private Transform m_HitTransform;
        public Transform TransCache
        {
            get{ return m_HitTransform;}
            set{ value = m_HitTransform;}
        }
        public RefreshPosArgs(GameObject pObject,Transform trans)
        {
            MainObject = pObject;
            m_HitTransform = trans;
        }
        public override void Clear()
        {

        }
    }
}

