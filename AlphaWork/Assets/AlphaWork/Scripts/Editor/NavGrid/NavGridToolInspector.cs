using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AlphaWork.Editor
{
    [CustomEditor(typeof(NavGridTool))]
    class NavGridToolInspector : UnityEditor.Editor
    {        
        int m_columns = 64;
        int m_rows = 64;
        float m_brushSize = 5.0f;
        float m_viewSize = 40.0f;
        float m_meshSize = 1f;
        bool m_bAccurateToggle = false;
        bool m_bOperateToggle = false;
        float m_RayHeightField = 50.0f;
        float m_RayEndField = 50.0f;
        bool m_bOpenBlock = false;
        bool m_bCloseBlock = true;
        bool m_bFlushHeight = false;
        float m_fFlushHight = 0.0f;
        Vector3 m_StartPt = new Vector3();
        Vector3 m_EndPt = new Vector3();
        NavGridTool m_gridTool;
        int layoutExtent = 400;
        static bool bActive = false;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            m_gridTool = (NavGridTool)target;
            m_gridTool.SyncEvent += OnSync;
            

            //GUILayout.BeginArea(new Rect(10, 10 + layoutExtent, 300, 1000));
            bActive = EditorGUILayout.BeginToggleGroup("激活", bActive);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("长：", EditorStyles.boldLabel);
            m_rows = EditorGUILayout.DelayedIntField(m_rows, GUILayout.Width(100));
            GUILayout.Label("宽：", EditorStyles.boldLabel);
            m_columns = EditorGUILayout.DelayedIntField(m_columns, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("画刷大小：", EditorStyles.boldLabel);
            m_brushSize = GUILayout.HorizontalSlider(m_brushSize, 0, 10);
            m_brushSize = EditorGUILayout.DelayedFloatField(m_brushSize, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("可视范围：", EditorStyles.boldLabel);
            m_viewSize = GUILayout.HorizontalSlider(m_viewSize, 0, 1000);
            m_viewSize = EditorGUILayout.DelayedFloatField(m_viewSize, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("网格大小：", EditorStyles.boldLabel);
            m_meshSize = GUILayout.HorizontalSlider(m_meshSize, 0, 10);
            m_meshSize = EditorGUILayout.DelayedFloatField(m_meshSize, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            //GUILayout.EndArea();
            EditorGUILayout.Space();
            //GUILayout.BeginArea(new Rect(10, 100 + layoutExtent, 300, 1000));
            m_bAccurateToggle = EditorGUILayout.BeginToggleGroup("精准高度", m_bAccurateToggle);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("投射点高度（自地形或工具高度起始）：", EditorStyles.boldLabel);
            m_RayHeightField = GUILayout.HorizontalSlider(m_RayHeightField, 0, 1000);
            m_RayHeightField = EditorGUILayout.DelayedFloatField(m_RayHeightField, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("最大投射高度（自地形或工具高度起始）：", EditorStyles.boldLabel);
            m_RayEndField = GUILayout.HorizontalSlider(m_RayEndField, 0, 1000);
            m_RayEndField = EditorGUILayout.DelayedFloatField(m_RayEndField, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndToggleGroup();
            if (GUILayout.Button("生成网格", GUILayout.Width(300)))
            {
                if(EditorUtility.DisplayDialog("提示", "是否重新生成网格？", "确认", "取消"))
                    GenerateMesh();
            }
            //GUILayout.EndArea();

            //GUILayout.BeginArea(new Rect(10, 160 + layoutExtent, 300, 500));
            m_bOperateToggle = EditorGUILayout.BeginToggleGroup("基本画刷操作（只选一项）", m_bOperateToggle);
            EditorGUILayout.BeginVertical();
            m_bOpenBlock = EditorGUILayout.Toggle("开启通路", m_bOpenBlock);
            m_bCloseBlock = EditorGUILayout.Toggle("关闭通路", m_bCloseBlock);
            EditorGUILayout.BeginHorizontal();
            m_bFlushHeight = EditorGUILayout.Toggle("刷新Z值", m_bFlushHeight);
            m_fFlushHight = GUILayout.HorizontalSlider(m_fFlushHight, -1000, 1000);
            m_fFlushHight = EditorGUILayout.DelayedFloatField(m_fFlushHight, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndToggleGroup();
            //GUILayout.EndArea();

            //GUILayout.BeginArea(new Rect(10, 240 + layoutExtent, 300, 500));
            GUILayout.Label("设定寻路点：", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("起始点", GUILayout.Width(120)))
            {
                SetStartPt();
            }
            m_StartPt = EditorGUILayout.Vector3Field("", m_StartPt);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("终止点", GUILayout.Width(120)))
            {
                SetEndPt();
            }
            m_EndPt = EditorGUILayout.Vector3Field("", m_EndPt);
            EditorGUILayout.EndHorizontal();
            //GUILayout.EndArea();

            //GUILayout.BeginArea(new Rect(10, 320 + layoutExtent, 300, 500));
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("读取寻路数据", GUILayout.Width(150)))
            {
                if (EditorUtility.DisplayDialog("提示", "是否读取寻路数据？", "确认", "取消"))
                    ReadData();
            }
            if (GUILayout.Button("保存寻路数据", GUILayout.Width(150)))
            {
                if (EditorUtility.DisplayDialog("提示", "是否覆盖保存寻路数据？", "确认", "取消"))
                    SaveData();
            }
            EditorGUILayout.EndHorizontal();
            //GUILayout.EndArea();
            EditorGUILayout.EndToggleGroup();

            Sysnc();
        }

        private void Sysnc()
        {
            m_gridTool.IsActive = bActive;
            if(bActive && m_gridTool)
                m_gridTool.Init();
            m_gridTool.BrushSize = m_brushSize;
            m_gridTool.ViewSize = m_viewSize;
            m_gridTool.MeshSize = m_meshSize;
            m_gridTool.RayHeightField = m_RayHeightField;
            m_gridTool.RayEndField = m_RayEndField;
            m_gridTool.FlushHight = m_fFlushHight;
            m_gridTool.AccurateHight = m_bAccurateToggle;
            m_gridTool.X = m_rows;
            m_gridTool.Y = m_columns;
            m_gridTool.bOperate = m_bOperateToggle;
            if (m_bOpenBlock)
                m_gridTool.PaintType = NavGridTool.EPaint.EPAINT_OPENBLOCK;
            else if (m_bCloseBlock)
                m_gridTool.PaintType = NavGridTool.EPaint.EPAINT_CLOSEBLOCK;
            else
                m_gridTool.PaintType = NavGridTool.EPaint.EPAINT_SETHIGHT;

            m_StartPt = m_gridTool.StartPt;
            m_EndPt = m_gridTool.EndPt;
        }

        private void OnSync()
        {            
            m_brushSize = m_gridTool.BrushSize;
            m_meshSize = m_gridTool.MeshSize;
            m_rows = m_gridTool.X;
            m_columns = m_gridTool.Y;
        }

        private void GenerateMesh()
        {
            m_gridTool.GenerateMesh();
        }

        private void SetStartPt()
        {
            m_gridTool.SetStartPt();
        }

        private void SetEndPt()
        {
            m_gridTool.SetEndPt();
        }

        private void ReadData()
        {
            m_gridTool.ReadData();
        }

        private void SaveData()
        {
            m_gridTool.SaveData();
        }
    }
}
