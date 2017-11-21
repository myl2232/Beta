using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public struct RecastTriangle
{
    public Vector3 m_A; // vertexA
    public Vector3 m_B; // vertexB
    public Vector3 m_C; // vertexC

    public void Offset(float offset)
    {
        m_A.y += offset;
        m_B.y += offset;
        m_C.y += offset;
    }

    public Vector3 GetNormal()
    {
        return Vector3.Cross((m_B - m_A), (m_C - m_A)).normalized;
    }
}

public struct RecastVisualTriangle
{
    public RecastTriangle m_triangle;
    public Color32 m_color;
};


public class RecastNavigationNavMeshRepresentation
{
	public List<Mesh> m_navDataVisualDebugMeshes = new List<Mesh>();
	
	private Vector3[] vertices;
	private int[] triangles;
	private Color32[] colors;
	
	private RecastVisualTriangle triangle;
	private uint currentVertexIdx = 0;
	private uint remainingVertexCount = 0;
	
	private Shader m_navMeshSolidShader;
	private Material m_navMeshSolidMaterial;
	private Shader m_navMeshWireframeShader;
	private Material m_navMeshWireframeMaterial;

    private Vector3[] m_polyVertices;
    private Color32[] m_polyColors;

    Mesh m_lineMesh1;
    Mesh m_lineMesh2;
	
	public void LoadShadersAndMaterials()
	{
 		m_navMeshSolidShader = (Shader)Resources.Load("Navigations/NavShaders/NavMeshSolid");
 		m_navMeshSolidMaterial = new Material(m_navMeshSolidShader);

        //m_navMeshSolidMaterial = new UnityEngine.Material(Shader.Find("Diffuse"));
        //m_navMeshSolidMaterial.hideFlags |= HideFlags.DontSave;
		
		m_navMeshWireframeShader = (Shader)Resources.Load("Navigations/NavShaders/NavMeshWireframe");
		m_navMeshWireframeMaterial = new Material(m_navMeshWireframeShader);
	}
	
	public void Draw()
	{
		if (m_navMeshSolidMaterial == null)
			LoadShadersAndMaterials();
		
		m_navMeshSolidMaterial.SetPass(0);
		foreach(Mesh mesh in m_navDataVisualDebugMeshes)
		{	
			if (mesh != null)
				Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
		}
		
// 		GL.wireframe = true;
// 		m_navMeshWireframeMaterial.SetPass(0);
// 		foreach(Mesh mesh in m_navDataVisualDebugMeshes)
// 		{
// 			if (mesh != null)
// 				Graphics.DrawMeshNow(mesh, Matrix4x4.identity);
// 		}
// 		GL.wireframe = false;

        
	}
	
	public void DoClear()
	{
		if (m_navDataVisualDebugMeshes != null)
		{
			foreach (Mesh mesh in m_navDataVisualDebugMeshes)
			{
				if (mesh != null)
					mesh.Clear();
			}
			m_navDataVisualDebugMeshes.Clear();
		}
	}
	
	public void Begin(uint triangleCount)
	{
		// Unity has a limit of 64k vertices
		remainingVertexCount = triangleCount * 3;
		uint vertexSize = remainingVertexCount;
		if (vertexSize >= 65535)
			vertexSize = 65535;
	
		vertices = new Vector3[vertexSize];
		triangles = new int[vertexSize];
		colors = new Color32[vertexSize];
		
		currentVertexIdx = 0;
		
		DoClear();
	}
	
	public void AddTriangle(RecastVisualTriangle triangle)
	{
		// Unity has a limit of 64k vertices for a single mesh, so if we can not store this triangle we build the mesh
		// with all triangles until now and start a new mesh.
		if (currentVertexIdx > 65532)
		{
			BuildAndAddMesh(ref vertices, ref triangles, ref colors);
			
			remainingVertexCount -= currentVertexIdx;
			if (remainingVertexCount < 65535)
			{
				vertices = new Vector3[remainingVertexCount];
				triangles = new int[remainingVertexCount];
				colors = new Color32[remainingVertexCount];
			}
			
			currentVertexIdx = 0;
		}
		
		triangle.m_triangle.Offset(0.3f);
		
		vertices[currentVertexIdx]   = triangle.m_triangle.m_A;
		vertices[currentVertexIdx+1] = triangle.m_triangle.m_B;
		vertices[currentVertexIdx+2] = triangle.m_triangle.m_C;
		
		triangles[currentVertexIdx]   = (int)currentVertexIdx;
		triangles[currentVertexIdx+1] = (int)currentVertexIdx + 1;
		triangles[currentVertexIdx+2] = (int)currentVertexIdx + 2;
		
		colors[currentVertexIdx] = triangle.m_color;
		colors[currentVertexIdx+1] = triangle.m_color;
		colors[currentVertexIdx+2] = triangle.m_color;
		
		currentVertexIdx += 3;
	}
	
	public void End()
	{
		BuildAndAddMesh(ref vertices, ref triangles, ref colors);
	}
	
	public void BuildAndAddMesh(ref Vector3[] vertices, ref int[] triangles, ref Color32[] colors)
	{
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors32 = colors;
        mesh.RecalculateNormals();
        ;
        m_navDataVisualDebugMeshes.Add(mesh);
	}

    public void BuildLineMesh()
    {
        m_lineMesh1 = new Mesh();
        m_lineMesh1.vertices = m_polyVertices;
        m_lineMesh1.colors32 = new Color32[m_polyVertices.Length];
        int[] indices = new int[m_polyVertices.Length];

        for (int i = 0; i < m_polyVertices.Length; i++)
        {
            indices[i] = i;
            m_lineMesh1.colors32[i] = m_polyColors[0];
        }
        m_lineMesh1.SetIndices(indices, MeshTopology.Lines, 0);

//         m_lineMesh2 = new Mesh();
//         m_lineMesh2.vertices = new Vector3[m_polyVertices2.Count];
//         m_polyVertices2.CopyTo(m_lineMesh2.vertices);
//         m_lineMesh2.colors32 = new Color32[m_polyVertices2.Count];
// 
//         int[] indices2 = new int[m_polyVertices2.Count];
//         for (int i = 0; i < m_polyVertices2.Count; i++)
//         {
//             indices2[i] = i;
//             m_lineMesh2.colors32[i] = m_polyColors[1];
//         }
//         m_lineMesh2.SetIndices(indices2, MeshTopology.Lines, 0);

        //m_lineMesh1.RecalculateNormals();
        m_lineMesh1.RecalculateBounds();
//         m_lineMesh2.RecalculateNormals();
//         m_lineMesh2.RecalculateBounds();
        m_navDataVisualDebugMeshes.Add(m_lineMesh1);
        //m_navDataVisualDebugMeshes.Add(m_lineMesh2);
    }

    public void ReSetLine(int vertexCount)
    {
        m_polyVertices = new Vector3[vertexCount];
        m_polyColors = new Color32[] { new Color32(0, 48, 64, 128), new Color32(0,48,64,220) };
    }
    public void AddLineVertex(int vertexIndex, Vector3 vertex, Color32 color)
    {
        m_polyVertices[vertexIndex] = vertex;
    }
}