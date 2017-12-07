using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System;

[Serializable]
public class RecastNavigationEditorWindowParams
{
    [SerializeField]
    public float radius = 0.5f;
    [SerializeField]
    public float height = 2;
    [SerializeField]
    public float maxSlope = 45;
    [SerializeField]
    public float stepHeight = 0.4f;
    [SerializeField]
    public float minRegionArea = 2;

    [SerializeField]
    public float DropHeight = 0.0f;
    [SerializeField]
    public float JumpDistance = 0.0f;

    [SerializeField]
    public float WidthInaccuracy = 100.0f / 6.0f;
    [SerializeField]
    public float HeightInaccuracy = 10.0f;
    [SerializeField]
    public bool HeightMesh = false;


    [SerializeField]
    public bool showAdvanced = false;

    [SerializeField]
    public bool drawNavMesh = true;
    [SerializeField]
    public bool showInEditorGui = true;

    public void InitFromPrefs()
    {
        radius = EditorPrefs.GetFloat("radius", radius);
        height = EditorPrefs.GetFloat("height", height);
        maxSlope = EditorPrefs.GetFloat("maxSlope", maxSlope);
        stepHeight = EditorPrefs.GetFloat("stepHeight", stepHeight);
        minRegionArea = EditorPrefs.GetFloat("minRegionArea", minRegionArea);
        DropHeight = EditorPrefs.GetFloat("DropHeight", DropHeight);
        JumpDistance = EditorPrefs.GetFloat("JumpDistance", JumpDistance);
        WidthInaccuracy = EditorPrefs.GetFloat("WidthInaccuracy", WidthInaccuracy);
        HeightInaccuracy = EditorPrefs.GetFloat("HeightInaccuracy", HeightInaccuracy);
        HeightMesh = EditorPrefs.GetBool("HeightMesh", HeightMesh);

        showAdvanced = EditorPrefs.GetBool("showAdvanced", showAdvanced);
        drawNavMesh = EditorPrefs.GetBool("drawNavMesh", drawNavMesh);
        showInEditorGui = EditorPrefs.GetBool("showInEditorGui", showInEditorGui);
    }

    public void SaveToPrefs()
    {
        EditorPrefs.SetFloat("radius", radius);
        EditorPrefs.SetFloat("height", height);
        EditorPrefs.SetFloat("maxSlope", maxSlope);
        EditorPrefs.SetFloat("stepHeight", stepHeight);
        EditorPrefs.SetFloat("minRegionArea", minRegionArea);
        EditorPrefs.SetFloat("DropHeight", DropHeight);
        EditorPrefs.SetFloat("JumpDistance", JumpDistance);
        EditorPrefs.SetFloat("WidthInaccuracy", WidthInaccuracy);
        EditorPrefs.SetFloat("HeightInaccuracy", HeightInaccuracy);
        EditorPrefs.SetBool("HeightMesh", HeightMesh);

        EditorPrefs.SetBool("showAdvanced", showAdvanced);
        EditorPrefs.SetBool("drawNavMesh", drawNavMesh);
        EditorPrefs.SetBool("showInEditorGui", showInEditorGui);
    }

    public void OnGUI()
    {
        GUILayout.Label("General", EditorStyles.boldLabel);
        radius = EditorGUILayout.FloatField("Radius", radius);
        height = EditorGUILayout.FloatField("Height", height);
        maxSlope = EditorGUILayout.Slider("Max Slope", maxSlope, 0.0f, 90.0f);
        stepHeight = EditorGUILayout.FloatField("Step Height", stepHeight);

        EditorGUILayout.Space();
        GUILayout.Label("Generated Off Mesh Links", EditorStyles.boldLabel);
        DropHeight = EditorGUILayout.FloatField("Drop Height", DropHeight);
        JumpDistance = EditorGUILayout.FloatField("Jump Distance", JumpDistance);

        EditorGUILayout.Space();
        showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced");
        if (showAdvanced)
        {
            minRegionArea = EditorGUILayout.FloatField("Min region area", minRegionArea);
            WidthInaccuracy = EditorGUILayout.Slider("Width Inaccuracy %", WidthInaccuracy, 1.0f, 100.0f);
            HeightInaccuracy = EditorGUILayout.Slider("Height Inaccuracy %", HeightInaccuracy, 1.0f, 100.0f);
            HeightMesh = EditorGUILayout.Toggle("Height Mesh", HeightMesh);
        }

        //         if (RecastNavigationDllImports.NavGeneration_Init() == false)
        //         {
        //             Debug.Log("Error initializing Recast Navigation plugin!");
        //         }
        //         else
        //         {
        //             Debug.Log("Successfully initialized Recast Navigation plugin.");
        //         }
    }

