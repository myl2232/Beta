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

            if(MainEthan && !GameEntry.ArMode)
            {
                Vector3 offset = new Vector3(6, 6, 6);
                offset += MainEthan.transform.position;
                Camera.main.transform.position = offset;

                //RaycastHit hitInfo;
                //Physics.Raycast(Camera.main.transform.position, MainEthan.transform.position - Camera.main.transform.position - offset, out hitInfo, 100);

                Camera.main.transform.LookAt(MainEthan.transform.position/* + new Vector3(0,1,0)hitInfo.point*/);

                //string str = string.Format("{0:f2},{1:f2},{2:f2}", hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
                //DebugUtility.DebugText(str);
                //GameFramework.Log.Debug(str);

            }

            m_ElapseSeconds += realElapseSeconds;
            if (m_ElapseSeconds >= 2f)
            {
                m_ElapseSeconds = 0.0f;
                //RaycastHit hit;
                Vector3 tempPos = new Vector3(UnityEngine.Random.Range(0.0f, 50), 5.0f, UnityEngine.Random.Range(0.0f, 50.0f));
                //if (Physics.Raycast(tempPos, Vector3.down, out hit))
                {
                    m_levelManager.RefreshAgent(tempPos/*hit.point*/);
                }
                ////refresh ships pos
                //for(int i = 0; i < m_EnemyIds.Count; ++i)
                //{
                //    RaycastHit hitResult;
                //    Vector3 RamPos = new Vector3(UnityEngine.Random.Range(0.0f, 50), 5.0f, UnityEngine.Random.Range(0.0f, 50.0f));
                //    if (Physics.Raycast(RamPos, Vector3.down, out hitResult))
                //    {
                //        UnityGameFramework.Runtime.Entity enemyObj = GameEntry.Entity.GetEntity(m_EnemyIds[i]);
                //        if(enemyObj)
                //            m_levelManager.RefreshEnemy(enemyObj.Handle as GameObject, ObjectUtility.GetTargetAgent() as GameObject, RamPos);
                //    }
                //}
            }
            m_levelManager.RefreshStructures();
        }
                
    }
}
