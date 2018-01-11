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
        private List<GameObject> m_Parts/* = new List<GameObject>()*/;
        private LoadAssetCallbacks m_LoadCallbacks;        
        private EnemyAgent m_agent;//agent for behaviour tree
        public EnemyAgent Agent
        {
            get { return m_agent; }
        }
        private float m_lastTime;
        private int m_senseHit;
        
        public AvatarEntity()
        {
            GameEntry.Event.Subscribe(UIAlphaEventArgs.EventId, OnTestAlpha);
            GameEntry.Event.Subscribe(UIBetaEventArgs.EventId, OnTestBeta);
            GameEntry.Event.Subscribe(AvatarCreateEventArgs.EventId, OnCreateAvatar);
            m_LoadCallbacks = new LoadAssetCallbacks(OnLoadSuccessCallback, OnLoadFailureCallback);
            m_Parts = new List<GameObject>();
 
            m_lastTime = 0;
            m_senseHit = 0;
        }
        
        // Use this for initialization
        void Start()
        {
            if ((Data as AvatarData).AlowMove)
            {
                //ai
                m_agent = new EnemyAgent();
                bool bRet = m_agent.btload("EnemyAvatar");
                m_agent.btsetcurrent("EnemyAvatar");
                m_agent.ParentId = Id;
                m_agent._set_senseRadius((Data as AvatarData).SenseRadius);
                m_agent._set_attackRadius((Data as AvatarData).AttackRadius);
                m_agent.InitAI();

//                 BehaviourMove moveBehaviour = gameObject.AddComponent<BehaviourMove>();
//                 moveBehaviour.Parent = this;
            }
        }

        // Update is called once per frame
        void Update()
        {                   
            //ai
            behaviac.EBTStatus status = behaviac.EBTStatus.BT_RUNNING;
            while ((status == behaviac.EBTStatus.BT_RUNNING) && (m_agent != null)
                && (Time.realtimeSinceStartup - m_lastTime > 0.3))                 
            {
                status = m_agent.btexec();
                m_lastTime = Time.realtimeSinceStartup;
                m_agent._set_bAwakeSense(true);
            }

        }

        /*for test*/
        private float lscale = 1;
        private void LateUpdate()
        {
            Transform trans = AssetUtility.FindChild(Entity.transform, "Bip001 L Thigh");//"Bip001 L Hand"
            trans.localScale *= lscale;
        }
        void OnTestAlpha(object sender, GameEventArgs arg)
        {
            lscale = (lscale >1)?lscale*0.5f:1.0f;
        }
        void OnTestBeta(object sender, GameEventArgs arg)
        {
            lscale *= 2;
        }
        /*end test*/

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
