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

        public bool GameOver
        {
            get;
            protected set;
        }

        protected UnityGameFramework.Runtime.Entity MainEthan = null;
        public UnityGameFramework.Runtime.Entity MainActor
        {
            get { return MainEthan; }
            set { MainEthan = value; }
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
            //if (ne.EntityLogicType == typeof(Structure))
            //{
            //    RegisterStructure(ne.Entity);
            //}
            if (ne.EntityLogicType == typeof(Ethan))
            {
                List<UPlayer> players;
                GameEntry.DataBase.DataDevice.GetDataByKey<UPlayer>(GameEntry.Config.GameSetting.CurrentUser, out players);
                if (players.Count > 0)
                {
                    Vector3 pos = new Vector3(players[0].xPos, players[0].yPos, players[0].zPos);
                    MainEthan.transform.position = pos;
                }
            }
        }
        
        //protected virtual void RegisterStructure(UnityGameFramework.Runtime.Entity ent)
        //{
        //}

        protected virtual void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            Log.Warning("Show entity failure with error message '{0}'.", ne.ErrorMessage);
        }

        public static void GetMainPos(out Vector3 pos)
        {
            pos = GameEntry.Config.GameSetting.gameContrller.MainActor.transform.position;
        }

        public static bool HasMainActor()
        {
            return null != GameEntry.Config.GameSetting.gameContrller.MainActor ? true : false;
        }
    }
}
