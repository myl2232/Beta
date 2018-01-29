using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameFramework.Event;

namespace AlphaWork
{
    public class MoveToTargetEndEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(MoveToTargetEndEventArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public int parentId;
        public MoveToTargetEndEventArgs(int pId)
        {
            parentId = pId;
        }
        public override void Clear()
        {

        }
        
    }
}
