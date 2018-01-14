using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFramework.Event;

namespace AlphaWork
{
    class UIAttack2EventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(UIAttack2EventArgs).GetHashCode();
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