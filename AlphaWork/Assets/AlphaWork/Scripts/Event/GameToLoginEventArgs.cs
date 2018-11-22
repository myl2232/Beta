using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameFramework.Event;

namespace AlphaWork
{
    public class GameToLoginEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(GameToLoginEventArgs).GetHashCode();
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
