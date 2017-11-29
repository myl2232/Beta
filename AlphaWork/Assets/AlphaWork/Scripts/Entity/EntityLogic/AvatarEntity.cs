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

        public AvatarEntity()
        {
            GameEntry.Event.Subscribe(AvatarCreateEventArgs.EventId, OnCreateAvatar);
            m_LoadCallbacks = new LoadAssetCallbacks(OnLoadSuccessCallback, OnLoadFailureCallback);
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            bool skill1 = CrossPlatformInputManager.GetButtonDown("skill1");
            if(skill1)
            {
                GameObject gb = Entity.Handle as GameObject;
                gb.GetComponent<Animator>().SetInteger("skill",1);
            }
            if(CrossPlatformInputManager.GetButtonDown("noskill"))
            {
                GameObject gb = Entity.Handle as GameObject;
                gb.GetComponent<Animator>().SetInteger("skill", 0);
            }
        }

        protected internal override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
        {
            base.OnAttached(childEntity, parentTransform, userData);

        }

        private void OnDestroy()
        {
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
            //data.AttachTrans = AssetUtility.FindChild(CachedTransform, drEntity.Bone/*"Bip001 Spine1"*/);//
            data.AttachPos = drEntity.Position;
            data.AttachScale = drEntity.Scale;
            data.AttachScale.Set(data.AttachScale.x*transform.localScale.x,
                data.AttachScale.y * transform.localScale.y,
                data.AttachScale.z * transform.localScale.z);
            data.AttachRotate = drEntity.Rotate;           
            data.Bone = drEntity.Bone;
            
            int newId = GameEntry.Entity.GenerateSerialId();
            GameEntry.Entity.ShowEntity<AttachmentEntity>(newId, drEntity.Weapon, "Attachment",data);

            GameEntry.Entity.AttachEntity(newId,Id,data);
        }

    }
}
