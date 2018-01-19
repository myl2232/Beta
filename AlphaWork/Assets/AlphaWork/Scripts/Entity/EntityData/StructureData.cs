using GameFramework.DataTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlphaWork
{
    public class StructureData : TargetableObjectData
	{
		[SerializeField]
		private int m_MaxHP = 0;
        private string m_replaceTex;

        public int maxHP
        {
            get { return m_MaxHP; }
        }
        public string ReplaceTex
        {
            get { return m_replaceTex; }
        }

		public StructureData(int entityId, int typeId)
            : base(entityId, typeId, CampType.Unknown)
        {
            IDataTable<DRStructure> dRStructure = GameEntry.DataTable.GetDataTable<DRStructure>();
            DRStructure dtStructure = dRStructure.GetDataRow(TypeId);
            if (dtStructure == null)
            {
                return;
            }

            m_MaxHP = dtStructure.MaxHP;
            m_replaceTex = dtStructure.ReplaceTex;

        }
        // Use this for initialization
        void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}

		///// <summary>
		///// 最大生命。
		///// </summary>
		//public override int MaxHP
		//{
		//	get
		//	{
		//		return m_MaxHP;
		//	}
		//}
	}
}

