//using Galaxy;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Navigation
{
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
      for (; ; )
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
            return m_height;
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
      public Position(int row, int col)
      {
        Row = row;
        Column = col;
        Height = 0.1f;
      }

      public int Row
      {
        set;
        get;
      }

      public int Column
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
        return new Position(lhs.Row + rhs.Row, lhs.Column + rhs.Column);
      }

      public static int ManhattanDistance(Position a, Position b)
      {
        return Math.Abs(a.Row - b.Row) + Math.Abs(a.Column - b.Column);
      }
    }

    public Grid()
    {
    }

    public Grid(int numRow, int numColumn)
    {
        Init(numRow, numColumn);
    }

    public void Init(int numRow, int numColumn)
    {
        m_grid = null;
        m_grid = new Node[numRow, numColumn];

        for (var i = 0; i != numRow; ++i)
        for (var j = 0; j != numColumn; ++j)
        {
            m_grid[i, j] = new Node(new Position(i, j));
            this[new Position(i, j)] = true;//add by myl
        }
    }
    public void DrawGizmos()
    {
        Vector3 size = new Vector3(GridSize, 0.01f, GridSize);
        for (var i = 0; i != m_grid.GetLength(0); ++i)
            for (var j = 0; j != m_grid.GetLength(1); ++j)
            {
                Node node = m_grid[i,j];
                Vector3 pos = new Vector3(node.Position.Row,node.Heigh,node.Position.Column );
                Gizmos.DrawWireCube(pos, size);
            }
    }

    public float GridSize
    {
        set;
        get;
    }
    public int NumRows
    {
      get
      {
        return m_grid.GetLength(0);
      }
    }

    public int NumColumns
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
        m_grid[pos.Row, pos.Column].Walkable = value;
      }

      get
      {
        return m_grid[pos.Row, pos.Column].Walkable;
      }
    }

    bool IsOutOfBound(Position pos)
    {
      return pos.Row < 0 || pos.Row >= NumRows || pos.Column < 0 || pos.Column >= NumColumns;
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
      return m_grid[startPos.Row, startPos.Column];
    }

    Node GetNode(Vector3 startPos)
    {
        int r = (int)(startPos.x / GridSize);
        int c = (int)(startPos.z / GridSize);
        return m_grid[r, c];
    }

    float GetHeight(Vector3 startPos)
    {
        int r = (int)(startPos.x / GridSize);
        int c = (int)(startPos.z / GridSize);
        return m_grid[r, c].Heigh;
    }

    float GetHeight(int r, int c)
    {
        return m_grid[r, c].Heigh;
    }

    bool IsWalkable(Vector3 startPos)
    {
        int r = (int)(startPos.x / GridSize);
        int c = (int)(startPos.z / GridSize);
        return m_grid[r, c].Walkable;
    }

    bool IsBetterParent(Node node, Node potentialParent, int stepG)
    {
      return potentialParent.GCost + stepG < node.GCost;
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
              for (; ; )
              {
                currNode = currNode.Parent;
                if (object.ReferenceEquals(currNode, startNode))
                  return true;

                wayPoints.AddFirst(currNode.Position);
              }
            }

            var currPos = currNode.Position;

            for (var i = 0; i != 8; ++i)
            {
              var adjPos = currPos + adjOffset[i];

              if (IsOutOfBound(adjPos))
                continue;

              var adjNode = GetNode(adjPos);

              if (adjNode.IsClosed || !adjNode.Walkable)
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

    public bool RayCast(Vector3 sPos, ref Vector3 ePos, float fHeight)
	{
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
            if (!IsWalkable(checkPos))
            {
                ePos = res;
                return true;
            }
            float curH = GetHeight(checkPos);
            if (Math.Abs(curH - lastHeight) > fHeight)
            {
                ePos = res;
                return true;
            }
            lastHeight = curH;
            res = checkPos;
        }
		return false;
	}
    public void ReadData()
    {

        Scene scene = SceneManager.GetActiveScene();
        if (scene == null)
        {
            return;
        }
        string gStrHeader = "NavigationGrid Header:";
        int gVersion = 1;

        string filePath = Application.streamingAssetsPath + "/NavGrid/" + scene.name + "/GalaxyNavFile";
        FileStream fs = new FileStream(filePath, FileMode.Open);
        BinaryReader binReader = new BinaryReader(fs);

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

        binReader.Close();
        fs.Close();

    }
   
  }
}
