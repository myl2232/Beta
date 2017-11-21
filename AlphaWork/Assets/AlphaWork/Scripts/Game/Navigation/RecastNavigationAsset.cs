using UnityEngine;
using System;
using System.Runtime.InteropServices;

	
public class RecastNavigationAsset : UnityEngine.ScriptableObject
{
	[SerializeField]
	public int triangleCount;
	[SerializeField]
	public int sizeInBytes;
	[SerializeField]
	public byte[] navMeshData;

    public RecastNavigationAsset() { }
	
	public void SetDataInfo(int _triangleCount)
	{
		triangleCount = _triangleCount;
	}
	
	public void AssignData(IntPtr ptrToNavMeshBuffer, int bufferSizeInBytes)
	{
		sizeInBytes = bufferSizeInBytes;
		navMeshData = new byte[bufferSizeInBytes];
        Marshal.Copy(ptrToNavMeshBuffer, navMeshData, 0, bufferSizeInBytes);
	}
		
}