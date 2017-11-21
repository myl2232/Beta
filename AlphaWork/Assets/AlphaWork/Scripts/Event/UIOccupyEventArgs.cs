using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFramework.Event;

namespace AlphaWork
{
    class UIOccupyEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(UIOccupyEventArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

    }
}
