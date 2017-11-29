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
        }

        // Use this for initialization
        void Start()
		{
        }

		// Update is called once per frame
		void Update()
		{
        }

        protected internal override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
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


    }

}

