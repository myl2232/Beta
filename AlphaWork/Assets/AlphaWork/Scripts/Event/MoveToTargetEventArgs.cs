using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameFramework.Event;

namespace AlphaWork
{
    public class MoveToTargetEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(MoveToTargetEventArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        protected int m_entId;
        public int EId
        {
            get { return m_entId; }
        }
        protected Vector3 m_MovePos;
        public Vector3 MovePos
        {
            get { return m_MovePos; }
            set { value = m_MovePos; }
        }
        public MoveToTargetEventArgs(Vector3 newMovePos)
        {
            m_MovePos = newMovePos;
        }
        public MoveToTargetEventArgs(int entId,Vector3 newMovePos)
        {
            m_entId = entId;
            m_MovePos = newMovePos;
        }
        public MoveToTargetEventArgs(int entId)
        {
            m_entId = entId;
        }
        public override void Clear()
        {

        }
    }
}
