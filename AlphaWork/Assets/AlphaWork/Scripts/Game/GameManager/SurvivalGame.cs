
using System;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using UnityEngine;

namespace AlphaWork
{
    public class SurvivalGame : GameBase
    {
        //protected LevelManager m_levelManager;
        protected AvatarManager m_avatarManager;
        private float m_ElapseSeconds = 0f;
        public Transform m_MainEthanTransform;
        protected TBPinchZoom tbZoom;
        protected TBOrbit tbOrbit;

        public override GameMode GameMode
        {
            get
            {
                return GameMode.Survival;
            }
        }

        //public LevelManager LevelManager
        //{
        //    get
        //    {
        //        return m_levelManager;
        //    }
        //}

        public override void Initialize()
        {
            base.Initialize();
            GameEntry.Event.Subscribe(GameStartEventArgs.EventId, OnGameStart);
            GameEntry.Event.Subscribe(RefreshPosArgs.EventId, OnRefreshMainPos);
            GameEntry.Event.Fire(this, new GameStartEventArgs());            

            //m_levelManager = new LevelManager();
            //m_levelManager.m_parent = this;
            m_avatarManager = new AvatarManager();

            Camera.main.gameObject.GetOrAddComponent<PinchRecognizer>();
            tbZoom = Camera.main.gameObject.GetOrAddComponent<TBPinchZoom>();
            tbOrbit = Camera.main.gameObject.GetOrAddComponent<TBOrbit>();
            
        }
        public override void Shutdown()
        {
            base.Shutdown();
            GameEntry.Event.Unsubscribe(RefreshPosArgs.EventId, OnRefreshMainPos);
            GameEntry.Event.Unsubscribe(GameStartEventArgs.EventId, OnGameStart);       
        }

        //protected override void RegisterStructure(UnityGameFramework.Runtime.Entity ent)
        //{
        //    base.RegisterStructure(ent);

        //    m_levelManager.RegisterStructure(ent);
        //}
        protected void LoadGameObjects()
        {
            if (MainEthan == null)
            {
                object tg = ObjectUtility.GetFellow("TargetMain");
                //add main actor
                Vector3 mainPos = Camera.main.transform.position + Camera.main.transform.forward * 20;
                if (tg != null)
                    mainPos = (tg as GameObject).transform.position + Vector3.up * 10;
                RaycastHit hit;
                Physics.Raycast(mainPos, Vector3.down, out hit, 1000);
                GameEntry.Entity.ShowEthan(new EthanData(GameEntry.Entity.GenerateSerialId(), 80001, CampType.Player)
                {
                    Position = hit.point,
                });

                //GameEntry.Entity.ShowNPC(new NPCData(GameEntry.Entity.GenerateSerialId(), 50003, CampType.Player)
                //{
                //    Position = hit.point,
                //});
            }
            else if(!MainEthan.gameObject.activeSelf)
            {                
                //MainEthan.OnShow(null);
            }
        }

        public void OnGameStart(object sender, GameEventArgs e)
        {
            LoadGameObjects();
        }
        public void OnRefreshMainPos(object sender, GameEventArgs e)
        {
            RefreshPosArgs arg = e as RefreshPosArgs;
            if (arg != null && GameBase.MainEthan.gameObject == arg.Gb)
            {
                arg.Gb.transform.position = arg.TransCache.position;
                List<UPlayer> players;
                GameEntry.DataBase.DataDevice.GetDataByKey<UPlayer>(GameEntry.Config.GameSetting.CurrentUser, out players);
                if (players.Count > 0)
                {
                    Vector3 lastPos = new Vector3(players[0].xPos, players[0].yPos, players[0].zPos);
                    if((MainEthan.transform.position - lastPos).magnitude > 0.1)
                    {
                        Vector3 pos = arg.Gb.transform.position;
                        players[0].xPos = pos.x;
                        players[0].yPos = pos.y;
                        players[0].zPos = pos.z;
                        GameEntry.DataBase.DataDevice.UpdateData<UPlayer>(players[0]);
                    }

                }
            }
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            base.Update(elapseSeconds, realElapseSeconds);

            if (MainEthan && !GameEntry.Config.GameSetting.ArMode)
            {
                Vector3 offset = new Vector3(8, 8, 8);
                offset += MainEthan.transform.position;
                Camera.main.transform.position = offset;
                tbZoom.DefaultPos = Camera.main.transform.position;
                tbOrbit.target = MainEthan.gameObject.transform;

                Camera.main.transform.LookAt(MainEthan.transform.position);

            }

            m_ElapseSeconds += realElapseSeconds;
            if (m_ElapseSeconds >= 3f)
            {
                m_ElapseSeconds = 0.0f;
                //角色位置存数据库
                if(MainEthan)
                    GameEntry.Event.Fire(this, new RefreshPosArgs(MainEthan.gameObject, MainEthan.transform));
                    
                //Vector3 tempPos = new Vector3(UnityEngine.Random.Range(0.0f, 50), 5.0f, UnityEngine.Random.Range(0.0f, 50.0f));
                //m_levelManager.RefreshAgent(tempPos);
            }
            //m_levelManager.RefreshStructures();
        }
    }

}
