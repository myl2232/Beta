using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AlphaWork
{
    public class AvatarData : TargetableObjectData
    {
        [SerializeField]
        private string m_skeleton;
        [SerializeField]
        private List<string> m_parts = new List<string>();
        [SerializeField]
        private bool m_AlowMove = false;

        public AvatarData(int entityId = -1, int typeId = 80002, CampType camp = CampType.Unknown,bool alowMove = false)
            : base(entityId, typeId, camp)
        {
            m_AlowMove = alowMove;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AddPart(string part)
        {
            if (!m_parts.Contains(part))
                m_parts.Add(part);
        }


        public List<string> GetParts()
        {
            return m_parts;
        }

        //允许移动
        public bool AlowMove
        {
            get { return m_AlowMove; }
            set { m_AlowMove = value; }
        }
        /// <summary>
        /// 主骨架
        /// </summary>
        public  string Skeleton
        {
            get
            {
                return m_skeleton;
            }
            set
            {
                m_skeleton = value;
            }
        }
        public List<string> Parts
        {
            get { return m_parts; }
            set { m_parts = value; }
        }
        public override int MaxHP
        {
            get
            {
                return 0;
            }
        }
    }
}
