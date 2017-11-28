using GameFramework.Event;
using GameFramework.Resource;
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
        }
         
        protected void _AssemblePart(GameObject part)
        {
            m_Parts.Add(part);
            if (m_Parts.Count == m_partCount)
                RefreshCharacter();
        }

        public void RefreshCharacter()
        {       
            List<SkinnedMeshRenderer> meshRenders = new List<SkinnedMeshRenderer>();
            for (int i = 0; i < m_Parts.Count; ++i)
            {
                SkinnedMeshRenderer[] smr = m_Parts[i].GetComponentsInChildren<SkinnedMeshRenderer>();
                for(int k = 0; k < smr.Length; ++k)
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
            string skeleton = userData as string;
            GameObject gb = asset as GameObject;
            _AssemblePart(gb);
        }

        public void OnLoadFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
        }
    }

}

