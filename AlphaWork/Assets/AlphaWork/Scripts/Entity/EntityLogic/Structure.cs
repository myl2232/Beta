using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlphaWork
{
    public class Structure : EntityObject
	{
        public int m_TypeId;
        private Vector3 lockPos;
        private StructureData m_data = null;
        private Texture m_originalTex = null;
        private bool m_Occupyed = false;
        protected int m_weight = 1;

        public int Weight
        {
            get { return m_weight; }
            set { m_weight = value; }
        }
        public Texture OriginalTex
        {
            get { return m_originalTex; }
        }
        public bool Occupyed
        {
            get { return m_Occupyed; }
            set { m_Occupyed = value; }
        }
		// Use this for initialization
		void Start()
		{
            m_TypeId = TypeId;
        }

		// Update is called once per frame
		void Update()
		{
//             if (CachedTransform.position.y > (lockPos.y + 0.1))
//                 CachedTransform.position += new Vector3(0, -0.1f, 0); 
        }

        protected internal override void OnShow(object userdata)
        {
            base.OnShow(userdata);
            m_data = userdata as StructureData;

            CachedTransform.position += new Vector3(0,20,0);

            RaycastHit hitResult;            
            if (Physics.Raycast(CachedTransform.position, Vector3.down, out hitResult))
            {
                lockPos = hitResult.point;
            }
            CachedTransform.position = lockPos;

            GameObject gb = GameEntry.Entity.GetEntity(Id).Handle as GameObject;
            if(gb)
            {
                m_originalTex = gb.GetComponent<MeshRenderer>().material.mainTexture;
            }
        }

        protected internal override void OnHide(object userData)
        {
            base.OnHide(userData);

        }
        public string GetReplaceTex()
        {
            return m_data.ReplaceTex;
        }
        
    }
}

