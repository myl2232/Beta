using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class NavGridTool : MonoBehaviour
{
    public enum EPaint
    {
        EPAINT_OPENBLOCK,
        EPAINT_CLOSEBLOCK,
        EPAINT_SETHIGHT,
        EPAINT_NULL
    }
    public delegate void SyncBroadcast();
    public event SyncBroadcast SyncEvent;

    private  Navigation.Grid m_grid;
    [HideInInspector]
    public float BrushSize;
    [HideInInspector]
    public float ViewSize;
    [HideInInspector]
    public float MeshSize;
    [HideInInspector]
    public float RayHeightField;
    [HideInInspector]
    public float FlushHight;
    [HideInInspector]
    public int Rows;
    [HideInInspector]
    public int Columns;
    [HideInInspector]
    public bool AccurateHight;
    [HideInInspector]
    public bool bGenerate = false;
    [HideInInspector]
    public bool bOperate = false;
    [HideInInspector]
    public bool bStartPick = false;
    [HideInInspector]
    public bool bEndPick = false;
    [HideInInspector]
    public Vector3 StartPt;
    [HideInInspector]
    public Vector3 EndPt;
    [HideInInspector]
    public EPaint PaintType = EPaint.EPAINT_NULL;
    [HideInInspector]
    public bool IsActive = true;
    [HideInInspector]
    public GameObject gGiz = null;

    private List<List<float>> m_hightFields;// = new List<List<float>>();
    private LinkedList<Navigation.Grid.Position> m_path;// = new LinkedList<Navigation.Grid.Position>();
    private  Vector3 mPos;
    private  Transform selTransform;
    private  Vector3 mToolPos;
    private  string gStrHeader;
    private  int gVersion;

    public NavGridTool(int columns, int rows)
    {
        Columns = columns;
        Rows = rows;

#if UNITY_EDITOR
        Init();
#endif
    }

    public  void Initialize()
    {
        if (m_grid != null)
            m_grid = null;

        if (m_hightFields != null)
        {
            for (int i = 0; i < m_hightFields.Count; ++i)
                m_hightFields[i].Clear();
            m_hightFields.Clear();
        }
        else
            m_hightFields = new List<List<float>>();

        if (m_path == null)
            m_path = new LinkedList<Navigation.Grid.Position>();

        int realRow = (int)(Rows / MeshSize);
        int realColumns = (int)(Columns / MeshSize);
        
        if(m_grid == null && realRow > 0 && realColumns > 0)
            m_grid = new Navigation.Grid(realRow, realColumns);

        gStrHeader = "NavigationGrid Header:";
        gVersion = 1;
        mToolPos = gameObject.transform.position;

        Refresh();
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy()
    {
        DestroyImmediate(gGiz);
        gGiz = null;
    }

    public void GenerateMesh()
    {
        //Refresh();
        Initialize();
        bGenerate = true;
    }

    public void SetStartPt()
    {
        bStartPick = true;
        bEndPick = false;
    }

    public void SetEndPt()
    {
        bEndPick = true;
        bStartPick = false;
    }
    
    public  void Refresh()
    {
        m_hightFields.Clear();
        for(int i = 0; i < Rows/MeshSize; ++i)
        {
            m_hightFields.Add(new List<float>());
            for(int j = 0; j < Columns/MeshSize; ++j)
            {
                m_hightFields[i].Add(GetZPos(i , j , true));
            }
        }
    }

    private  float GetZPos(int x, int y, bool bRefresh = false)
    {
        if (!bRefresh)
        {
            if (m_hightFields.Count == 0 || x < 0 ||y < 0 || x >= m_hightFields.Count || y >= m_hightFields[x].Count || m_hightFields[x].Count == 0)
            {
                //Debug.LogWarning("error pos: x=" + x + ", y=" + y);
                return 0;
            }
            return m_hightFields[x][y];
        }

        float fValue = Mathf.Max(mToolPos.y, GetTerrainZ(x,y));      //GetTerrainZ(x, y);   
        
        if (AccurateHight)
        {
            RaycastHit hitInfo;
            Physics.Raycast(new Vector3(x*MeshSize,fValue+RayHeightField,y*MeshSize), new Vector3(0, -1, 0),out hitInfo);
            fValue = hitInfo.point.y;
        }

        return fValue;
    }

    //[InitializeOnLoadMethod]
    public void Init()
    {
#if UNITY_EDITOR
        SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
        //Initialize();
        if (gGiz == null)
        {
            gGiz = GameObject.CreatePrimitive(PrimitiveType.Quad);
            gGiz.transform.Rotate(new Vector3(1, 0, 0), 90);
            //gGiz.transform.parent = transform;
            gGiz.name = "NavTool_Giz";
        }
    }


#if UNITY_EDITOR
    public void ReadData()
    {
        gStrHeader = "NavigationGrid Header:";
        gVersion = 1;

        FileStream fs = new FileStream(GetFilePath() + "//GalaxyNavFile", FileMode.Open);
        BinaryReader binReader = new BinaryReader(fs);

        byte[] bBuffer = new byte[100];
        bBuffer = binReader.ReadBytes(gStrHeader.Length);
        gStrHeader = System.Text.Encoding.Default.GetString(bBuffer);
        gVersion = binReader.ReadInt32();
        Rows = binReader.ReadInt32();
        Columns = binReader.ReadInt32();
        MeshSize = binReader.ReadSingle();

        if (m_hightFields == null)
            m_hightFields = new List<List<float>>();
        if (m_path == null)
            m_path = new LinkedList<Navigation.Grid.Position>();

        //if (m_grid == null)
        int realRow = (int)(Rows / MeshSize);
        int realColumns = (int)(Columns / MeshSize);
        m_grid = null;
        m_grid = new Navigation.Grid(realRow, realColumns);

        for (int i = 0; i < realRow; ++i)
        {
            m_hightFields.Add(new List<float>());
            for (int j = 0; j < realColumns; ++j)
            {
                int nWalkable = binReader.ReadInt32();
                float hight = binReader.ReadSingle();
                m_hightFields[i].Add(hight);
                m_grid[new Navigation.Grid.Position(i, j)] = (nWalkable == 1 ? true : false);
            }
        }

        binReader.Close();
        fs.Close();

        SyncEvent();
    }

    public void SaveData()
    {
        if (!Directory.Exists(GetFilePath()))
            Directory.CreateDirectory(GetFilePath());

        string currentFile = GetFilePath() + "//GalaxyNavFile";
        FileStream fs = new FileStream(currentFile, FileMode.OpenOrCreate);
        BinaryWriter binWriter = new BinaryWriter(fs);

        binWriter.Write(gStrHeader.ToCharArray(), 0, gStrHeader.Length);
        binWriter.Write(gVersion);
        binWriter.Write(Rows);
        binWriter.Write(Columns);
        binWriter.Write(MeshSize);

        for (int i = 0; i < m_hightFields.Count; ++i)
        {
            for (int j = 0; j < m_hightFields[i].Count; ++j)
            {
                try
                {
                    bool bWalkable = m_grid[new Navigation.Grid.Position(i, j)];
                    //以行的形式写入信息  
                    binWriter.Write(bWalkable == true ? 1 : 0);
                    binWriter.Write(m_hightFields[i][j]);
                }
                catch
                {
                    Debug.LogError("wrong index: " + i + ", " + j);
                    return;
                }
            }
        }

        binWriter.Close();
        fs.Close();

        //temp        
        //string targetPath = GetTargetPath() + "//GalaxyNavFile";
        //System.IO.File.Copy(currentFile, targetPath, true);
    }

    private  string GetFilePath()
    {
        return Application.dataPath + "//Resources//NavGrid//" + EditorSceneManager.GetActiveScene().name;
    }

    private  string GetTargetPath()
    {
        return Application.dataPath + "//AlphaWork//Navigations//" + EditorSceneManager.GetActiveScene().name;
    }

    private void OnDrawGizmos()
    {
        if (!IsActive || m_grid == null)
            return;

        if (gGiz == null)
        {
            gGiz = GameObject.CreatePrimitive(PrimitiveType.Quad);
            gGiz.transform.Rotate(new Vector3(1, 0, 0), 90);
            //gGiz.transform.parent = transform;
            gGiz.name = "NavTool_Giz";
        }
        Vector3 pt = mPos;
        pt.y = GetZPos((int)pt.x, (int)pt.z) + 0.1f;
        gGiz.transform.position = pt;
        gGiz.transform.localScale = new Vector3(BrushSize, BrushSize, 0.1f);
        ////draw pointer     
        //Gizmos.color = Color.yellow;
        //Vector3[] ptLines = new Vector3[4];
        //if (BrushSize <= 1)
        //{
        //    ptLines[0].x = mPos.x;
        //    ptLines[0].z = mPos.z;
        //    ptLines[0].y = GetZPos((int)ptLines[0].x, (int)ptLines[0].z) + 0.1f;
        //    Gizmos.DrawWireCube(ptLines[0], new Vector3(0.5f, 0.1f, 0.5f));
        //}
        //else
        //{
        //    ptLines[0].x = mPos.x - (int)(BrushSize * 0.5);
        //    ptLines[0].z = mPos.z - (int)(BrushSize * 0.5);
        //    ptLines[0].y = GetZPos((int)ptLines[0].x, (int)ptLines[0].z) + 0.1f;
        //    ptLines[1].x = mPos.x - (int)(BrushSize * 0.5);
        //    ptLines[1].z = mPos.z + (int)(BrushSize * 0.5);
        //    ptLines[1].y = GetZPos((int)ptLines[1].x, (int)ptLines[1].z) + 0.1f;
        //    ptLines[2].x = mPos.x + (int)(BrushSize * 0.5);
        //    ptLines[2].z = mPos.z + (int)(BrushSize * 0.5);
        //    ptLines[2].y = GetZPos((int)ptLines[2].x, (int)ptLines[2].z) + 0.1f;
        //    ptLines[3].x = mPos.x + (int)(BrushSize * 0.5);
        //    ptLines[3].z = mPos.z - (int)(BrushSize * 0.5);
        //    ptLines[3].y = GetZPos((int)ptLines[3].x, (int)ptLines[3].z) + 0.1f;
        //    Gizmos.DrawLine(ptLines[0], ptLines[1]);
        //    Gizmos.DrawLine(ptLines[1], ptLines[2]);
        //    Gizmos.DrawLine(ptLines[2], ptLines[3]);
        //    Gizmos.DrawLine(ptLines[3], ptLines[0]);
        //}

        //draw this pos
        Gizmos.color = Color.black;
        float viewCenterX = transform.position.x;
        float viewCenterY = transform.position.z;
        Gizmos.DrawCube(transform.position, new Vector3(3,3,3));
        if (m_grid.NumColumns != Columns/MeshSize || m_grid.NumRows != Rows/MeshSize)
            return;
        //draw grid                
        for (int j = (int)((viewCenterY - ViewSize )/MeshSize > 0 ? (viewCenterY - ViewSize )/ MeshSize : 0) + 1; (j < Columns / MeshSize) && j < (viewCenterY + ViewSize) / MeshSize; ++j)
        {
            if (j + 1 >= Columns / MeshSize)
                break;
            for (int i = (int)((viewCenterX  - ViewSize )/ MeshSize > 0 ? (viewCenterX  - ViewSize )/ MeshSize : 0) + 1; (i < Rows / MeshSize) && i < (viewCenterX + ViewSize) / MeshSize; ++i)
            {
                if (i + 1 >= Rows / MeshSize)
                    break;
                Navigation.Grid.Position pMin = new Navigation.Grid.Position(Convert.ToInt32(i), Convert.ToInt32(j));
                Navigation.Grid.Position pMax = new Navigation.Grid.Position(Convert.ToInt32(i+1), Convert.ToInt32(j + 1));

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
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(StartPt, new Vector3(MeshSize*0.5f, MeshSize * 0.5f, MeshSize * 0.5f));
        Gizmos.DrawCube(EndPt, new Vector3(MeshSize * 0.5f, MeshSize * 0.5f, MeshSize * 0.5f));

        //draw path
        if(m_path.Count > 0)
        {
            IEnumerator iter = m_path.GetEnumerator();
            iter.MoveNext();
            
            Navigation.Grid.Position p1 = (Navigation.Grid.Position)iter.Current;
            Gizmos.DrawLine(StartPt, new Vector3(p1.Row,GetZPos(p1.Row,p1.Column),p1.Column));
            Navigation.Grid.Position p2 = p1;
            while (iter.MoveNext())
            {
                p2 = (Navigation.Grid.Position)iter.Current;
                Gizmos.DrawLine(new Vector3(p1.Row, GetZPos(p1.Row, p1.Column), p1.Column), new Vector3(p2.Row, GetZPos(p2.Row, p2.Column), p2.Column));
                p1 = p2;
            }
            Gizmos.DrawLine(new Vector3(p1.Row, GetZPos(p1.Row, p1.Column), p1.Column),EndPt);
        }

    }

     void OnSceneGUI(SceneView sceneView)
    {
        var current = Event.current;
        int button = Event.current.button;

        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        if (IsActive)
        {
            if (current.type == EventType.Layout)
                HandleUtility.AddDefaultControl(controlID);
        }
        else
        {
            m_grid = null;
            bGenerate = false;
        }
        switch (current.type)
        {
            case EventType.MouseMove:
                {
                    RaycastHit hit;
                    Vector2 mousePosition = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y);
                    Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition/*Event.current.mousePosition*/);                   
                    if (Physics.Raycast(ray, out hit))
                    {
                        mPos = hit.point;
                    }
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
                    RaycastHit hit;
                    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    if (Physics.Raycast(ray, out hit))
                    {
                        mPos = hit.point;
                    }

                    if (button == 0 && Event.current.isMouse && bOperate)
                    {
                        if (PaintType == EPaint.EPAINT_OPENBLOCK)
                        {
                            FlushWalkable(mPos, true);
                        }
                        else if (PaintType == EPaint.EPAINT_CLOSEBLOCK)
                        {
                            FlushWalkable(mPos, false);
                        }
                        else if (PaintType == EPaint.EPAINT_SETHIGHT)
                        {
                            FlushZ(mPos, FlushHight);
                        }
                    }

                    if (bStartPick)
                        StartPt = mPos;
                    else if (bEndPick)
                        EndPt = mPos;

                    FindPath();
                }
                break;
            case EventType.MouseDrag:
                //鼠标拖
                {
                    RaycastHit hit;
                    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                    if (Physics.Raycast(ray, out hit))
                    {
                        mPos = hit.point;
                    }
                    if (Selection.activeTransform != selTransform)
                    {
                        selTransform = Selection.activeTransform;
                    }
                    else if (button == 0 && Event.current.isMouse && bOperate )
                    {
                        if (PaintType == EPaint.EPAINT_OPENBLOCK)
                        {
                            FlushWalkable(mPos , true);
                        }
                        else if (PaintType == EPaint.EPAINT_CLOSEBLOCK)
                        {
                            FlushWalkable(mPos , false);
                        }
                        else if (PaintType == EPaint.EPAINT_SETHIGHT)
                        {
                            FlushZ(mPos , FlushHight);
                        }
                    }
                }
                break;
        }
    }
#endif

    private  void FindPath()
    {
        if (m_grid != null)
        {
            m_path.Clear();
            Navigation.Grid.Position st = new Navigation.Grid.Position((int)StartPt.x, (int)StartPt.z);
            Navigation.Grid.Position et = new Navigation.Grid.Position((int)EndPt.x, (int)EndPt.z);
            m_grid.FindPath(st,et,ref m_path);
        }            
    }

    private  void FlushWalkable(Vector3 center, bool bWalkable = false)
    {
        if (m_grid == null)
            return;

        for (int j = (int)((center.z - BrushSize*0.5) / MeshSize > 0 ? (center.z - BrushSize*0.5) / MeshSize : 0) + 1; (j < Columns / MeshSize) && j < (center.z + BrushSize*0.5) / MeshSize; ++j)
        {
            if (j + 1 >= Columns / MeshSize)
                break;
            for (int i = (int)((center.x - BrushSize*0.5) / MeshSize > 0 ? (center.x - BrushSize*0.5) / MeshSize : 0) + 1; (i < Rows / MeshSize) && i < (center.x + BrushSize*0.5) / MeshSize; ++i)
            {
                if (i + 1 >= Rows / MeshSize)
                    break;
                Navigation.Grid.Position pPos = new Navigation.Grid.Position(i, j);
                m_grid[pPos] = bWalkable;
            }
        }
    }

    private  void FlushZ(Vector3 center, float zHight)
    {
        if (m_grid == null)
            return;

        for (int j = (int)((center.z - BrushSize * 0.5) / MeshSize > 0 ? (center.z - BrushSize * 0.5) / MeshSize : 0) + 1; (j < Columns / MeshSize) && j < (center.z + BrushSize * 0.5) / MeshSize; ++j)
        {
            if (j + 1 >= Columns / MeshSize)
                break;
            for (int i = (int)((center.x - BrushSize * 0.5) / MeshSize > 0 ? (center.x - BrushSize * 0.5) / MeshSize : 0) + 1; (i < Rows / MeshSize) && i < (center.x + BrushSize * 0.5) / MeshSize; ++i)
            {
                if (i + 1 >= Rows / MeshSize)
                    break;
                //Navigation.Grid.Position pPos = new Navigation.Grid.Position(i, j);
                m_hightFields[i][j] = zHight;
            }
        }
    } 

    private  float GetTerrainZ(int x, int y)
    {
        Terrain[] terrains = FindObjectsOfType(typeof(Terrain)) as Terrain[];
        for (int i = 0; i < terrains.Length; ++i)
        {
            if(x <= terrains[i].terrainData.size.x && y <= terrains[i].terrainData.size.z)
                return terrains[i].terrainData.GetInterpolatedHeight(x*MeshSize, y*MeshSize);
        }

        //GameObject gbTerrain = GameObject.Find("Terrain") as GameObject;
        //if (gbTerrain)
        //{
        //    Terrain ter = gbTerrain.GetComponent<Terrain>();
        //    return ter.terrainData.GetHeight(x, y);
        //}

        return 0;
    }

#if UNITY_EDITOR

#endif
}
