using AlphaWork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Navigation
{
    public static class VectorExtension
    {
        public static float Distance2D(this Vector3 pos, Vector3 tarPos)
        {
            pos.y = 0f;
            tarPos.y = 0f;
            return Vector3.Distance(pos, tarPos);
        }

        public static Vector3 Normalize2D(this Vector3 vector)
        {
            vector.y = 0f;
            vector.Normalize();
            return vector;
        }

    }

    class OpenList : IDisposable
    {
        SortedDictionary<int, LinkedList<Node>> m_costToNodes = new SortedDictionary<int, LinkedList<Node>>();
        int m_nodeCount;

        void IDisposable.Dispose()
        {
            foreach (var pair in m_costToNodes)
                foreach (var node in pair.Value)
                    node.ResetOpen();
        }

        void Add(int FCost, Node node)
        {
            LinkedList<Node> nodeList;
            if (!m_costToNodes.TryGetValue(FCost, out nodeList))
            {
                nodeList = new LinkedList<Node>();
                m_costToNodes.Add(FCost, nodeList);
            }

            node.Link(nodeList);
        }

        public void Register(int FCost, Node node)
        {
            node.Open();
            Add(FCost, node);

            ++m_nodeCount;
        }

        public void Adjust(int FCost, Node node)
        {
            node.Unlink();
            Add(FCost, node);
        }

        public Node RemoveLowestFCostNode()
        {
            for (;;)
            {
                var itr = m_costToNodes.GetEnumerator();
                itr.MoveNext();

                var firstPair = itr.Current;

                var list = firstPair.Value;

                if (list.Count == 0)
                    m_costToNodes.Remove(firstPair.Key);
                else
                {
                    var node = list.Last.Value;
                    list.RemoveLast();

                    --m_nodeCount;

                    return node;
                }
            }
        }

        public bool IsEmpty
        {
            get
            {
                return m_nodeCount == 0;
            }
        }
    }

    class CloseList : IDisposable
    {
        LinkedList<Node> m_nodes = new LinkedList<Node>();

        void IDisposable.Dispose()
        {
            foreach (var node in m_nodes)
                node.ResetClose();
        }

        public void Register(Node node)
        {
            node.Close();
            m_nodes.AddLast(node);
        }
    }

    class Node
    {
        Grid.Position m_pos;
        Node m_parent;
        LinkedListNode<Node> m_openListLink;
        int m_GCost;
        float m_height = 0;
        float m_Dynamicheight = 0;
        bool m_walkable = true;
        bool m_open;
        bool m_closed;

        public Node(Grid.Position pos)
        {
            m_pos = pos;
        }

        public void Link(LinkedList<Node> nodeList)
        {
            m_openListLink = nodeList.AddLast(this);
        }

        public void Unlink()
        {
            m_openListLink.List.Remove(m_openListLink);
        }

        public bool Walkable
        {
            set
            {
                m_walkable = value;
            }

            get
            {
                return m_walkable;
            }
        }

        public float Heigh
        {
            set
            {
                m_height = value;
                m_pos.Height = value;
            }

            get
            {
                return m_height+m_Dynamicheight;
            }
        }

        public float DynamicHeigh
        {
            set
            {
                m_Dynamicheight = value;
            }

            get
            {
                return m_Dynamicheight;
            }
        }

        public Grid.Position Index
        {
            set
            {
                m_pos = value;
            }

            get
            {
                return m_pos;
            }
        }

        public Grid.Position Position
        {
            get
            {
                return m_pos;
            }
        }

        public void Open()
        {
            Debug.Assert(!m_open);
            Debug.Assert(!m_closed);

            m_open = true;
        }

        public void ResetOpen()
        {
            Debug.Assert(m_open);
            Debug.Assert(!m_closed);

            m_open = false;

            m_GCost = 0;
        }

        public void Close()
        {
            Debug.Assert(m_open);
            Debug.Assert(!m_closed);

            m_open = false;
            m_closed = true;
        }

        public void ResetClose()
        {
            Debug.Assert(!m_open);
            Debug.Assert(m_closed);

            m_closed = false;

            m_GCost = 0;
        }

        public void SetParent(Node parent, int stepG)
        {
            m_parent = parent;
            m_GCost = parent.m_GCost + stepG;
        }

        public Node Parent
        {
            get
            {
                return m_parent;
            }
        }

        public int GCost
        {
            get
            {
                return m_GCost;
            }
        }

        public bool IsClosed
        {
            get
            {
                return m_closed;
            }
        }

        public bool IsOpen
        {
            get
            {
                return m_open;
            }
        }
    }

    public class Grid
    {
        Node[,] m_grid;

        public struct Position
        {
			public Position(int x, int y):this()
            {
                X = x;
                Y = y;
                Height = 0.1f;
            }

            public int X
            {
                set;
                get;
            }

            public int Y
            {
                set;
                get;
            }

            public float Height
            {
                set;
                get;
            }

            public static Position operator +(Position lhs, Position rhs)
            {
                return new Position(lhs.X + rhs.X, lhs.Y + rhs.Y);
            }

            public static int ManhattanDistance(Position a, Position b)
            {
                return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
            }
        }

        public Grid()
        {
        }

        public Grid(int numRow, int numColumn)
        {
            Init(numRow, numColumn);
        }

        Vector3 GetGridPosition(int x, int y)
		{
			return new Vector3(x* GridSize, y* GridSize, 0);
        }
        Vector3 GetGridCenter(int x, int y)
        {
            Vector3 c = GetGridPosition(x, y);
            c.x += GridSize * 0.5f;
            c.z += GridSize * 0.5f;
            return c;
        }
        Vector3 GetGridCenter(Vector3 pos)
        {
            return GetGridCenter((int)(pos.x / GridSize), (int)(pos.z / GridSize));
        }
        public void Init(int x, int y)
        {
            m_grid = null;
            m_grid = new Node[x, y];

            for (var i = 0; i != x; ++i)
                for (var j = 0; j != y; ++j)
                {
                    m_grid[i, j] = new Node(new Position(i, j));
                    this[new Position(i, j)] = true;//add by myl
                }
        }
        public void DrawGizmos(Vector3 center, float di)
        {
            Vector3 size = new Vector3(GridSize, 0.01f, GridSize);
            for (var i = 0; i != m_grid.GetLength(0); ++i)
                for (var j = 0; j != m_grid.GetLength(1); ++j)
                {
                    Node node = m_grid[i, j];
                    Vector3 pos = new Vector3(node.Position.X * GridSize, node.Heigh, node.Position.Y * GridSize);
                    if(VectorExtension.Distance2D(center,pos) < di)
                    {
                        Vector3 p1 = pos;
                        p1.x += GridSize;
                        Vector3 p2 = pos;
                        p2.z += GridSize;
                        Vector3 p3 = pos;
                        p3.x += GridSize;
                        p3.z += GridSize;
                        Gizmos.DrawLine(pos, p1);
                        Gizmos.DrawLine(pos, p2);
                        Gizmos.DrawLine(p3, p1);
                        Gizmos.DrawLine(p3, p2);
                    }
                }
        }

        public void DrawGizmos(ref Vector3 pos)
        {
            Vector3 size = new Vector3(GridSize, 0.01f, GridSize);
            Node node = GetNode(pos);
            if (node == null)
                return;
            Vector3 tpos = GetGridCenter(node.Position.X, node.Position.Y);// new Vector3(node.Position.X * GridSize - GridSize/2, node.Heigh, node.Position.Y * GridSize - GridSize / 2);
            Gizmos.DrawWireCube(tpos, size);
        }

        public float GridSize
        {
            set;
            get;
        }
        public int NumX
        {
            get
            {
                return m_grid.GetLength(0);
            }
        }

        public int NumY
        {
            get
            {
                return m_grid.GetLength(1);
            }
        }

        public bool this[Position pos]
        {
            set
            {
                if (m_grid.GetLength(0) > pos.X && m_grid.GetLength(1) > pos.Y)
                {
                    m_grid[pos.X, pos.Y].Walkable = value;
                }
            }


            get
            {
                if (m_grid.GetLength(0) > pos.X && m_grid.GetLength(1) > pos.Y)
                {
                    return m_grid[pos.X, pos.Y].Walkable;
                }
                return false;
            }
        }

        bool IsOutOfBound(Position pos)
        {
            return pos.X < 0 || pos.X >= NumX || pos.Y < 0 || pos.Y >= NumY;
        }

        int CalcFCost(Node curr, Position currPos, Position destPos)
        {
            return curr.GCost + CalcHCost(currPos, destPos);
        }

        int CalcHCost(Position currPos, Position destPos)
        {
            return Position.ManhattanDistance(currPos, destPos) * 10;
        }

        Node GetNode(Position startPos)
        {
            if (m_grid.GetLength(0) <= startPos.X || m_grid.GetLength(1) <= startPos.Y)
            {
                return null;
            }
            return m_grid[startPos.X, startPos.Y];
        }

        Node GetNode(Vector3 startPos)
        {
            int r = (int)(startPos.x / GridSize);
            int c = (int)(startPos.z / GridSize);
            if (r < 0 || c < 0 || m_grid.GetLength(0) <= r || m_grid.GetLength(1) <= c)
            {
                return null;
            }
            return m_grid[r, c];
        }

        public float GetHeight(Vector3 startPos)
        {
            int r = (int)(startPos.x / GridSize);
            int c = (int)(startPos.z / GridSize);
            if (r < 0 || c < 0 || m_grid.GetLength(0) <= r || m_grid.GetLength(1) <= c)
            {
                return 0;
            }
            return m_grid[r, c].Heigh;
        }

        float GetHeight(int x, int y)
        {
            if (x < 0 || y < 0 || m_grid.GetLength(0) <= x || m_grid.GetLength(1) <= y)
            {
                return 0;
            }
            return m_grid[x, y].Heigh;
        }

        public bool IsWalkable(Vector3 startPos)
        {
            int r = (int)(startPos.x / GridSize);
            int c = (int)(startPos.z / GridSize);
            if (r<0 || c<0 || m_grid.GetLength(0) <= r || m_grid.GetLength(1) <= c)
            {
                return false;
            }

            return m_grid[r, c].Walkable;
        }

        public bool CanGoto(Vector3 startPos, Vector3 endPos, float checkHeight)
        {
            int r = (int)(endPos.x / GridSize);
            int c = (int)(endPos.z / GridSize);
            if (r < 0 || c < 0 || m_grid.GetLength(0) <= r || m_grid.GetLength(1) <= c)
            {
                return false;
            }

            return m_grid[r, c].Walkable && (Math.Abs(m_grid[r, c].Heigh - startPos.y) <= checkHeight);
        }

        bool IsBetterParent(Node node, Node potentialParent, int stepG)
        {
            return potentialParent.GCost + stepG < node.GCost;
        }

        public bool FindPath(Vector3 startPos, Vector3 endPos, ref LinkedList<Vector3> wayPoints)
        {
            Position sPos = new Position((int)(startPos.x / GridSize), (int)(startPos.z / GridSize));
            Position ePos = new Position((int)(endPos.x / GridSize), (int)(endPos.z / GridSize));
            LinkedList<Position> tempPoints = new LinkedList<Position>();
            if(!FindPath(sPos, ePos, ref tempPoints))
                return false;

            foreach (Position point in tempPoints)
            {
                Vector3 pos = new Vector3(point.X * GridSize, point.Height, point.Y * GridSize);
                wayPoints.AddLast(pos);
            }
            return true;
        }

        public bool FindPath(Vector3 startPos, Vector3 endPos, ref LinkedList<Position> wayPoints)
        {
            Position sPos = new Position((int)(startPos.x / GridSize), (int)(startPos.z / GridSize));
            Position ePos = new Position((int)(endPos.x / GridSize), (int)(endPos.z / GridSize));
            return FindPath(sPos, ePos, ref wayPoints);
        }

        public bool FindPath(Position startPos, Position endPos, ref LinkedList<Position> wayPoints)
        {
            Debug.Assert(wayPoints.Count == 0);

            if (IsOutOfBound(startPos) || IsOutOfBound(endPos))
                return false;

            var startNode = GetNode(startPos);
            if (!startNode.Walkable)
                return false;

            var endNode = GetNode(endPos);
            if (!endNode.Walkable)
                return false;

            if (object.ReferenceEquals(startNode, endNode))
                return true;

            var adjOffset = new[]
              {
                new Position(-1, -1),
                new Position(-1, 0),
                new Position(-1, 1),
                new Position(0, 1),
                new Position(1, 1),
                new Position(1, 0),
                new Position(1, -1),
                new Position(0, -1)
            };

            var stepG = new[] { 14, 10 };

            using (var openList = new OpenList())
            {
                using (var closeList = new CloseList())
                {
                    openList.Register(CalcFCost(startNode, startPos, endPos), startNode);

                    while (!openList.IsEmpty)
                    {
                        var currNode = openList.RemoveLowestFCostNode();
                        closeList.Register(currNode);

                        if (object.ReferenceEquals(currNode, endNode))
                        {
                            for (;;)
                            {
                                currNode = currNode.Parent;
                                if (object.ReferenceEquals(currNode, startNode))
                                    return true;

                                wayPoints.AddFirst(currNode.Position);
                            }
                        }

                        var currPos = currNode.Position;
                        float lastHeight = currNode.Position.Height;
                        for (var i = 0; i != 8; ++i)
                        {
                            var adjPos = currPos + adjOffset[i];

                            if (IsOutOfBound(adjPos))
                                continue;

                            var adjNode = GetNode(adjPos);

                            if (adjNode.IsClosed || !adjNode.Walkable || (Math.Abs(lastHeight - adjNode.Heigh) > 0.5f))
                                continue;

                            var currStepG = stepG[i & 1];

                            Func<int> linkup = () =>
                            {
                                adjNode.SetParent(currNode, currStepG);
                                return CalcFCost(adjNode, adjPos, endPos);
                            };

                            if (adjNode.IsOpen)
                            {
                                if (IsBetterParent(adjNode, currNode, currStepG))
                                    openList.Adjust(linkup(), adjNode);
                            }
                            else
                                openList.Register(linkup(), adjNode);
                        }
                    }
                }
            }

            return false;
        }

        //         public bool RayCastBak(Vector3 sPos, ref Vector3 ePos)
        //         {
        //             float dis = Vector3.Distance(sPos, ePos);
        //             Vector3 res = sPos;
        //             Vector3 dir = ePos - sPos;
        //             dir = dir.normalized * GridSize;
        //             int cnt = (int)(dis / GridSize) + 1;
        //             for (int i = 0; i < cnt; ++i)
        //             {
        //                 Vector3 checkPos = res + dir;
        //                 if (!IsWalkable(checkPos))
        //                 {
        //                     ePos = res;
        //                     return true;
        //                 }
        //                 float curH = GetHeight(checkPos);
        //                 if (curH > checkPos.y)
        //                 {
        //                     ePos = res;
        //                     return true;
        //                 }
        //                 res = checkPos;
        //             }
        //             return false;
        //         }


        Dictionary<int, OBB> ShieldList = new Dictionary<int, OBB>();
        public void AddShield(int id, OBB sh)
        {
            ShieldList.Add(id, sh);
        }
        public void RemoveShield(int id)
        {
            ShieldList.Remove(id);
        }
        public bool RayCastShield(Vector3 sPos, ref Vector3 ePos)
        {
            Ray ray = new Ray(sPos, ePos-sPos);
            foreach (KeyValuePair<int, OBB> pair in ShieldList)
            {
                if (pair.Value.Intersect(ray, out ePos))
                    return true;
            }
            return false;
        }
        public bool RayCast(Vector3 sPos, ref Vector3 ePos)
        {
            float dis = Vector3.Distance(sPos, ePos);
            Vector3 res = sPos;
            Vector3 dir = ePos - sPos;
            dir = dir.normalized * GridSize;
            int cnt = (int)(dis / GridSize) + 1;
            for (int i = 0; i < cnt; ++i)
            {
                Vector3 checkPos = res + dir;
                Vector3 checkPos1 = res;
                Vector3 checkPos2 = checkPos;
                checkPos1.x = checkPos.x;
                checkPos1.z = res.z;
                checkPos2.x = res.x;
                checkPos2.z = checkPos.z;

                if (!IsWalkable(checkPos) || 
                    !IsWalkable(checkPos1) ||
                    !IsWalkable(checkPos2))
                {
                    //ePos = res;
                    ePos = GetGridCenter(res);
                    ePos.y = res.y;
                    return true;
                }
                float curH = GetHeight(checkPos);
                float curH1 = GetHeight(checkPos1);
                float curH2 = GetHeight(checkPos2);
                if (curH > checkPos.y ||
                    curH1 > checkPos.y ||
                    curH2 > checkPos.y)
                {
                    //ePos = res;
                    ePos = GetGridCenter(res);
                    ePos.y = res.y;
                    return true;
                }
                res = checkPos;
            }
            return false;
        }

        public bool RayCast2D(Vector3 sPos, ref Vector3 ePos, float fHeight)
        {
            if(!IsWalkable(sPos) || Math.Abs(GetHeight(sPos) - sPos.y) > 2.0f)
            {
                ePos = sPos;
                return true;
            }
            bool bCollition = false;
            Vector3 tmp = ePos;
            float dis = Vector3.Distance(sPos, tmp);
            Vector3 res = sPos;
            float lastHeight = GetHeight(sPos);
            Vector3 dir = ePos - sPos;
            dir = dir.normalized * GridSize;
            int cnt = (int)(dis / GridSize) + 1;
            for (int i = 0; i < cnt; ++i)
            {
                Vector3 checkPos = res + dir;
                Vector3 checkPos1 = res;
                Vector3 checkPos2 = checkPos;
                checkPos1.x = checkPos.x;
                checkPos1.z = res.z;
                checkPos2.x = res.x;
                checkPos2.z = checkPos.z;

                if (!IsWalkable(checkPos) ||
                    !IsWalkable(checkPos1) ||
                    !IsWalkable(checkPos2))
                {
                    //ePos = res;
                    ePos = GetGridCenter(res);
                    ePos.y = GetHeight(ePos);
                    bCollition = true;
                    break;
                }

                float curH = GetHeight(checkPos);
                float curH1 = GetHeight(checkPos1);
                float curH2 = GetHeight(checkPos2);

                if (Math.Abs(curH - lastHeight) > fHeight ||
                    Math.Abs(curH1 - lastHeight) > fHeight ||
                    Math.Abs(curH2 - lastHeight) > fHeight)
                {
                    //ePos = res;
                    ePos = GetGridCenter(res);
                    ePos.y = GetHeight(ePos);
                    bCollition = true;
                    break;
                }
                lastHeight = curH;
                res = checkPos;
            }
            ePos.y = GetHeight(ePos);
            return bCollition;
        }

        public bool RayCastAvatar(Vector3 tPos, Vector3 sPos, Vector3 ePos, float r)
        {
            float dis = Point2SegmentDis.GetDis(tPos, sPos, ePos);
            if (dis <= r)
            {
                return true;
            }
            return false;
        }

        private Action m_OnReadDataComplete = null;

        public void ReadData(byte[] content, Action onReadComplete)
        {
            //m_OnReadDataComplete = onReadComplete;
            Scene scene = SceneManager.GetActiveScene();
            if (scene == null)
            {
                return;
            }

            string gStrHeader = "NavigationGrid Header:";
            int gVersion = 1;

            using (MemoryStream stream = new MemoryStream(content))
            using (BinaryReader binReader = new BinaryReader(stream))
            {
                byte[] bBuffer = new byte[100];
                bBuffer = binReader.ReadBytes(gStrHeader.Length);
                gStrHeader = System.Text.Encoding.Default.GetString(bBuffer);
                gVersion = binReader.ReadInt32();
                int Rows = binReader.ReadInt32();
                int Columns = binReader.ReadInt32();
                GridSize = binReader.ReadSingle();
                Init(Rows, Columns);
                for (int i = 0; i < Rows; ++i)
                {
                    for (int j = 0; j < Columns; ++j)
                    {
                        int nWalkable = binReader.ReadInt32();
                        float hight = binReader.ReadSingle();
                        m_grid[i, j].Heigh = hight;
                        m_grid[i, j].Walkable = (nWalkable == 1 ? true : false);
                    }
                }
            }
            if (onReadComplete != null)
            {
                onReadComplete();
            }
        }

        IEnumerator GetText(string path)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(path))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    // Show results as text
                    Debug.Log(www.downloadHandler.text);

                    // Or retrieve results as binary data
                    byte[] results = www.downloadHandler.data;

                    using (MemoryStream ms = new MemoryStream(results))
                    {
                        using (BinaryReader binReader = new BinaryReader(ms))
                        {
                            string gStrHeader = "NavigationGrid Header:";
                            int gVersion = 1;

                            byte[] bBuffer = new byte[100];
                            bBuffer = binReader.ReadBytes(gStrHeader.Length);
                            gStrHeader = System.Text.Encoding.Default.GetString(bBuffer);
                            gVersion = binReader.ReadInt32();
                            int Rows = binReader.ReadInt32();
                            int Columns = binReader.ReadInt32();
                            GridSize = binReader.ReadSingle();
                            Init(Rows, Columns);
                            for (int i = 0; i < Rows; ++i)
                            {
                                for (int j = 0; j < Columns; ++j)
                                {
                                    int nWalkable = binReader.ReadInt32();
                                    float hight = binReader.ReadSingle();
                                    m_grid[i, j].Heigh = hight;
                                    m_grid[i, j].Walkable = (nWalkable == 1 ? true : false);
                                }
                            }
                        }
                    }
                }
            }
            if (m_OnReadDataComplete != null)
            {
                m_OnReadDataComplete.Invoke();
            }
        }

        public void CreateDynamicWall(int id, Vector3 sPos, Vector3 ePos, float height, int effectid = 800)
        {
            DestroyDynamicWall(id);
            DynamicWall wall = new DynamicWall();
            wall.m_Index = id;
            WallMap.Add(id, wall);

            Vector3 tmp = ePos;
            float dis = Vector3.Distance(sPos, tmp);
            Vector3 checkPos = sPos;
            float lastHeight = GetHeight(sPos);
            Vector3 dir = ePos - sPos;
            dir = dir.normalized * GridSize;    
            int cnt = (int)(dis / GridSize) + 1;
            for (int i = 0; i < cnt; ++i)
            {
                checkPos = checkPos + dir;
                Node n = GetNode(checkPos);
                if (n == null)
                {
                    continue;
                }
                n.DynamicHeigh = height;
                wall.m_GridList.Add(n.Index);
            }
        }

        public void DestroyDynamicWall(int id)
        {
            //myl.temp remove
            //DynamicWall wall;
            //if (!WallMap.TryGetValue(id, out wall))
            //    return;
            //foreach (Grid.Position pos in wall.m_GridList)
            //{
            //    Node n = GetNode(pos);
            //    if (n != null)
            //    {
            //        n.DynamicHeigh = 0.0f;
            //    }
            //}
            //WallMap.Remove(id);
            //EffectManager effmgr = GalaxyGameModule.GetGameManager<EffectManager>();
            //wall.m_GridList = null;
            //wall = null;
        }

        Dictionary<int, DynamicWall> WallMap = new Dictionary<int, DynamicWall>();
    }

    class DynamicWall
    {
       
	    public DynamicWall()
        {

        }
		public int m_Index = 0;
        private GameObject m_EffectObj;
        public List<Grid.Position> m_GridList = new List<Grid.Position>();
    };
}
