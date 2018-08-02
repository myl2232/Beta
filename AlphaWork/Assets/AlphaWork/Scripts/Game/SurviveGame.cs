using GameFramework;
using GameFramework.DataTable;
using GameFramework.Event;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
/*using UnityStandardAssets.Characters.ThirdPerson;*/
using System;
using System.Collections.Generic;
using UnityGameFramework.Runtime;
//using UnityEngine.XR.iOS;

namespace AlphaWork
{
    public partial class SurviveGame : GameBase
    {
        protected LevelManager m_levelManager = new LevelManager();
        protected AvatarManager m_avatarManager = new AvatarManager();
        private float m_ElapseSeconds = 0f;
        public Transform m_MainEthanTransform;
        protected TBPinchZoom tbZoom;
        protected TBOrbit tbOrbit;
        //protected Alpha Alib = new Alpha();

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

            m_levelManager.m_parent = this;

            Camera.main.gameObject.GetOrAddComponent<PinchRecognizer>();
            tbZoom = Camera.main.gameObject.GetOrAddComponent<TBPinchZoom>();
            tbOrbit = Camera.main.gameObject.GetOrAddComponent<TBOrbit>();
            
        }

        public void OnRefreshMainPos(object sender, GameEventArgs e)
        {
            RefreshMainPosArgs arg = e as RefreshMainPosArgs;
            MainEthan.transform.position = arg.TransCache.position + new Vector3(0,2,0);
        }


        protected override void RegisterStructure(UnityGameFramework.Runtime.Entity ent)
        {
            base.RegisterStructure(ent);

            m_levelManager.RegisterStructure(ent);            
        }
        

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            base.Update(elapseSeconds, realElapseSeconds);

            if(MainEthan && !GameEntry.Config.GameSetting.ArMode)
            {
                Vector3 offset = new Vector3(8, 8, 8);
                offset += MainEthan.transform.position;
                Camera.main.transform.position = offset;
                tbZoom.DefaultPos = Camera.main.transform.position;
                tbOrbit.target = MainEthan.gameObject.transform;

                Camera.main.transform.LookAt(MainEthan.transform.position);

                //string str = string.Format("{0:f2},{1:f2},{2:f2}", hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
                //DebugUtility.DebugText(str);
                //GameFramework.Log.Debug(str);

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
