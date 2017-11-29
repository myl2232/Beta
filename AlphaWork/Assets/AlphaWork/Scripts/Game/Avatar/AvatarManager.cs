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
        private Dictionary<int, string> m_Skeletons = new Dictionary<int, string>();//<skeletonString, entityId>

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

                if(!m_Skeletons.ContainsKey(userData.Id))
                    m_Skeletons[userData.Id] = userData.Skeleton;
                
                GameObject ob = ne.Entity.Handle as GameObject;
                ob.name = "Avatar" + m_Skeletons.Count;
                GameEntry.Event.Fire(this,new AvatarCreateEventArgs(userData, ob));
            }
        }

        protected virtual void OnShowEntityFailure(object sender, GameEventArgs e)
        {
        }

        

    }
}