    public void Reset()
    {
        radius = 0.5f;
        height = 2;
        maxSlope = 45;
        stepHeight = 0.4f;
        minRegionArea = 2;
        DropHeight = 0.0f;
        JumpDistance = 0.0f;
        WidthInaccuracy = 100.0f / 6.0f;
        HeightInaccuracy = 10.0f;
        HeightMesh = false;
    }
}

public class RecastNavigationEditorWindow : EditorWindow
{
    NavMeshBuildSettings m_generationConfig;
    private RecastNavigationEditorWindowParams m_params;
    //public bool hasNavMeshVisualRep = false;

    int totalTriangleCount = 0;
    RecastNavigationNavMeshRepresentation m_navMeshRepresentation;

    string levelName;
    string sceneName;
    string resourceFolder;
    string projectName;

    //private GwNavigationWorld m_world;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/RecastNavigation")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(RecastNavigationEditorWindow));
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
    }

    void Awake()
    {
        if (m_params == null)
            m_params = new RecastNavigationEditorWindowParams();
        m_params.InitFromPrefs();
        Init();
    }

    void OnDestroy()
    {
        m_params.SaveToPrefs();
        DeInit();
    }

    void OnGUI()
    {
        title = "RecastNavigation";

        EditorGUILayout.Space();
        m_params.OnGUI();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset"))
        {
            OnResetButtonPressed();
        }
#if !UNITY_STANDALONE_OSX
        if (GUILayout.Button("Clear"))
        {
            if (m_navMeshRepresentation != null)
                m_navMeshRepresentation.DoClear();
            AssetDatabase.DeleteAsset("Assets/Resources/" + resourceFolder + "RecastNavmesh.asset");

        }
        if (GUILayout.Button("Bake"))
        {
            Bake();
        }
#endif
        EditorGUILayout.EndHorizontal();

        m_params.showInEditorGui = GUILayout.Toggle(m_params.showInEditorGui, "Editor navMesh view config");
    }

    void OnFocus()
    {
    }

    void Update()
    {
    }

    void OnInspectorUpdate()
    {
    }

    void OnLostFocus()
    {
    }

    public void OnSceneGUI(SceneView sceneView)
    {
#if !UNITY_STANDALONE_OSX
        if (m_params.showInEditorGui)
#endif
        {
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(Screen.width - 140, Screen.height - 180, 120, 80));
            GUILayout.Label("NavMesh display");

            bool newDrawNavMesh = GUILayout.Toggle(m_params.drawNavMesh, "Show NavMesh");
            if (newDrawNavMesh != m_params.drawNavMesh)
            {
                m_params.drawNavMesh = newDrawNavMesh;
                if (m_params.drawNavMesh == false)
                {
                    //m_navMeshRepresentation.DoClear();
                }
                else
                {
                    //if (hasNavMeshVisualRep)
                    //  BuildDatabaseMesh();
                }
            }
            GUILayout.Toggle(true, "Show NavTags");
            GUILayout.EndArea();
            Handles.EndGUI();
        }
#if !UNITY_STANDALONE_OSX
        if(m_params.drawNavMesh)
