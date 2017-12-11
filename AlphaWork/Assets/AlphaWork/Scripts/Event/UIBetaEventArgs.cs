using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlphaWork
{
    class UIBetaEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(UIBetaEventArgs).GetHashCode();
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
