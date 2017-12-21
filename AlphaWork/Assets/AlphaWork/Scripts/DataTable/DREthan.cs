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
		}
	}

}
