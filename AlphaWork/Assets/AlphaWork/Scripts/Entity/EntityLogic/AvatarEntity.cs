using GameFramework;
using GameFramework.DataTable;
using GameFramework.Event;
using GameFramework.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityStandardAssets.CrossPlatformInput;

namespace AlphaWork
{
    public class AvatarEntity : EntityObject
    {
        private int m_partCount;
        private GameObject m_skeleton;
        private List<GameObject> m_Parts = new List<GameObject>();
        private LoadAssetCallbacks m_LoadCallbacks;
        private SecondAgent m_agent;//agent for behaviour tree
        public SecondAgent Agent
        {
            get { return m_agent; }
        }

        //private int testskill = 0;

        public AvatarEntity()
        {
            GameEntry.Event.Subscribe(UIBetaEventArgs.EventId, OnTestBeta);
            GameEntry.Event.Subscribe(AvatarCreateEventArgs.EventId, OnCreateAvatar);
            m_LoadCallbacks = new LoadAssetCallbacks(OnLoadSuccessCallback, OnLoadFailureCallback);
            //ai
            m_agent = new SecondAgent();
            bool bRet = m_agent.btload("EnemyActorBT");
            m_agent.btsetcurrent("EnemyActorBT");
            m_agent.Parent = this;
            m_agent._set_m_LogicStatus(LogicStatus.ELogic_IDLE);
        }

        // Use this for initialization
        void Start()
        {
            if ((Data as AvatarData).AlowMove)
            {
                m_agent.InitAI((Data as AvatarData).AIRadius);
//                 BehaviourMove moveBehaviour = gameObject.AddComponent<BehaviourMove>();
//                 moveBehaviour.Parent = this;
            }
        }

        // Update is called once per frame
        void Update()
        {
            //bool skill1 = CrossPlatformInputManager.GetButtonDown("skill1");
            //if(skill1)
            //{
            //    GameObject gb = Entity.Handle as GameObject;
            //    gb.GetComponent<Animator>().SetInteger("skill",1);
            //}
            //if(CrossPlatformInputManager.GetButtonDown("noskill"))
            //{
            //    GameObject gb = Entity.Handle as GameObject;
            //    gb.GetComponent<Animator>().SetInteger("skill", 0);
            //}

            //ai
            if(m_agent != null)
            {
                behaviac.EBTStatus status = behaviac.EBTStatus.BT_RUNNING;
                status = m_agent.btexec();
            }
                
        }

        void OnTestBeta(object sender, GameEventArgs arg)
        {
            //if (testskill == 0)
            //{
            //    GameObject gb = Entity.Handle as GameObject;
            //    gb.GetComponent<Animator>().SetInteger("skill", 1);

            //    testskill = 1;
            //}
            //else if (testskill == 1)
            //{
            //    GameObject gb = Entity.Handle as GameObject;
            //    gb.GetComponent<Animator>().SetInteger("skill", 0);

            //    testskill = 0;
            //}

        }
        protected internal override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
        {
            base.OnAttached(childEntity, parentTransform, userData);

        }

        private void OnDestroy()
        {
            m_agent = null;
            m_Parts.Clear();
        }

        protected void _AssemblePart(GameObject part)
        {
            m_Parts.Add(part);
            if (m_Parts.Count == m_partCount)
            {
                //拼接Avatar
                RefreshCharacter();
                //加载部件（目前只是一个武器60001）
                ShowAttachment(new AttachmentData(GameEntry.Entity.GenerateSerialId(), 60001, CampType.Player));
            }
        }

        public void RefreshCharacter()
        {
            List<SkinnedMeshRenderer> meshRenders = new List<SkinnedMeshRenderer>();
            for (int i = 0; i < m_Parts.Count; ++i)
            {
                SkinnedMeshRenderer[] smr = m_Parts[i].GetComponentsInChildren<SkinnedMeshRenderer>();
                for (int k = 0; k < smr.Length; ++k)
                    meshRenders.Add(smr[k]);
            }
            Utils.CombineObject(m_skeleton, meshRenders.ToArray());
        }

        protected void OnCreateAvatar(object sender, GameEventArgs e)
        {
            AvatarCreateEventArgs ne = (AvatarCreateEventArgs)e;
            if (Id != ne.UserData.Id)
                return;

            m_Parts.Clear();
            m_partCount = 0;

            AvatarData userData = ne.UserData;
            m_skeleton = ne.SkeletonObject;
            List<string> parts = userData.GetParts();
            m_partCount = parts.Count;
            for (int i = 0; i < m_partCount; ++i)
            {
                GameEntry.Resource.LoadAsset(parts[i], m_LoadCallbacks, userData.Skeleton);
            }

            return;
        }

        public void OnLoadSuccessCallback(string assetName, object asset, float duration, object userData)
        {
            GameObject gb = asset as GameObject;
            _AssemblePart(gb);
        }

        public void OnLoadFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
        }

        public void ShowAttachment(AttachmentData data)
        {
            if (data == null)
            {
                Log.Warning("Data is invalid.");
                return;
            }
            IDataTable<DRAttachment> dtEntity = GameEntry.DataTable.GetDataTable<DRAttachment>();
            DRAttachment drEntity = dtEntity.GetDataRow(data.TypeId);
            if (drEntity == null)
            {
                Log.Warning("Can not load entity id '{0}' from data table.", data.TypeId.ToString());
                return;
            }

            data.ParentId = Id;
            data.AttachPos = drEntity.Position;
            data.AttachScale = drEntity.Scale;
            data.AttachScale.Set(data.AttachScale.x*transform.localScale.x,
                data.AttachScale.y * transform.localScale.y,
                data.AttachScale.z * transform.localScale.z);
            data.AttachRotate = drEntity.Rotate;           
            data.Bone = drEntity.Bone;

            GameEntry.Entity.ShowEntity<AttachmentEntity>(data.Id/*GameEntry.Entity.GenerateSerialId()*/, drEntity.Weapon, "Attachment",data);

            //GameEntry.Entity.AttachEntity(newId,Id,data);
        }

    }
}
