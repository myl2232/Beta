
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
        protected LevelManager m_levelManager;
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

        public LevelManager LevelManager
        {
            get
            {
                return m_levelManager;
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            GameEntry.Event.Subscribe(RefreshMainPosArgs.EventId, OnRefreshMainPos);

            GameEntry.Event.Fire(this, new GameStartEventArgs());
            GameEntry.UI.OpenUIForm(UIFormId.MainForm);

            m_levelManager = new LevelManager();
            m_levelManager.m_parent = this;
            m_avatarManager = new AvatarManager();

            Camera.main.gameObject.GetOrAddComponent<PinchRecognizer>();
            tbZoom = Camera.main.gameObject.GetOrAddComponent<TBPinchZoom>();
            tbOrbit = Camera.main.gameObject.GetOrAddComponent<TBOrbit>();
            
        }

        public void OnRefreshMainPos(object sender, GameEventArgs e)
        {
            RefreshMainPosArgs arg = e as RefreshMainPosArgs;
            MainEthan.transform.position = arg.TransCache.position + new Vector3(0, 2, 0);
        }


        //protected override void RegisterStructure(UnityGameFramework.Runtime.Entity ent)
        //{
        //    base.RegisterStructure(ent);

        //    m_levelManager.RegisterStructure(ent);
        //}


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
            if (m_ElapseSeconds >= 2f)
            {
                m_ElapseSeconds = 0.0f;
                Vector3 tempPos = new Vector3(UnityEngine.Random.Range(0.0f, 50), 5.0f, UnityEngine.Random.Range(0.0f, 50.0f));
                m_levelManager.RefreshAgent(tempPos);
            }
            m_levelManager.RefreshStructures();
        }
    }

}
