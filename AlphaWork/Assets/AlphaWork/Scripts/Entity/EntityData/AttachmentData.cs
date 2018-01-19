using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public class AttachmentData : TargetableObjectData
    {
        [SerializeField]
        private int m_parentEntityId;

        [SerializeField]
        private Transform m_Transform;
        [SerializeField]
        private GameObject m_Attachment;
        [SerializeField]
        private string m_Bone;
        [SerializeField]
        private Vector3 m_AttachPos;
        [SerializeField]
        private Vector3 m_AttachScale;
        [SerializeField]
        private Vector3 m_AttachRotate;

        public AttachmentData(int entityId = -1, int typeId = 60001, CampType camp = CampType.Unknown)
            : base(entityId, typeId, camp)
        {
            //m_parentEntityId = parentId;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public int ParentId
        {
            get { return m_parentEntityId; }
            set { m_parentEntityId = value; }
        }

        public Vector3 AttachPos
        {
            get
            {
                return m_AttachPos;
            }
            set
            {
                m_AttachPos = value;
            }
        }
        public Vector3 AttachRotate
        {
            get
            {
                return m_AttachRotate;
            }
            set
            {
                m_AttachRotate = value;
            }
        }
        public Vector3 AttachScale
        {
            get
            {
                return m_AttachScale;
            }
            set
            {
                m_AttachScale = value;
            }
        }


        public string Bone
        {
            get { return m_Bone; }
            set { m_Bone = value; }
        }
        public Transform AttachTrans
        {
            get { return m_Transform; }
            set { m_Transform = value; }
        }
        public GameObject Attachment
        {
            get { return m_Attachment; }
            set { m_Attachment = value; }
        }
        //public override int MaxHP
        //{
        //    get
        //    {
        //        return 0;
        //    }
        //}
    }
}
