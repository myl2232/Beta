using UnityEngine;
using System.Collections;
using GameFramework.Event;

namespace AlphaWork
{
    public class RefreshMainPosArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(RefreshMainPosArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        protected Transform m_HitTransform;
        public Transform TransCache
        {
            get{ return m_HitTransform;}
            set{ value = m_HitTransform;}
        }
        public RefreshMainPosArgs(Transform trans)
        {
            m_HitTransform = trans;
        }
        public override void Clear()
        {

        }
    }
}

