using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.DataTable;
using System;
using System.Globalization;

namespace AlphaWork
{
	public class DRAttachment : IDataRow
	{
		/// <summary>
		/// 编号。
		/// </summary>
		public int Id
		{
			get;
			set;
		}
        //相对绑定骨骼的位置
        private Vector3 m_Trans;
        public Vector3 Position
        {
            get { return m_Trans; }
        }
        //相对绑定骨骼的缩放大小
        private Vector3 m_scale;
        public Vector3 Scale
        {
            get { return m_scale; }
        }
        //相对绑定骨骼的旋转
        private Vector3 m_rotate;
        public Vector3 Rotate
        {
            get { return m_rotate; }
        }
        //主骨架
        private string m_skeleton;
        public string Skeleton
        {
            get { return m_skeleton; }
        }
        //绑定骨骼点
        private string m_bone;
        public string Bone
        {
            get { return m_bone; }
        }
        //武器资源
        private string m_weapon;
        public string Weapon
        {
            get { return m_weapon; }
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
            m_skeleton = text[index++];
            m_weapon = text[index++];
            m_bone = text[index++].Replace("\"","");            
            string[] pos = text[index++].Replace("\"", "").Split(',');
            m_Trans = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));     
            string[] scale = text[index++].Replace("\"", "").Split(',');
            m_scale = new Vector3(float.Parse(scale[0]), float.Parse(scale[1]), float.Parse(scale[2]));
            string[] rotate = text[index++].Replace("\"", "").Split(',');
            m_rotate = new Vector3(float.Parse(rotate[0]), float.Parse(rotate[1]), float.Parse(rotate[2]));

        }
    }

}
