using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;
using System;
using System.IO;

#if UNITY_EDITOR
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

    public Navigation.Grid m_grid;

    public float BrushSize
    { get; set; }
    public float ViewSize
    { get; set; }
    public float MeshSize
    { get; set; }
    public float RayHeightField
    { get; set; }
    public float FlushHight
    { get; set; }
    public int Rows
    { get; set; }
    public int Columns
    { get; set; }
    public bool AccurateHight
    { get; set; }
    public Vector3 StartPt
    { get; set; }
    public Vector3 EndPt
    { get; set; }
    public EPaint PaintType
    { get; set; }
    public Vector3 mPos
    { get; set; }
    public bool IsActive
    { get; set; }

    private List<List<float>> m_hightFields;
    private LinkedList<Navigation.Grid.Position> m_path;
    private Transform selTransform;

    private static string gStrHeader;
    private static int gVersion;

    public NavGridTool(int columns, int rows)
    {
        Columns = columns;
        Rows = rows;
        m_hightFields = new List<List<float>>();
        m_path = new LinkedList<Navigation.Grid.Position>();
    }

    public void Initialize()
    {
        int realRow = (int)(Rows / MeshSize);
        int realColumns = (int)(Columns / MeshSize);

        if (m_hightFields == null)
            m_hightFields = new List<List<float>>();
        if (m_path == null)
            m_path = new LinkedList<Navigation.Grid.Position>();
        if (/*m_grid == null &&*/ realRow > 0 && realColumns > 0)
            m_grid = new Navigation.Grid(realRow, realColumns);

        gStrHeader = "NavigationGrid Header:";
        gVersion = 1;

        Refresh();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsActive)
            m_grid = null;
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
        else
            m_hightFields.Clear();

        if (m_grid == null)
            m_grid = new Navigation.Grid((int)(Rows / MeshSize), (int)(Columns / MeshSize));

        for (int i = 0; i < Rows / MeshSize; ++i)
        {
            m_hightFields.Add(new List<float>());
            for (int j = 0; j < Columns / MeshSize; ++j)
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

        for (int i = 0; i < Rows / MeshSize; ++i)
        {
            for (int j = 0; j < Columns / MeshSize; ++j)
            {
                bool bWalkable = m_grid[new Navigation.Grid.Position(i, j)];
                //以行的形式写入信息  
                binWriter.Write(bWalkable == true ? 1 : 0);
                binWriter.Write(m_hightFields[i][j]);
            }
        }

        binWriter.Close();
        fs.Close();

        ////temp        
        string targetPath = GetTargetPath() + "//GalaxyNavFile";
        System.IO.File.Copy(currentFile, targetPath, true);
    }
