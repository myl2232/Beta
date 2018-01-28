using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.DataTable;

namespace AlphaWork
{
	public class DREthan : IDataRow
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
        public float walkSpeed;
        public float runSpeed;
        public float sprintSpeed;
        public float baseSpeed;
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
            MaxHP = int.Parse(text[index++]);
            string AssetName = text[index++];
            walkSpeed = float.Parse(text[index++]);
            runSpeed = float.Parse(text[index++]);
            sprintSpeed = float.Parse(text[index++]);
            baseSpeed = float.Parse(text[index++]);            
        }
	}

}
