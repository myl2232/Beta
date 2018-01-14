using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlphaWork
{
    class UIThetaEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(UIThetaEventArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        public override void Clear()
        {

        }
    }
}