#endif

    public void Refresh()
    {
        m_hightFields.Clear();
        for (int i = 0; i < Rows / MeshSize; ++i)
        {
            m_hightFields.Add(new List<float>());
            for (int j = 0; j < Columns / MeshSize; ++j)
            {
                m_hightFields[i].Add(GetZPos(i, j, true));
            }
        }
    }

    private float GetZPos(int x, int y, bool bRefresh = false)
    {
        if (!bRefresh)
        {
            if (m_hightFields.Count == 0 || x < 0 || y < 0 || x >= m_hightFields.Count || y >= m_hightFields[x].Count || m_hightFields[x].Count == 0)
            {
                Debug.LogWarning("error pos: x=" + x + ", y=" + y);
                return 0;
            }
            return m_hightFields[x][y];
        }

        float fValue = GetTerrainZ(x, y);

        if (AccurateHight)
        {
            RaycastHit hitInfo;
            Physics.Raycast(new Vector3(x, fValue + RayHeightField, y), new Vector3(0, -1, 0), out hitInfo);
            fValue = hitInfo.point.y;
        }

        return fValue;
    }

    private void OnDrawGizmos()
    {
        if (!IsActive || m_grid == null)
            return;
        //draw pointer     
        Gizmos.color = Color.yellow;
        Vector3[] ptLines = new Vector3[4];
        ptLines[0].x = mPos.x - (int)(BrushSize * 0.5);
        ptLines[0].z = mPos.z - (int)(BrushSize * 0.5);
        ptLines[0].y = GetZPos((int)ptLines[0].x, (int)ptLines[0].z) + 3;
        ptLines[1].x = mPos.x - (int)(BrushSize * 0.5);
        ptLines[1].z = mPos.z + (int)(BrushSize * 0.5);
        ptLines[1].y = GetZPos((int)ptLines[1].x, (int)ptLines[1].z) + 3;
        ptLines[2].x = mPos.x + (int)(BrushSize * 0.5);
        ptLines[2].z = mPos.z + (int)(BrushSize * 0.5);
        ptLines[2].y = GetZPos((int)ptLines[2].x, (int)ptLines[2].z) + 3;
        ptLines[3].x = mPos.x + (int)(BrushSize * 0.5);
        ptLines[3].z = mPos.z - (int)(BrushSize * 0.5);
        ptLines[3].y = GetZPos((int)ptLines[3].x, (int)ptLines[3].z) + 3;
        Gizmos.DrawLine(ptLines[0], ptLines[1]);
        Gizmos.DrawLine(ptLines[1], ptLines[2]);
        Gizmos.DrawLine(ptLines[2], ptLines[3]);
        Gizmos.DrawLine(ptLines[3], ptLines[0]);
        //draw this pos
        Gizmos.color = Color.black;
        float viewCenterX = transform.position.x;
        float viewCenterY = transform.position.z;
        Gizmos.DrawCube(transform.position, new Vector3(3, 3, 3));
        //draw grid                
        for (int j = (int)((viewCenterY - ViewSize) / MeshSize > 0 ? (viewCenterY - ViewSize) / MeshSize : 0) + 1; (j < Columns / MeshSize) && j < (viewCenterY + ViewSize) / MeshSize; ++j)
        {
            if (j + 1 >= Columns / MeshSize)
                break;
            for (int i = (int)((viewCenterX - ViewSize) / MeshSize > 0 ? (viewCenterX - ViewSize) / MeshSize : 0) + 1; (i < Rows / MeshSize) && i < (viewCenterX + ViewSize) / MeshSize; ++i)
            {
                if (i + 1 >= Rows / MeshSize)
                    break;
                Navigation.Grid.Position pMin = new Navigation.Grid.Position(i, j);
                Navigation.Grid.Position pMax = new Navigation.Grid.Position(i + 1, j + 1);

                float z1 = GetZPos(pMin.Row, pMin.Column);
                Vector3 p1 = new Vector3(pMin.Row * MeshSize, z1, pMin.Column * MeshSize);

                float z2 = GetZPos(pMin.Row, pMax.Column);
                Vector3 p2 = new Vector3(pMin.Row * MeshSize, z2, pMax.Column * MeshSize);

                float z3 = GetZPos(pMax.Row, pMax.Column);
                Vector3 p3 = new Vector3(pMax.Row * MeshSize, z3, pMax.Column * MeshSize);

                if (m_grid[pMin])
                {
                    Gizmos.color = Color.green;
                    Navigation.Grid.Position pos = new Navigation.Grid.Position(i - 1, j);
                    if (!m_grid[pos])
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
                    Navigation.Grid.Position pos = new Navigation.Grid.Position(i, j + 1);
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
        Gizmos.DrawCube(StartPt, new Vector3(MeshSize * 0.5f, MeshSize * 0.5f, MeshSize * 0.5f));
        Gizmos.DrawCube(EndPt, new Vector3(MeshSize * 0.5f, MeshSize * 0.5f, MeshSize * 0.5f));

        //draw path
        if (m_path != null && m_path.Count > 0)
        {
            IEnumerator iter = m_path.GetEnumerator();
            iter.MoveNext();

            Navigation.Grid.Position p1 = (Navigation.Grid.Position)iter.Current;
            Gizmos.DrawLine(StartPt, new Vector3(p1.Row, GetZPos(p1.Row, p1.Column), p1.Column));
            Navigation.Grid.Position p2 = p1;
            while (iter.MoveNext())
            {
                p2 = (Navigation.Grid.Position)iter.Current;
                Gizmos.DrawLine(new Vector3(p1.Row, GetZPos(p1.Row, p1.Column), p1.Column), new Vector3(p2.Row, GetZPos(p2.Row, p2.Column), p2.Column));
                p1 = p2;
            }
            Gizmos.DrawLine(new Vector3(p1.Row, GetZPos(p1.Row, p1.Column), p1.Column), EndPt);
        }

    }

    public void FindPath()
    {
        if (m_grid != null)
        {
            if (m_path == null)
                m_path = new LinkedList<Navigation.Grid.Position>();
            else
                m_path.Clear();

            Navigation.Grid.Position st = new Navigation.Grid.Position((int)StartPt.x, (int)StartPt.z);
            Navigation.Grid.Position et = new Navigation.Grid.Position((int)EndPt.x, (int)EndPt.z);
            m_grid.FindPath(st, et, m_path);
        }

    }

    public void FlushWalkable(Vector3 center, bool bWalkable = false)
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
                Navigation.Grid.Position pPos = new Navigation.Grid.Position(i, j);
                m_grid[pPos] = bWalkable;
            }
        }
    }

    public void FlushZ(Vector3 center, float zHight)
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

    private float GetTerrainZ(int x, int y)
    {
        Terrain[] terrains = FindObjectsOfType(typeof(Terrain)) as Terrain[];
        for (int i = 0; i < terrains.Length; ++i)
        {
            if (x <= terrains[i].terrainData.size.x && y <= terrains[i].terrainData.size.z)
                return terrains[i].terrainData.GetHeight(x, y);
        }

        return 0;
    }

#if UNITY_EDITOR
    private string GetFilePath()
    {
        return Application.dataPath + "//Resources//NavGrid//" + EditorSceneManager.GetActiveScene().name;
    }

    private string GetTargetPath()
    {
        return Application.dataPath + "//AlphaWork//Navigations//" + EditorSceneManager.GetActiveScene().name;
    }
#endif
}
