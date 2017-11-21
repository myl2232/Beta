using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlphaWork
{
    class AIGoEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(AIGoEventArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
    }
}
