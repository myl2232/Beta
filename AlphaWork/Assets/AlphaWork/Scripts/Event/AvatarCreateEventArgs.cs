using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    class AvatarCreateEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(AvatarCreateEventArgs).GetHashCode();

        private AvatarData m_data = new AvatarData();
        public AvatarCreateEventArgs(AvatarData data, GameObject skeletonObj)
        {
            m_data = data;
            SkeletonObject = skeletonObj;
        }
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public GameObject SkeletonObject
        {
            get;
            set;
        }
        /// <summary>
        /// 获取用户自定义数据。
        /// </summary>
        public AvatarData UserData
        {
            get { return m_data; }
        }
        public override void Clear()
        {
            
        }
    }
}
