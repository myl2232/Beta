using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFramework.Event;

namespace AlphaWork
{
    class UIAlphaEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(UIAlphaEventArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

    }
}
