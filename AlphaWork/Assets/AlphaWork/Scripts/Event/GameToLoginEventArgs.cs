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

        /// <summary>
        /// 获取用户自定义数据。
        /// </summary>
        //protected object sender;
        public GameObject Sender;
        //{
        //    get { return sender; }
        //    private set {}
        //}

        public GameToLoginEventArgs(GameObject senderObject = null)
        {
            Sender = senderObject;
        }

        public override void Clear()
        {

        }
    }
}
