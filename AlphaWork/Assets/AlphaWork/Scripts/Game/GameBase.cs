using GameFramework;
using GameFramework.Event;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    public abstract class GameBase
    {
        public abstract GameMode GameMode
        {
            get;
        }

        //         protected ScrollableBackground SceneBackground
        //         {
        //             get;
        //             private set;
        //         }

        public bool GameOver
        {
            get;
            protected set;
        }

        protected static UnityGameFramework.Runtime.Entity MainEthan = null;
        protected int m_MenuId;
        public int MenuID
        {
            get { return m_MenuId; }
            set { m_MenuId = value; }
        }
        protected bool m_AIGo;
        public bool AIGo
        {
            get { return m_AIGo; }
            set { m_AIGo = value; }
        }
        public virtual void Initialize()
        {
            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);

            GameOver = false;
            
        }

        public virtual void Shutdown()
        {
            GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Unsubscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
        }

        public virtual void Update(float elapseSeconds, float realElapseSeconds)
        {
                           
        }

        protected virtual void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.EntityLogicType == typeof(Ethan))
            {
                if (MainEthan == null)
                {
                    Vector3 offset = new Vector3(10, 10, 0);
                    Camera.main.transform.position = ne.Entity.transform.position + offset;
                    Camera.main.transform.LookAt(ne.Entity.transform);
                    MainEthan = ne.Entity;
                    GameObject ob = MainEthan.Handle as GameObject;
                    ob.name = "MainEthan";
                }

            }
            else if (ne.EntityLogicType == typeof(Structure))
            {                
                RegisterStructure(ne.Entity);
            }
            else if (ne.EntityLogicType == typeof(NPC))
            {
                
            }
        }
        
        protected virtual void RegisterStructure(UnityGameFramework.Runtime.Entity ent)
        {
        }

        protected virtual void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            Log.Warning("Show entity failure with error message '{0}'.", ne.ErrorMessage);
        }

        public static void GetMainPos(out Vector3 pos)
        {
            pos = MainEthan.transform.position;
        }

        public static bool HasMainActor()
        {
            return null != MainEthan ? true : false;
        }
    }
}
