﻿using System;
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

        public AvatarData(int entityId = -1, int typeId = 80002, CampType camp = CampType.Unknown)
            : base(entityId, typeId, camp)
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

        public void AddPart(string part)
        {
            if (!m_parts.Contains(part))
                m_parts.Add(part);
        }

        public void GetParts(ref List<string> parts)
        {
            parts = m_parts;
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
