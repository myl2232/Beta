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
            Id = int.Parse(text[index]);
            index++;
            string str = text[index];
			m_Data.Skeleton = str;
            for (int i = 3; i < text.Length-2; ++i)
            {
                index++;
                m_Data.AddPart(text[index]);
            }
            index++;
            m_Data.SenseRadius = float.Parse(text[index]);
            index++;
            m_Data.AttackRadius = float.Parse(text[index]);
        }
	}

}
