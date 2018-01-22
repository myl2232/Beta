using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AlphaWork.Editor
{
    public class NavGridWindow : EditorWindow
    {
        int m_columns = 64;
        int m_rows = 64;
        float m_brushSize = 5.0f;
        float m_viewSize = 40.0f;
        float m_meshSize = 1f;
        bool m_bAccurateToggle = false;
        bool m_bOperateToggle = false;
        float m_RayHeightField = 50.0f;
        bool m_bOpenBlock = false;
        bool m_bCloseBlock = true;
        bool m_bFlushHeight = false;
        float m_fFlushHight = 0.0f;
        Vector3 m_StartPt = new Vector3();
        Vector3 m_EndPt = new Vector3();
        NavGridTool m_gridTool;     
        bool bActive = false;
        public Vector3 mousePos = new Vector3();


        private NavGridTool.EPaint PaintType = NavGridTool.EPaint.EPAINT_NULL;
        private bool bStartPick = false;
        private bool bEndPick = false;

        [MenuItem("Window/NavGridWindow")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow sWin = EditorWindow.GetWindow(typeof(NavGridWindow));
            sWin.Show();
        }

        void Awake()
        {
            
        }

        private void OnEnable()
        {
            SceneView.onSceneGUIDelegate += this.OnSceneGUI; 
        }

        void OnDestroy()
        {
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
            if (m_gridTool != null)
                m_gridTool.IsActive = false;
        }

        void OnGUI()
        {
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
            GUILayout.Label("自地面以上射线高度：", EditorStyles.boldLabel);
            m_RayHeightField = GUILayout.HorizontalSlider(m_RayHeightField, 0, 1000);
            m_RayHeightField = EditorGUILayout.DelayedFloatField(m_RayHeightField, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndToggleGroup();
            if (GUILayout.Button("生成网格", GUILayout.Width(300)))
            {
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
                ReadData();
            }
            if (GUILayout.Button("保存寻路数据", GUILayout.Width(150)))
            {
                SaveData();
            }
            EditorGUILayout.EndHorizontal();
            //GUILayout.EndArea();
            EditorGUILayout.EndToggleGroup();

            if (bActive)
                Sysnc();
        }
        

        public void OnSceneGUI(SceneView sceneView)
        {
            var current = Event.current;
            int button = Event.current.button;

            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            if (bActive)
            {
                if (current.type == EventType.Layout)
                    HandleUtility.AddDefaultControl(controlID);
            }
            else
            {
                if(m_gridTool != null)
                    m_gridTool.m_grid = null;
            }
            switch (current.type)
            {
                case EventType.MouseMove:
                    {

                        current.Use();
                    }
                    break;
                case EventType.MouseUp:
                    //鼠标弹起
                    {
                        bStartPick = false;
                        bEndPick = false;
                    }
                    break;
                case EventType.MouseDown:
                    //鼠标按下
                    {
                        if (m_gridTool == null)
                            return;

                        RaycastHit hit;
                        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                        if (Physics.Raycast(ray, out hit))
                        {
                            mousePos = hit.point;
                            m_gridTool.mPos = mousePos;
                        }

                        if (bStartPick)
                            m_StartPt = mousePos;
                        else if (bEndPick)
                            m_EndPt =  mousePos;

                    }
                    break;
                case EventType.MouseDrag:
                    //鼠标拖
                    {
                        if (m_gridTool == null)
                            return;

                        RaycastHit hit;
                        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                        if (Physics.Raycast(ray, out hit))
                        {
                            mousePos = hit.point;
                            m_gridTool.mPos = mousePos;
                        }
                        
                        if (button == 0 && Event.current.isMouse && m_bOperateToggle)
                        {
                            if (PaintType == NavGridTool.EPaint.EPAINT_OPENBLOCK)
                            {
                                m_gridTool.FlushWalkable(mousePos, true);
                            }
                            else if (PaintType == NavGridTool.EPaint.EPAINT_CLOSEBLOCK)
                            {
                                m_gridTool.FlushWalkable(mousePos, false);
                            }
                            else if (PaintType == NavGridTool.EPaint.EPAINT_SETHIGHT)
                            {
                                m_gridTool.FlushZ(mousePos, m_fFlushHight);
                            }
                        }
                    }
                    break;
            }
        }

        private void Sysnc(bool bGenerate = false)
        {
            if (m_gridTool == null)
            {
                m_gridTool = FindObjectOfType<NavGridTool>();
                m_gridTool.SyncEvent += OnSync;
            }
            
            m_gridTool.IsActive = bActive;
            m_gridTool.BrushSize = m_brushSize;
            m_gridTool.ViewSize = m_viewSize;
            if(bGenerate)
                m_gridTool.MeshSize = m_meshSize;
            m_gridTool.RayHeightField = m_RayHeightField;
            m_gridTool.FlushHight = m_fFlushHight;
            m_gridTool.AccurateHight = m_bAccurateToggle;
            m_gridTool.Rows = m_rows;
            m_gridTool.Columns = m_columns;

            if (m_bOpenBlock)
                PaintType = NavGridTool.EPaint.EPAINT_OPENBLOCK;
            else if (m_bCloseBlock)
                PaintType = NavGridTool.EPaint.EPAINT_CLOSEBLOCK;
            else
                PaintType = NavGridTool.EPaint.EPAINT_SETHIGHT;

            m_gridTool.StartPt = m_StartPt;
            m_gridTool.EndPt = m_EndPt;

            m_gridTool.FindPath();
        }

        private void OnSync()
        {
            m_brushSize = m_gridTool.BrushSize;
            m_meshSize = m_gridTool.MeshSize;
            m_rows = m_gridTool.Rows;
            m_columns = m_gridTool.Columns;
        }

        private void GenerateMesh()
        {
            Sysnc(true);
            m_gridTool.Initialize();            
        }

        private void SetStartPt()
        {
            bStartPick = true;
            bEndPick = false;
        }

        private void SetEndPt()
        {
            bEndPick = true;
            bStartPick = false;
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