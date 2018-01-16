using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NavGridTool : MonoBehaviour
{
    public enum EPaint
    {
        EPAINT_OPENBLOCK,
        EPAINT_CLOSEBLOCK,
        EPAINT_SETHIGHT,
        EPAINT_NULL
    }
    private static Navigation.Grid m_grid;

    public static float BrushSize;
    public static float ViewSize;
    public static float MeshSize;
    public static float RayHeightField;
    public static float FlushHight;
    public static int Rows;
    public static int Columns;
    public static bool AccurateHight;
    public static bool bGenerate = false;

    private Vector3 StartPt = new Vector3();
    private Vector3 EndPt = new Vector3();
    private EPaint m_PaintType = EPaint.EPAINT_NULL;
    private Dictionary<List<Navigation.Grid.Position>, bool> m_path = new Dictionary<List<Navigation.Grid.Position>, bool>();
    private static List<List<float>> m_hightFields = new List<List<float>>();
    private static Vector3 mPos;

    public NavGridTool(int columns, int rows)
    {
        Columns = columns;
        Rows = rows;
    }

    public static void Initialize()
    {
        int realRow = (int)(Rows / MeshSize);
        int realColumns = (int)(Columns / MeshSize);
        if(m_grid == null)
            m_grid = new Navigation.Grid(realRow, realColumns);
       
        Refresh();
    }
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GenerateMesh()
    {
        Initialize();
        bGenerate = true;
    }

    public void SetStartPt(Vector3 pt)
    {
        StartPt = pt;
    }

    public void SetEndPt(Vector3 pt)
    {
        EndPt = pt;
    }

    public void ReadData()
    {

    }

    public void SaveData()
    {

    }

    public static void Refresh()
    {
        m_hightFields.Clear();
        for(int i = 0; i < Rows/MeshSize; ++i)
        {
            m_hightFields.Add(new List<float>());
            for(int j = 0; j < Columns/MeshSize; ++j)
            {
                m_hightFields[i].Add(GetZPos(i, j, true));
            }
        }
    }

    //private static Navigation.Grid.Position GetCameraPos(int x, int y)
    //{
    //    Navigation.Grid.Position pos = new Navigation.Grid.Position();
    //    pos.Row = (int)( x / MeshSize);
    //    pos.Column = (int)(y / MeshSize);
    //    return pos;
    //}
    private static float GetZPos(int x, int y, bool bRefresh = false)
    {
        if (!bRefresh)
        {
            if (m_hightFields.Count == 0|| x >= m_hightFields.Count || y >= m_hightFields[x].Count|| m_hightFields[x].Count == 0)
                Debug.LogWarning("error pos: x="+x+", y="+y);
            return m_hightFields[x][y];
        }
            

        float fValue = 0.0f;
        GameObject gbTerrain = GameObject.Find("Terrain") as GameObject;
        if (gbTerrain)
        {
            Terrain ter = gbTerrain.GetComponent<Terrain>();
            fValue = ter.terrainData.GetHeight(x, y);
        }

        if (AccurateHight)
        {
            RaycastHit hitInfo;
            Physics.Raycast(new Vector3(x,fValue+RayHeightField,y), new Vector3(0, -1, 0),out hitInfo);
            fValue = hitInfo.point.y;
        }

        return fValue;
    }

    [InitializeOnLoadMethod]
    static void Init()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
        Initialize();
    }

    private void OnDrawGizmos()
    {
        if (!IsActive || !bGenerate)
            return;
        //draw pointer       
        //Gizmos.color = Color.red;
        //mPos.y = GetZPos((int)mPos.x, (int)mPos.z, true);
        //Gizmos.DrawSphere(mPos, 5);

        float viewCenterX = transform.position.x;
        float viewCenterY = transform.position.z;

        //draw grid                
        for (int j = (int)((viewCenterY - ViewSize / MeshSize) > 0 ? (viewCenterY - ViewSize / MeshSize) : 0) + 1; (j < Columns / MeshSize) && j < (viewCenterY + ViewSize / MeshSize); ++j)
        {
            if (j + 1 >= Columns / MeshSize)
                break;
            for (int i = (int)(viewCenterX - ViewSize / MeshSize > 0 ? (viewCenterX - ViewSize / MeshSize) : 0) + 1; (i < Rows / MeshSize) && j < (viewCenterX + ViewSize / MeshSize); ++i)
            {
                if (i + 1 >= Rows / MeshSize)
                    break;
                Navigation.Grid.Position pMin = new Navigation.Grid.Position(i, j);
                Navigation.Grid.Position pMax = new Navigation.Grid.Position(i+1, j+1);

                float z1 = GetZPos(pMin.Row, pMin.Column);
                Vector3 p1 = new Vector3(pMin.Row * MeshSize, z1, pMin.Column * MeshSize);

                float z2 = GetZPos(pMin.Row, pMax.Column);
                Vector3 p2 = new Vector3(pMin.Row * MeshSize, z2, pMax.Column * MeshSize);

                float z3 = GetZPos(pMax.Row, pMax.Column);
                Vector3 p3 = new Vector3(pMax.Row * MeshSize, z3, pMax.Column * MeshSize);

                if(m_grid[pMin])
                {
                    Gizmos.color = Color.green;
                    Navigation.Grid.Position pos = new Navigation.Grid.Position(i - 1, j);
                    if(!m_grid[pos])
                        Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawLine(p1, p2);

                if (m_grid[pMin])
                {
                    Gizmos.color = Color.green;
                    Navigation.Grid.Position pos = new Navigation.Grid.Position(i, j+1);
                    if (!m_grid[pos])
                        Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawLine(p2, p3);
            }

        }
        //draw start.end
        //draw path
    }


    public static bool IsActive = true;
    private static Vector3 vNormal = new Vector3(0, 1, 0);
    
    static void OnSceneGUI(SceneView sceneView)
    {
        
        var current = Event.current;        

        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        if (IsActive)
        {
            if (current.type == EventType.Layout)
                HandleUtility.AddDefaultControl(controlID);
        }
        else
            bGenerate = false;
        
        switch (current.type)
        {
            case EventType.MouseMove:
                {
                    Camera cam = sceneView.camera;
                    Vector2 mousepos = Event.current.mousePosition;
                    mPos = sceneView.camera.ScreenToWorldPoint(mousepos);
                    mPos.z += 5;
                    Handles.DrawSolidDisc(mPos,vNormal, 5);

                    //Ray ray = sceneView.camera.ScreenPointToRay(mousepos.x, -mousepos.y);
                    

                }
                break;
            case EventType.MouseUp:
                //鼠标弹起，这里是鼠标所有的点击，如果要在区别如下
                if (current.button == 0)
                {

                }
                break;
            case EventType.MouseDown:
                //鼠标按下
                break;
            case EventType.MouseDrag:
                //鼠标拖
                break;
            case EventType.Repaint:
                //重绘
                break;
            case EventType.Layout:
                //布局
                break;
        }
    }

}
