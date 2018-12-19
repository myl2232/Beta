using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;

namespace AlphaWork
{
    public class NavGridComponent : GameFrameworkComponent
    {
        private Navigation.Grid m_ActiveGrid;
        public int Rows;
        public int Columns;
        private string gStrHeader;
        private int gVersion;
        private List<List<float>> m_hightFields = new List<List<float>>();
        private LinkedList<Navigation.Grid.Position> m_path = new LinkedList<Navigation.Grid.Position>();
        private float Threshold = 0.1f;
        public float MeshSize = 0.0f;

        protected override void Awake()
        {
            base.Awake();

            gStrHeader = "NavigationGrid Header:";
            gVersion = 1;
        }


        private void Update()
        {
            
        }

        public void ReadData(string scene = "")
        {
            gStrHeader = "NavigationGrid Header:";

            if (scene == "")
                scene = SceneManager.GetActiveScene().name;
            string path = GetFilePath() + scene + "//GalaxyNavFile";

            if (!File.Exists(path))
                return;
            else
                m_ActiveGrid = null;

            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader binReader = new BinaryReader(fs);      

            byte[] bBuffer = new byte[100];
            bBuffer = binReader.ReadBytes(gStrHeader.Length);
            gStrHeader = System.Text.Encoding.Default.GetString(bBuffer);
            gVersion = binReader.ReadInt32();
            Rows = binReader.ReadInt32();
            Columns = binReader.ReadInt32();
            MeshSize = binReader.ReadSingle();

            m_hightFields.Clear();

            Navigation.Grid grid = new Navigation.Grid((int)(Rows/MeshSize), (int)(Columns/MeshSize));

            for (int i = 0; i < Rows / MeshSize; ++i)
            {
                m_hightFields.Add(new List<float>());
                for (int j = 0; j < Columns / MeshSize; ++j)
                {
                    int nWalkable = binReader.ReadInt32();
                    float hight = binReader.ReadSingle();
                    m_hightFields[i].Add(hight);
                    grid[new Navigation.Grid.Position(i, j)] = (nWalkable == 1 ? true : false);
                }
            }

            m_ActiveGrid = grid;

            binReader.Close();
            fs.Close();
        }

        private string GetFilePath()
        {
            return Application.dataPath + "//Resources//NavGrid//";
        }

        public void Close()
        {
            m_ActiveGrid = null;
        }

        public Vector3[] FindPath(Vector3 startPt, Vector3 endPt)
        {
            if (m_ActiveGrid != null)
            {
                m_path.Clear();
                Navigation.Grid.Position st = new Navigation.Grid.Position((int)startPt.x, (int)startPt.z);
                Navigation.Grid.Position et = new Navigation.Grid.Position((int)endPt.x, (int)endPt.z);
                m_ActiveGrid.FindPath(st, et, ref m_path);
            }

            Vector3[] path = new Vector3[m_path.Count];
            int index = 0;
            IEnumerator iter = m_path.GetEnumerator();
            while (iter.MoveNext())
            {
                Navigation.Grid.Position pos = (Navigation.Grid.Position)iter.Current;
                path[index++] = new Vector3(pos.X, m_hightFields[pos.X][pos.Y], pos.Y);
            }
            return path;
        }

        public bool IsWalkable(Vector3 pos)
        {
            if (m_ActiveGrid != null)
            {
                Navigation.Grid.Position st = new Navigation.Grid.Position((int)pos.x, (int)pos.z);
                return m_ActiveGrid[st];
            }
            else
                return false;
        }

        private int GetSmoothPath(ref Vector3[] path)
        {
            int index = 0;
            IEnumerator iter = m_path.GetEnumerator();
            while (iter.MoveNext())
            {
                Navigation.Grid.Position pos = (Navigation.Grid.Position)iter.Current;                
                path[index++] = new Vector3(pos.X, m_hightFields[pos.X][pos.Y], pos.Y);
            }
            return m_path.Count;
        }

        public Vector3 GetNextPoint(Vector3 current)
        {
            if (m_path.Count > 0)
            {
                IEnumerator iter = m_path.GetEnumerator();                
                while (iter.MoveNext())
                {
                    Navigation.Grid.Position pos = (Navigation.Grid.Position)iter.Current;
                    Vector3 currentPos = new Vector3(pos.X, m_hightFields[pos.X][pos.Y], pos.Y);
                    if(Vector3.Distance(current, currentPos) < Threshold)
                    {
                        iter.MoveNext();
                        return currentPos;
                    }
                }
                return current;
            }
            return Vector3.zero;
        }

    }
}
