using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.DataTable;

namespace AlphaWork
{
	public class DRAvatar : IDataRow
	{
		/// <summary>
		/// 编号。
		/// </summary>
		public int Id
		{
			get;
			set;
		}

        //      /// <summary>
        ///// 最大生命值。
        ///// </summary>
        public int MaxHP
        {
            get;
            private set;
        }

        private AvatarData m_Data = new AvatarData();
        public AvatarData Data
        {
            get { return m_Data; }
            //set { m_Data = value; }
        }

        // Use this for initialization
        void Start()
		{
            
		}

		// Update is called once per frame
		void Update()
		{

		}

		public void ParseDataRow(string dataRowText)
		{
			string[] text = DataTableExtension.SplitDataRow(dataRowText);
			int index = 0;
            index++;
            Id = int.Parse(text[index++]);
            m_Data.AvatarName = text[index++];
            m_Data.AI = text[index++];
            m_Data.walkSpeed = float.Parse(text[index++]);
            m_Data.runSpeed = float.Parse(text[index++]);
            m_Data.sprintSpeed = float.Parse(text[index++]);
            m_Data.SenseRadius = float.Parse(text[index++]);
            m_Data.AttackRadius = float.Parse(text[index++]);
            m_Data.MaxHP = int.Parse(text[index++]);          
			m_Data.Skeleton = text[index++];

            for (int i = index; i < text.Length; ++i,index++)
            {                
                m_Data.AddPart(text[index]);
            }
        }

    }

}
