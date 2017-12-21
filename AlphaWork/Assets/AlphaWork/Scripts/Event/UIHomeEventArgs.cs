using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlphaWork
{
    class UIHomeEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(UIHomeEventArgs).GetHashCode();
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
