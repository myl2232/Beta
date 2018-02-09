using GameFramework.Event;
using GameFramework.Resource;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using UnityStandardAssets.CrossPlatformInput;

namespace AlphaWork
{
    public class AttachmentEntity : EntityObject
	{
        public AttachmentEntity()
        {
            GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
        }

        // Use this for initialization
        void Start()
		{
        }

		// Update is called once per frame
		void Update()
		{
        }

#if UNITY_2017_3_OR_NEWER
        protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
#else
        protected internal override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
#endif
        {
            base.OnAttachTo(parentEntity, parentTransform, userData);

            AttachmentData data = userData as AttachmentData;

            GameObject gb = gameObject;
            gb.transform.parent = AssetUtility.FindChild(parentTransform, data.Bone).parent;
            gb.transform.position = AssetUtility.FindChild(parentTransform, data.Bone).position;//parentTransform.position + data.AttachPos;
            gb.transform.localScale = data.AttachScale;
            gb.transform.rotation = AssetUtility.FindChild(parentTransform, data.Bone).rotation;
//             gb.transform.rotation = new Quaternion(data.AttachRotate.x, data.AttachRotate.y,
//                 data.AttachRotate.z, 0);

        }

        protected virtual void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            if (ne.EntityLogicType == typeof(AttachmentEntity))
            {
                AttachmentData userData = ne.UserData as AttachmentData;
                if(Id == userData.Id)
                {
                    GameEntry.Entity.AttachEntity(Id, userData.ParentId, userData);
                }
            }
        }

        protected virtual void OnShowEntityFailure(object sender, GameEventArgs e)
        {
        }

    }

}

