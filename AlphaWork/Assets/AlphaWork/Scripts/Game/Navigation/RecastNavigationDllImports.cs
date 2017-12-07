﻿using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System;

public struct NavMeshBuildSettings
{
    public float m_radius;
    public float m_height;
    public float m_maxSlope;
    public float m_stepHeight;
    public float m_dropHeight;
    public float m_jumpDistance;
    public bool m_heightMesh;
    public float m_minRegionArea;
    public float m_widthInaccuracy;
    public float m_heightInaccuracy;
};


public class RecastNavigationDllImports
{

#if UNITY_IPHONE //|| UNITY_STANDALONE_OSX
    const string Recast_Dll = "__Internal";
#elif UNITY_STANDALONE_OSX
    const string Recast_Dll = "ASimplePlugin";
#else
    const string Recast_Dll = "recast-unity";
#endif
    /******** Log    ************************************************************
    * Call CreateLog before GwNavigation init (before calling GwNavRuntime_Init).
    * Call DestroyLog after GwNavigation deinit (after calling GwNavRuntime_DeInit).
    * If you don't create a log, GwNavigation will log by default to C++ IDE if attached
    * and in a text file on drive.
    * 
    * If you created a log, each time you want to retrieve the GwNavigation log (eg: each frame), call
    * GetGwNavLogMessageCount and for each message, call GetGwNavLog. When done, call FlushGwNavLog.
    */

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void CreateLog(int bufferMaxSize, int maxMessageCount);

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetGwNavLogMessageCount();

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetGwNavLog(int messageIndex, out int messageSize);

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void FlushGwNavLog();

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void DestroyLog();


    /******** Base system *************************************************************/

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool NavGeneration_Init();

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void NavGeneration_DeInit();

    /******** Path find *************************************************************/

    public static bool PathFind(Vector3 start, Vector3 end, 
        ref int pathNum, ref Vector3[] smoothPath)
    {
        pathNum = CalcSmoothPath(start, end);
        if (pathNum == 0)
        {
            return false;
        }

        GetSmoothPath(smoothPath);
        return true;
    }

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CalcSmoothPath(Vector3 start, Vector3 end);

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetSmoothPath(Vector3[] smoothPath);

    /******** Generate ************************************************************
     * 1. InitGenerator.
     * 2. Consume inputs.
     * 3. Generate. 
     * 4. Get generated data
     * 5. Render generated data
     */

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void InitGenerator();

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool Generate(NavMeshBuildSettings config, string generationName);


    /******** Consume inputs *************************************************************/

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void PushTriangle(Vector3 A, Vector3 B, Vector3 C);

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void PushSeedPoint(Vector3 pos);

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void PushTriangleWithNavTag(Vector3 A, Vector3 B, Vector3 C);


    /******** Get generated data *************************************************************/

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetGeneratedDataSizeInBytes();

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool GetGeneratedData(ref IntPtr buffer, ref int bufferSize);

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetNavMeshTriangleCount();


    /******** Render generated data *************************************************************/

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool LoadNavDataImmediate(IntPtr memory);

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool RemoveAllNavData();

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool BuildDatabaseGeometry();

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint GetDatabaseTriangleCount();

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool GetDatabaseTriangle(uint triangleIndex, out RecastVisualTriangle triangle);

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetPolygonVertexCount();

    [DllImport(Recast_Dll, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool GetPolygonVertex(uint vertexIndex, out Vector3 vertex, out Color32 color);
}