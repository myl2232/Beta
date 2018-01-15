using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavGridWindow))]
public class NavGridWindow : EditorWindow {

    private NavGridTool m_gridTool;
    Vector3 ptStart;
    Vector3 ptEnd;
    //通过MenuItem按钮来创建这样的一个对话框  
    [MenuItem("Window/NavGridWindow")]
    public static void ConfigDialog()
    {
        EditorWindow.GetWindow(typeof(NavGridWindow));
    }

    // Use this for initialization
    void Start () {
        m_gridTool = new NavGridTool();
        ptStart = new Vector3(1,1,1);
        ptEnd = new Vector3(1,1,1);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {        
        GUILayout.BeginArea(new Rect(10, 10, 300, 1000));
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("画刷大小：", EditorStyles.boldLabel);
        GUILayout.HorizontalSlider(1, 0, 10);
        EditorGUILayout.DelayedIntField(1, GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("可视范围：", EditorStyles.boldLabel);
        GUILayout.HorizontalSlider(1, 0, 10);
        EditorGUILayout.DelayedIntField(1, GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("网格大小：", EditorStyles.boldLabel);
        GUILayout.HorizontalSlider(1, 0, 10);
        EditorGUILayout.DelayedIntField(1, GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();
        GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
        
        GUILayout.BeginArea(new Rect(10, 80, 300, 1000));
        EditorGUILayout.Toggle("启动精准高度", false);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("自地面以上射线高度：", EditorStyles.boldLabel);
        GUILayout.HorizontalSlider(1, 0, 100);
        EditorGUILayout.DelayedIntField(1, GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("生成网格", GUILayout.Width(300)))
        {
            GenerateMesh();
        }
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(10, 140, 300, 500));
        GUILayout.Label("基本操作：", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Toggle("开启通路", true);
        EditorGUILayout.Toggle("关闭通路", false);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Toggle("刷新Z值", false);
        GUILayout.HorizontalSlider(1, 0, 10);
        EditorGUILayout.DelayedIntField(1, GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(10, 220, 300, 500));
        GUILayout.Label("设定寻路点：", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("起始点", GUILayout.Width(120)))
        {
            SetStartPt();
        }
        EditorGUILayout.Vector3Field("",ptStart);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("终止点", GUILayout.Width(120)))
        {
            SetEndPt();
        }
        EditorGUILayout.Vector3Field("", ptStart);
        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(10, 300, 300, 500));
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
        GUILayout.EndArea();
    }

    private void GenerateMesh()
    {
        m_gridTool.GenerateMesh();
    }

    private void SetStartPt()
    {

    }

    private void SetEndPt()
    {

    }

    private void ReadData()
    {

    }

    private void SaveData()
    {

    }

    private void OnInspectorUpdate()
    {
        
    }


}
