using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    public class AvatarManager
    {
        private Dictionary<string, int> m_Skeletons = new Dictionary<string, int>();//<skeletonString, entityId>

        public AvatarManager()
        {
            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);            
        }

        protected virtual void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.EntityLogicType == typeof(AvatarEntity))
            {
                AvatarData userData = ne.UserData as AvatarData;

                if(!m_Skeletons.ContainsKey(userData.Skeleton))
                    m_Skeletons[userData.Skeleton] = ne.Id;

                GameEntry.Event.Fire(this,new AvatarCreateEventArgs(userData,ne.Entity.Handle as GameObject));
            }
        }

        protected virtual void OnShowEntityFailure(object sender, GameEventArgs e)
        {
        }

    }
}
