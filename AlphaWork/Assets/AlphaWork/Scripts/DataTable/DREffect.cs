using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFramework.DataTable;

namespace AlphaWork
{
    class DREffect : IDataRow
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
        /// 伤害值。
        /// </summary>
        public float HitHP
        {
            get;
            private set;
        }
        public float Speed
        {
            get;
            private set;
        }
        public float LifeTime
        {
            get;
            private set;
        }
        public string AssetName
        {
            get;
            private set;
        }
        public string AttachName
        {
            get;
            private set;
        }
        public string AttachOffset
        {
            get;
            private set;
        }
        public string AttachRot
        {
            get;
            private set;
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
            AssetName = text[index++];
            HitHP = float.Parse(text[index++]);
            Speed = float.Parse(text[index++]);
            LifeTime = float.Parse(text[index++]);
            AttachName = text[index++];
            AttachOffset = text[index++];
            AttachRot = text[index++];
        }
    }
}
