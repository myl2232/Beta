using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFramework.DataTable;

namespace AlphaWork
{
    public class DRNPC : IDataRow
    {
        /// <summary>
		/// 编号。
		/// </summary>
		public int Id
        {
            get;
            private set;
        }
        /// <summary>
        /// 最大生命值。
        /// </summary>
        public int MaxHP
        {
            get;
            private set;
        }
        public string AssetName;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private NPCData m_Data = new NPCData();
        public NPCData Data
        {
            get { return m_Data; }
            //set { m_Data = value; }
        }

        public void ParseDataRow(string dataRowText)
        {
            string[] text = DataTableExtension.SplitDataRow(dataRowText);
            int index = 0;
            index++;
            Id = int.Parse(text[index++]);
            AssetName = text[index++];
            m_Data.MaxHP = int.Parse(text[index++]);
            m_Data.SenseRadius = float.Parse(text[index++]);
            m_Data.AttackRadius = float.Parse(text[index++]);
            m_Data.AI = text[index++];
            m_Data.walkSpeed = float.Parse(text[index++]);
            m_Data.runSpeed = float.Parse(text[index++]);
            m_Data.sprintSpeed = float.Parse(text[index++]);
        }
    }
}