#endif
        {
            if (m_navMeshRepresentation.m_navDataVisualDebugMeshes != null)
            {
                m_navMeshRepresentation.Draw();

                sceneView.Repaint();
            }
        }

    }

    void OnResetButtonPressed()
    {
        m_params.Reset();
    }

    //  GwNavTag BuildNavTag(GameObject gameObject)
    //  {
    //      GwNavigationGenerationInfo genInfo = gameObject.GetComponent<GwNavigationGenerationInfo>();
    //      //int unityNavMeshLayerIdx = GameObjectUtility.GetNavMeshLayer(gameObject);
    //      int navMeshLayerIdx = 0;
    //      if (genInfo != null)
    //          navMeshLayerIdx = genInfo.navMeshLayer;
    //      
    //      GwNavTag navTag = new GwNavTag();
    //      
    //      navTag.m_navTagType           = (uint)GwNavTagType.Layer;
    //      navTag.m_layer                = (uint)navMeshLayerIdx;
    //      navTag.m_staticCostMultiplier = (uint)m_world.GetLayerCost(navMeshLayerIdx);
    //      navTag.m_smartObjectID        = uint.MaxValue;
    //      navTag.m_isExclusive          = navMeshLayerIdx == 1 ? true : false; // navMeshLayerIdx 1 = "not walkable"
    //      Color32 color = new Color32();
    //      color = m_world.GetLayerColor(navMeshLayerIdx);
    //      navTag.m_color.Set(color);
    //      return navTag;
    //  }

    bool CompareApproximately(float x, float y, float epsilon = 0.000001F)
    {
        float dist = x - y;
        dist = Math.Abs(dist);
        return dist < epsilon;
    }

    bool HasNegativeScaleTransform(Transform t)
    {
        if (CompareApproximately(t.localScale.x, t.localScale.y, 0.0001F) && CompareApproximately(t.localScale.y, t.localScale.z, 0.0001F))
        {
            if (CompareApproximately(t.localScale.x, 1.0F, 0.0001F))
                return false;
            else
            {
                if (t.localScale.x > 0.0F)
                    return false;
                else
                    return true;
            }
        }
        else
        {
            if ((t.localScale.x <= 0.0F)
                || (t.localScale.y <= 0.0F)
                || (t.localScale.z <= 0.0F))
                return true;
        }
        return false;
    }

    void ConsumeMeshFilter(MeshFilter meshFilter, GameObject gameObject)
    {
        if (meshFilter.sharedMesh)
        {
            Vector3[] vertices = meshFilter.sharedMesh.vertices;
            int[] triangleVertexIndices = meshFilter.sharedMesh.triangles;
            Transform t = gameObject.transform;
            //GwNavTag navTag = BuildNavTag(gameObject);

            for (int triangleFirstVertexIdx = 0; triangleFirstVertexIdx < triangleVertexIndices.Length; triangleFirstVertexIdx += 3)
            {
                Vector3 posALocal = vertices[triangleVertexIndices[triangleFirstVertexIdx]];
                Vector3 posBLocal = vertices[triangleVertexIndices[triangleFirstVertexIdx + 1]];
                Vector3 posCLocal = vertices[triangleVertexIndices[triangleFirstVertexIdx + 2]];
                Vector3 posA = t.TransformPoint(posALocal);
                Vector3 posB = t.TransformPoint(posBLocal);
                Vector3 posC = t.TransformPoint(posCLocal);

                if (HasNegativeScaleTransform(t))
                    RecastNavigationDllImports.PushTriangleWithNavTag(posC, posB, posA);
                else
                    RecastNavigationDllImports.PushTriangleWithNavTag(posA, posB, posC);

                totalTriangleCount++;
            }
        }
    }

    Vector3 GetTerrainVertex(TerrainData terrain, int x, int y, float sampleWidthInMeter, float sampleHeightInMeter)
    {
        Vector3 v = new Vector3(x * sampleWidthInMeter, terrain.GetHeight(x, y), y * sampleHeightInMeter);
        return v;
    }

    void ConsumeTerrainTriangles(TerrainData terrain, GameObject gameObject)
    {
        Vector3 terrainPos = gameObject.transform.position;
        //GwNavTag navTag = BuildNavTag(gameObject);

        int vertexCount_x = terrain.heightmapWidth;
        int vertexCount_z = terrain.heightmapHeight;

        float sampleWidthInMeter = terrain.size.x / (terrain.heightmapWidth - 1);
        float sampleHeightInMeter = terrain.size.z / (terrain.heightmapHeight - 1);

        for (int z = 0; z < vertexCount_z - 1; ++z)
        {
            for (int x = 0; x < vertexCount_x - 1; ++x)
            {
                Vector3 A = GetTerrainVertex(terrain, x, z, sampleWidthInMeter, sampleHeightInMeter) + terrainPos;
                Vector3 B = GetTerrainVertex(terrain, x + 1, z, sampleWidthInMeter, sampleHeightInMeter) + terrainPos;
                Vector3 C = GetTerrainVertex(terrain, x + 1, z + 1, sampleWidthInMeter, sampleHeightInMeter) + terrainPos;
                Vector3 D = GetTerrainVertex(terrain, x, z + 1, sampleWidthInMeter, sampleHeightInMeter) + terrainPos;

                //                 RecastNavigationDllImports.PushTriangleWithNavTag(A, B, C);
                //                 RecastNavigationDllImports.PushTriangleWithNavTag(A, C, D);
                RecastNavigationDllImports.PushTriangleWithNavTag(C, B, A);
                RecastNavigationDllImports.PushTriangleWithNavTag(D, C, A);
                totalTriangleCount += 2;
            }
        }

        float treeRadius = 0.3f;
        float treeHeight = 5.0f;
        //navTag.m_isExclusive = true;
        foreach (TreeInstance tree in terrain.treeInstances)
        {
            Vector3 center = Vector3.Scale(terrain.size, tree.position) + terrainPos;
            Vector3 baseA = new Vector3(center.x - treeRadius, center.y, center.z - treeRadius);
            Vector3 baseB = new Vector3(center.x + treeRadius, center.y, center.z - treeRadius);
            Vector3 baseC = new Vector3(center.x + treeRadius, center.y, center.z + treeRadius);
            Vector3 baseD = new Vector3(center.x - treeRadius, center.y, center.z + treeRadius);

            Vector3 topA = baseA + Vector3.up * treeHeight;
            Vector3 topB = baseB + Vector3.up * treeHeight;
            Vector3 topC = baseC + Vector3.up * treeHeight;
            Vector3 topD = baseD + Vector3.up * treeHeight;

            /*
            // Push base has non walkable
            RecastNavigationDllImports.PushTriangleWithNavTag(baseA, baseB, baseC, navTag);
            RecastNavigationDllImports.PushTriangleWithNavTag(baseA, baseC, baseD, navTag);
            totalTriangleCount +=2;
            */

            // Push trunk as a vertical square based tube
            // Front
            RecastNavigationDllImports.PushTriangleWithNavTag(baseA, baseB, topB);
            RecastNavigationDllImports.PushTriangleWithNavTag(baseA, topB, topA);
            // Right
            RecastNavigationDllImports.PushTriangleWithNavTag(baseB, baseC, topC);
            RecastNavigationDllImports.PushTriangleWithNavTag(baseB, topC, topB);
            // Back
            RecastNavigationDllImports.PushTriangleWithNavTag(baseC, baseD, topD);
            RecastNavigationDllImports.PushTriangleWithNavTag(baseC, topD, topC);
            // Left
            RecastNavigationDllImports.PushTriangleWithNavTag(baseD, baseA, topA);
            RecastNavigationDllImports.PushTriangleWithNavTag(baseD, topA, topD);
            totalTriangleCount += 8;
        }
    }

    void ConsumeComponentTriangles(GameObject gameObject, int layer)
    {
        //MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            ConsumeMeshFilter(meshFilter, gameObject);
        }
        else
        {
            Terrain terrain = gameObject.GetComponent<Terrain>();
            if (terrain != null)
            {
                ConsumeTerrainTriangles(terrain.terrainData, gameObject);
            }
        }
    }

    void ConsumeGameObjectTriangles(GameObject gameObject)
    {
        int layer = gameObject.layer;

        //GwNavMeshSeedPoint seedPoint = gameObject.GetComponent<GwNavMeshSeedPoint>();
        //      if (seedPoint != null)
        //             RecastNavigationDllImports.PushSeedPoint(seedPoint.transform.position);

        if (gameObject.isStatic == false)
            return;

        if (GameObjectUtility.AreStaticEditorFlagsSet(gameObject, StaticEditorFlags.NavigationStatic))
        {
            ConsumeComponentTriangles(gameObject, layer);
        }
    }

    void GenerateAsset()
    {
        IntPtr unmanagedPtr = new IntPtr();
        int size = 0;
        bool success = RecastNavigationDllImports.GetGeneratedData(ref unmanagedPtr, ref size);
        if (success)
        {
            RecastNavigationAsset navMeshAsset = CreateInstance<RecastNavigationAsset>();
            navMeshAsset.AssignData(unmanagedPtr, size);
            //int generatedTriangleCount = RecastNavigationDllImports.GetNavMeshTriangleCount();
            //if (generatedTriangleCount == 0)
            //  Debug.LogError("No triangles generated: verify that you have some geometry tagged as NavigationStatic, that your seedpoint is not outside navigable geometry or that your world scale is correct (1 unity unit = 1 meter)");
            //navMeshAsset.SetDataInfo(generatedTriangleCount);

            System.IO.Directory.CreateDirectory(resourceFolder);

            AssetDatabase.CreateAsset(navMeshAsset, "Assets/Resources/" + resourceFolder + "RecastNavmesh.asset");
            Debug.Log(AssetDatabase.GetAssetPath(navMeshAsset));

            string binaryPath = resourceFolder + sceneName + ".bin";
            System.IO.File.WriteAllBytes(binaryPath, navMeshAsset.navMeshData);
        }
    }

    void ConsumeSceneTriangles()
    {
        totalTriangleCount = 0;
        object[] sceneObjs = GameObject.FindObjectsOfType(typeof(GameObject));
        foreach (object o in sceneObjs)
        {
            GameObject gameObject = (GameObject)o;
            ConsumeGameObjectTriangles(gameObject);
        }
    }

    bool IsEmptyScene()
    {
        BuildProjectNameAndSceneName();
        if (sceneName == null || sceneName.Length == 0)
        {
            if (EditorUtility.DisplayDialog("No scene fund", "Please save scene before generating NavData.", "Save", "Cancel") == true)
            {
                if (EditorApplication.SaveScene() == true)
                {
                    BuildProjectNameAndSceneName();
                    if (sceneName == null || sceneName.Length == 0)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        //      if (m_world == null)
        //      {
        //          m_world = (GwNavigationWorld) FindObjectOfType(typeof(GwNavigationWorld));
        //          if (m_world == null)
        //          {
        //              Debug.LogError("Your scene must have a gameObject with a NavigationWolrd component to bake gwnavigation data!");
        //          }
        //      }

        return false;
    }

    void Bake()
    {
        if (IsEmptyScene())
            return;

        //hasNavMeshVisualRep = false;

        m_generationConfig.m_radius = m_params.radius;
        m_generationConfig.m_height = m_params.height;
        m_generationConfig.m_maxSlope = m_params.maxSlope;
        m_generationConfig.m_stepHeight = m_params.stepHeight;
        m_generationConfig.m_minRegionArea = m_params.minRegionArea;
        m_generationConfig.m_dropHeight = m_params.DropHeight;
        m_generationConfig.m_jumpDistance = m_params.JumpDistance;
        m_generationConfig.m_widthInaccuracy = m_params.WidthInaccuracy;
        m_generationConfig.m_heightInaccuracy = m_params.HeightInaccuracy;
        m_generationConfig.m_heightMesh = m_params.HeightMesh;

        RecastNavigationDllImports.InitGenerator();
        ConsumeSceneTriangles();
        bool generationSucceeded = RecastNavigationDllImports.Generate(m_generationConfig, projectName + "-" + sceneName);

        UpdateLog_AllMessagesInOneUnityLog();

        if (generationSucceeded)
        {
            GenerateAsset();
        }
        else
        {
            UnityEngine.Debug.Log("Generation failed.");
            generationSucceeded = false;
        }

        if (generationSucceeded)
        {
            BuildDatabaseMesh();
        }
    }

    void BuildDatabaseMesh()
    {
        RecastNavigationDllImports.RemoveAllNavData();
        UpdateNavDataInDatabase();
        UpdateMeshFromDatabaseTriangles();
    }

    void UpdateMeshFromDatabaseTriangles()
    {
        RecastNavigationDllImports.BuildDatabaseGeometry();

        // TODO: understand why sometimes m_navMeshRepresentation becomes null
        if (m_navMeshRepresentation == null)
        {
            Debug.LogWarning("NavMesh representation became null!");
            m_navMeshRepresentation = new RecastNavigationNavMeshRepresentation();
            m_navMeshRepresentation.LoadShadersAndMaterials();
        }

        uint triangleCount = RecastNavigationDllImports.GetDatabaseTriangleCount();
        m_navMeshRepresentation.Begin(triangleCount);
        RecastVisualTriangle triangle;
        for (uint i = 0; i < triangleCount; ++i)
        {
            RecastNavigationDllImports.GetDatabaseTriangle(i, out triangle);
            m_navMeshRepresentation.AddTriangle(triangle);
        }
        m_navMeshRepresentation.End();

        int vertexCount = RecastNavigationDllImports.GetPolygonVertexCount();
        m_navMeshRepresentation.ReSetLine(vertexCount);
        for (int i = 0; i < vertexCount; i++)
        {
            Vector3 vertex = new Vector3();
            Color32 color = new Color32();
            RecastNavigationDllImports.GetPolygonVertex((uint)i, out vertex, out color);
            m_navMeshRepresentation.AddLineVertex(i, vertex, color);
        }
        m_navMeshRepresentation.BuildLineMesh();

        //hasNavMeshVisualRep = true;
    }

    bool UpdateNavDataInDatabase()
    {
        UnityEngine.Object asset = Resources.Load(resourceFolder + "RecastNavmesh");
        RecastNavigationAsset navDataAsset = asset as RecastNavigationAsset;
        if (navDataAsset == null)
        {
            if (m_navMeshRepresentation != null)
                m_navMeshRepresentation.DoClear();
            return false;
        }

        GCHandle handle = GCHandle.Alloc(navDataAsset.navMeshData, GCHandleType.Pinned);
        if (RecastNavigationDllImports.LoadNavDataImmediate(handle.AddrOfPinnedObject()) == false)
        {
            if (m_navMeshRepresentation != null)
                m_navMeshRepresentation.DoClear();
            Debug.Log("Could not load data");
            return false;
        }

        return true;
    }

    void BuildProjectNameAndSceneName()
    {
        // Project name
        string[] s = Application.dataPath.Split('/');
        projectName = s[s.Length - 2];

        // Level name and resource folder
        string[] s2 = EditorApplication.currentScene.Split('/');
        string currentSceneFullName = s2[s2.Length - 1];
        string[] s3 = currentSceneFullName.Split('.');

        if (s3.Length >= 2)
        {
            sceneName = s3[0];
            for (int i = 1; i < s3.Length - 1; i++)
            {
                sceneName += ".";
                sceneName += s3[i];
            }
            levelName = sceneName + "/";
        }
        resourceFolder = "Navigations/" + levelName;
    }

    void Init()
    {
        //RecastNavigationDllImports.CreateLog(4096, 500);
        if (RecastNavigationDllImports.NavGeneration_Init() == false)
        {
            Debug.Log("Error initializing Recast Navigation plugin!");
        }
        UpdateLog();

        m_navMeshRepresentation = new RecastNavigationNavMeshRepresentation();
        m_navMeshRepresentation.LoadShadersAndMaterials();

        BuildProjectNameAndSceneName();
        if (sceneName != null && sceneName.Length != 0)
            BuildDatabaseMesh();

        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
    }

    void DeInit()
    {
        // When the window is destroyed, remove the delegate
        // so that it will no longer do any drawing.
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;

        RecastNavigationDllImports.NavGeneration_DeInit();

        // Get last messages and destroy log
        UpdateLog();
        RecastNavigationDllImports.DestroyLog();
    }

    void OnDidOpenScene()
    {
        BuildProjectNameAndSceneName();
        BuildDatabaseMesh();
    }

    void UpdateLog()
    {
        int logMessageCount = RecastNavigationDllImports.GetGwNavLogMessageCount();
        int msgSize = 0;
        string fullLog = "";

        for (int msgIdx = 0; msgIdx < logMessageCount; ++msgIdx)
        {
            IntPtr logMsgPtr = RecastNavigationDllImports.GetGwNavLog(msgIdx, out msgSize);
            if (msgSize != 0)
            {
                string logMsg = Marshal.PtrToStringAnsi(logMsgPtr);

                if (logMsg != null && logMsg.Length != 0)
                {
                    fullLog += logMsg;
                    if (logMsg.EndsWith("\n"))
                    {
                        fullLog.Remove(fullLog.Length - 1);
                        Debug.Log("[RecastNavigation] " + fullLog);
                        fullLog = "";
                    }
                }
            }
        }

        // No need to flush if no message (save a call to the plugin)
        if (logMessageCount > 0)
            RecastNavigationDllImports.FlushGwNavLog();
    }

    void UpdateLog_AllMessagesInOneUnityLog()
    {
        int logMessageCount = RecastNavigationDllImports.GetGwNavLogMessageCount();
        int msgSize = 0;

        if (logMessageCount == 0)
            return;

        string fullLog = "";

        for (int msgIdx = 0; msgIdx < logMessageCount; ++msgIdx)
        {
            IntPtr logMsgPtr = RecastNavigationDllImports.GetGwNavLog(msgIdx, out msgSize);
            if (msgSize != 0)
            {
                string logMsg = Marshal.PtrToStringAnsi(logMsgPtr);

                // Hide the ========
                if (logMsg.StartsWith("="))
                    continue;

                // Dont jump 2 lines, only one
                if (logMsg.StartsWith("\n\n"))
                {
                    fullLog += '\n';
                    continue;
                }

                if (logMsg != null && logMsg.Length != 0)
                {
                    fullLog += logMsg;
                }
            }
        }

        if (fullLog.Length != 0)
            Debug.Log("[RecastNavigation] " + fullLog);

        RecastNavigationDllImports.FlushGwNavLog();
    }
}