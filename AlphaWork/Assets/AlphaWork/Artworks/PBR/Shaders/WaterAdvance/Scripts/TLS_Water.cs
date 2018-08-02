using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using RenderEngine.Adapter;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
// Make water live-update even when not in play mode
public class TLS_Water : MonoBehaviour
{
    public enum WaterMode
    {
        Simple = 0,
        Cube = 1,
        Reflective = 2,
        Refractive = 3,
    };

    public WaterMode m_WaterMode = WaterMode.Refractive;

    [HideInInspector]
    private const bool
        m_DisablePixelLights = true;

    [SerializeField]
    private int m_TextureSize = 256;
    public float m_ClipPlaneOffset = 0.07f;

    [HideInInspector]
    private LayerMask
        m_ReflectLayers = 1 << 10 | 1 << 30;

    [HideInInspector]
    private LayerMask
        m_RefractLayers = 1 << 10 | 1 << 30;

    private Dictionary<Camera, Camera> m_ReflectionCameras = new Dictionary<Camera, Camera>(); // Camera -> Camera table
    private Dictionary<Camera, Camera> m_RefractionCameras = new Dictionary<Camera, Camera>(); // Camera -> Camera table

    private RenderTexture m_ReflectionTexture = null;
    private RenderTexture m_RefractionTexture = null;
    private RenderTextureFormat m_RenderTextureFormat = RenderTextureFormat.Default;
    private WaterMode m_HardwareWaterSupport = WaterMode.Refractive;
    private int m_OldReflectionTextureSize = 0;
    private int m_OldRefractionTextureSize = 0;

    private static bool s_InsideWater = false;

    private MeshRenderer waterRenderer = null;
    private Material[] waterMaterials = null;

    private Transform waterTrans = null, camTrans = null, reflectionCamTrans = null, refractionCamTrans = null;

    void Awake()
    {
        RenderTextureFormat[] optionalFormats = new RenderTextureFormat[]
{ RenderTextureFormat.RGB565, RenderTextureFormat.ARGB1555, RenderTextureFormat.ARGB4444, RenderTextureFormat.ARGB32, RenderTextureFormat.Default  };
        for (int i = 0; i < optionalFormats.Length; ++i)
        {
            if (SystemInfo.SupportsRenderTextureFormat(optionalFormats[i]))
            {
                m_RenderTextureFormat = optionalFormats[i];
                break;
            }
        }

        waterRenderer = GetComponent<MeshRenderer>();
        waterMaterials = waterRenderer.sharedMaterials;

        // Actual water rendering mode depends on both the current setting AND
        // the hardware support. There's no point in rendering refraction textures
        // if they won't be visible in the end.
        m_HardwareWaterSupport = FindHardwareWaterSupport();

        waterTrans = this.transform;
    }

    void Start()
    {
    }

    // This is called when it's known that the object will be rendered by some
    // camera. We render reflections / refractions and do other updates here.
    // Because the script executes in edit mode, reflections for the scene view
    // camera will just work!
    public void OnWillRenderObject()
    {
        if (!enabled || !waterRenderer.enabled || waterMaterials.Length <= 0)
            return;

        Camera cam = Camera.current;
        if (!cam)
            return;

        camTrans = cam.transform;

        // Safeguard from recursive water reflections.		
        if (s_InsideWater)
            return;
        s_InsideWater = true;

        WaterMode mode = GetWaterMode();

        Camera reflectionCamera = null, refractionCamera = null;
        CreateWaterObjects(cam, out reflectionCamera, out refractionCamera);

        if (reflectionCamera != null)
            reflectionCamTrans = reflectionCamera.transform;
        if (refractionCamera != null)
            refractionCamTrans = refractionCamera.transform;

        // find out the reflection plane: position and normal in world space
        Vector3 pos = waterTrans.position;
        Vector3 normal = waterTrans.up;

        // Optionally disable pixel lights for reflection/refraction
        int oldPixelLightCount = QualitySettings.pixelLightCount;
        if (m_DisablePixelLights)
            QualitySettings.pixelLightCount = 0;

        // Render reflection if needed
        if (mode >= WaterMode.Reflective)
        {
            // Reflect camera around reflection plane
            float d = -Vector3.Dot(normal, pos) - m_ClipPlaneOffset;
            Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

            Matrix4x4 reflection = Matrix4x4.zero;
            CalculateReflectionMatrix(ref reflection, reflectionPlane);
            Vector3 oldpos = camTrans.position;
            Vector3 newpos = reflection.MultiplyPoint(oldpos);
            reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

            // Setup oblique projection matrix so that near plane is our reflection
            // plane. This way we clip everything below/above it for free.
            Vector4 clipPlane = CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);
            reflectionCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);

            reflectionCamera.cullingMask = ~(1 << 4) & m_ReflectLayers.value; // never render water layer
            reflectionCamera.targetTexture = m_ReflectionTexture;
            GL.SetRevertBackfacing(true);
            reflectionCamTrans.position = newpos;
            Vector3 euler = camTrans.eulerAngles;
            reflectionCamTrans.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);
            reflectionCamera.Render();
            reflectionCamTrans.position = oldpos;
            GL.SetRevertBackfacing(false);

            if (waterMaterials != null)
            {
                if (waterMaterials[0] != null)
                    waterMaterials[0].SetTexture("_ReflectionTex", m_ReflectionTexture);
                if (waterMaterials.Length > 1 && waterMaterials[1] != null)
                {
                    waterMaterials[1].SetTexture("_ReflectionTex", m_ReflectionTexture);
                }
            }
        }

        // Render refraction
        if (mode >= WaterMode.Refractive)
        {
            refractionCamera.worldToCameraMatrix = cam.worldToCameraMatrix;

            // Setup oblique projection matrix so that near plane is our reflection
            // plane. This way we clip everything below/above it for free.
            Vector4 clipPlane = CameraSpacePlane(refractionCamera, pos, normal, -1.0f);
            refractionCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);

            refractionCamera.cullingMask = ~(1 << 4) & m_RefractLayers.value; // never render water layer
            refractionCamera.targetTexture = m_RefractionTexture;
            refractionCamTrans.position = camTrans.position;
            refractionCamTrans.rotation = camTrans.rotation;
            refractionCamera.Render();

            if (waterMaterials != null)
            {
                if (waterMaterials[0] != null)
                    waterMaterials[0].SetTexture("_RefractionTex", m_RefractionTexture);
                if (waterMaterials.Length > 1 && waterMaterials[1] != null)
                {
                    waterMaterials[1].SetTexture("_RefractionTex", m_RefractionTexture);
                }
            }
        }

        // Restore pixel light count
        if (m_DisablePixelLights)
            QualitySettings.pixelLightCount = oldPixelLightCount;

        // Setup shader keywords based on water mode
        switch (mode)
        {
            case WaterMode.Simple:
                Shader.EnableKeyword("WATER_SIMPLE");
                Shader.DisableKeyword("WATER_CUBE");
                Shader.DisableKeyword("WATER_REFLECTIVE");
                Shader.DisableKeyword("WATER_REFRACTIVE");
                break;
            case WaterMode.Cube:
                Shader.DisableKeyword("WATER_SIMPLE");
                Shader.EnableKeyword("WATER_CUBE");
                Shader.DisableKeyword("WATER_REFLECTIVE");
                Shader.DisableKeyword("WATER_REFRACTIVE");
                break;

            case WaterMode.Reflective:
                Shader.DisableKeyword("WATER_SIMPLE");
                Shader.DisableKeyword("WATER_CUBE");
                Shader.EnableKeyword("WATER_REFLECTIVE");
                Shader.DisableKeyword("WATER_REFRACTIVE");
                break;
            case WaterMode.Refractive:
                Shader.DisableKeyword("WATER_SIMPLE");
                Shader.DisableKeyword("WATER_CUBE");
                Shader.DisableKeyword("WATER_REFLECTIVE");
                Shader.EnableKeyword("WATER_REFRACTIVE");
                break;
        }

        s_InsideWater = false;
    }


    // Cleanup all the objects we possibly have created
    void OnDisable()
    {
        ClearResources();
    }

    // This just sets up some matrices in the material; for really
    // old cards to make water texture scroll.
    void Update()
    {
        WaterMode mode = GetWaterMode();
        if (mode != WaterMode.Cube)
        {
            if (waterMaterials.Length <= 0)
                return;

            float waveScale = waterMaterials[0].GetFloat("_WaveScale");
            Vector4 waveScale4 = new Vector4(waveScale, waveScale, waveScale * 0.4f, waveScale * 0.45f);
            waterMaterials[0].SetVector("_WaveScale4", waveScale4);
            if (waterMaterials.Length > 1)
            {
                waterMaterials[1].SetVector("_WaveScale4", waveScale4);
            }
            if (mode != WaterMode.Simple)
            {
                Vector4 waveSpeed = waterMaterials[0].GetVector("WaveSpeed");
                // Time since level load, and do intermediate calculations with doubles
                double t = Time.timeSinceLevelLoad / 20.0;
                Vector4 offsetClamped = new Vector4(
                    (float)System.Math.IEEERemainder(waveSpeed.x * waveScale4.x * t, 1.0),
                    (float)System.Math.IEEERemainder(waveSpeed.y * waveScale4.y * t, 1.0),
                    (float)System.Math.IEEERemainder(waveSpeed.z * waveScale4.z * t, 1.0),
                    (float)System.Math.IEEERemainder(waveSpeed.w * waveScale4.w * t, 1.0)
                );
                waterMaterials[0].SetVector("_WaveOffset", offsetClamped);
                if (waterMaterials.Length > 1)
                {
                    waterMaterials[1].SetVector("_WaveOffset", offsetClamped);
                    waterMaterials[1].SetVector("_WaveScale4", waveScale4);
                    float reflectionDistort = waterMaterials[0].GetFloat("_ReflDistort");
                    float refractionDistort = waterMaterials[0].GetFloat("_RefrDistort");
                    waterMaterials[1].SetFloat("_ReflDistort", reflectionDistort);
                    waterMaterials[1].SetFloat("_RefrDistort", refractionDistort);
                }
                Vector3 waterSize = waterRenderer.bounds.size;
                Vector3 scale = new Vector3(waterSize.x * waveScale4.x, waterSize.z * waveScale4.y, 1);
                Matrix4x4 scrollMatrix = Matrix4x4.TRS(new Vector3(offsetClamped.x, offsetClamped.y, 0), Quaternion.identity, scale);
                waterMaterials[0].SetMatrix("_WaveMatrix", scrollMatrix);
                if (waterMaterials.Length > 1)
                {
                    waterMaterials[1].SetMatrix("_WaveMatrix", scrollMatrix);
                }
                scale = new Vector3(waterSize.x * waveScale4.z, waterSize.z * waveScale4.w, 1);
                scrollMatrix = Matrix4x4.TRS(new Vector3(offsetClamped.z, offsetClamped.w, 0), Quaternion.identity, scale);
                waterMaterials[0].SetMatrix("_WaveMatrix2", scrollMatrix);
                if (waterMaterials.Length > 1)
                {
                    waterMaterials[1].SetMatrix("_WaveMatrix2", scrollMatrix);
                }

            }

        }
    }

    void OnDestroy()
    {
        ClearResources();
        m_ReflectionCameras = null;
        m_RefractionCameras = null;

        waterRenderer = null;

        if (waterMaterials != null)
        {
            for (int matIndex = 0; matIndex < waterMaterials.Length; ++matIndex)
            {
                waterMaterials[matIndex] = null;
            }
            waterMaterials = null;
        }

        waterTrans = null;
        camTrans = null;
        reflectionCamTrans = null;
        refractionCamTrans = null;
    }

    private void ClearResources()
    {
        if (m_ReflectionTexture)
        {
            DestroyImmediate(m_ReflectionTexture);
            m_ReflectionTexture = null;
        }
        if (m_RefractionTexture)
        {
            DestroyImmediate(m_RefractionTexture);
            m_RefractionTexture = null;
        }
        foreach (KeyValuePair<Camera, Camera> kvp in m_ReflectionCameras)
            DestroyImmediate((kvp.Value).gameObject);
        m_ReflectionCameras.Clear();
        foreach (KeyValuePair<Camera, Camera> kvp in m_RefractionCameras)
            DestroyImmediate((kvp.Value).gameObject);
        m_RefractionCameras.Clear();
    }

    private void UpdateCameraModes(Camera src, Camera dest)
    {
        if (dest == null)
            return;
        // set water camera to clear the same way as current camera
        dest.clearFlags = CameraClearFlags.SolidColor;
        dest.backgroundColor = src.backgroundColor;
        // update other values to match current camera.
        // even if we are supplying custom camera&projection matrices,
        // some of values are used elsewhere (e.g. skybox uses far plane)
        dest.farClipPlane = src.farClipPlane;
        dest.nearClipPlane = src.nearClipPlane;
        dest.orthographic = src.orthographic;
        dest.fieldOfView = src.fieldOfView;
        dest.aspect = src.aspect;
        dest.orthographicSize = src.orthographicSize;
    }

    // On-demand create any objects we need for water
    private void CreateWaterObjects(Camera currentCamera, out Camera reflectionCamera, out Camera refractionCamera)
    {
        WaterMode mode = GetWaterMode();

        reflectionCamera = null;
        refractionCamera = null;

        if (mode >= WaterMode.Reflective)
        {
            // Reflection render texture
            if (!m_ReflectionTexture || m_OldReflectionTextureSize != m_TextureSize)
            {
                if (m_ReflectionTexture)
                    DestroyImmediate(m_ReflectionTexture);
                m_ReflectionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16, m_RenderTextureFormat);
                m_ReflectionTexture.name = "__WaterReflection" + GetInstanceID();
                m_ReflectionTexture.isPowerOfTwo = true;
                m_ReflectionTexture.hideFlags = HideFlags.DontSave;
                m_OldReflectionTextureSize = m_TextureSize;
            }

            // Camera for reflection
            m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera);
            if (!reflectionCamera)
            { // catch both not-in-dictionary and in-dictionary-but-deleted-GO
                GameObject go = new GameObject("Water Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
                reflectionCamera = go.GetComponent<Camera>();
                reflectionCamera.enabled = false;
                reflectionCamera.transform.position = transform.position;
                reflectionCamera.transform.rotation = transform.rotation;
                reflectionCamera.gameObject.AddComponent<FlareLayer>();
                go.hideFlags = HideFlags.HideAndDontSave;
                m_ReflectionCameras[currentCamera] = reflectionCamera;
                UpdateCameraModes(currentCamera, reflectionCamera);
            }
        }

        if (mode >= WaterMode.Refractive)
        {
            // Refraction render texture
            if (!m_RefractionTexture || m_OldRefractionTextureSize != m_TextureSize)
            {
                if (m_RefractionTexture)
                    DestroyImmediate(m_RefractionTexture);
                m_RefractionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16, m_RenderTextureFormat);
                m_RefractionTexture.name = "__WaterRefraction" + GetInstanceID();
                m_RefractionTexture.isPowerOfTwo = true;
                m_RefractionTexture.hideFlags = HideFlags.DontSave;
                m_OldRefractionTextureSize = m_TextureSize;
            }

            // Camera for refraction
            m_RefractionCameras.TryGetValue(currentCamera, out refractionCamera);
            if (!refractionCamera)
            { // catch both not-in-dictionary and in-dictionary-but-deleted-GO
                GameObject go = new GameObject("Water Refr Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
                refractionCamera = go.GetComponent<Camera>();
                refractionCamera.enabled = false;
                refractionCamera.transform.position = transform.position;
                refractionCamera.transform.rotation = transform.rotation;
                refractionCamera.gameObject.AddComponent<FlareLayer>();
                go.hideFlags = HideFlags.HideAndDontSave;
                m_RefractionCameras[currentCamera] = refractionCamera;
                UpdateCameraModes(currentCamera, refractionCamera);
            }
        }
    }

    public WaterMode GetWaterMode()
    {
        //Camera.main.depthTextureMode |= DepthTextureMode.Depth;	
        if (m_HardwareWaterSupport < m_WaterMode)
            return m_HardwareWaterSupport;
        else
        {
#if GAME
            if (IPlatformAdapter.Instance != null && IPlatformAdapter.Instance.RenderAdapter != null)
            {
                switch (IPlatformAdapter.Instance.RenderAdapter.WaterLevel)
                {
                    case 0:
                        return WaterMode.Simple;
                    case 1:
                        return WaterMode.Cube;
                    case 2:
                        return WaterMode.Reflective;
                    case 3:
                        return WaterMode.Refractive;
                    default:
                        return WaterMode.Simple;
                }
            }
            return WaterMode.Simple; ;
#else
            return m_WaterMode;
#endif
        }

    }

    private WaterMode FindHardwareWaterSupport()
    {
        if (!SystemInfo.supportsRenderTextures || !GetComponent<Renderer>())
            return WaterMode.Simple;


        return WaterMode.Refractive;

        // 		Material mat = renderer.sharedMaterial;
        // 		if( !mat )
        // 			return WaterMode.Simple;
        // 			
        // 		string mode = mat.GetTag("WATERMODE", false);
        // 		if( mode == "Refractive" )
        // 			return WaterMode.Refractive;
        // 		if( mode == "Reflective" )
        // 			return WaterMode.Reflective;
        // 			
        // 		return WaterMode.Simple;
    }

    // Extended sign: returns -1, 0 or 1 based on sign of a
    private static float sgn(float a)
    {
        if (a > 0.0f)
            return 1.0f;
        if (a < 0.0f)
            return -1.0f;
        return 0.0f;
    }

    // Given position/normal of the plane, calculates plane in camera space.
    private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + normal * m_ClipPlaneOffset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    // Calculates reflection matrix around the given plane
    private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
    {
        reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
        reflectionMat.m01 = (-2F * plane[0] * plane[1]);
        reflectionMat.m02 = (-2F * plane[0] * plane[2]);
        reflectionMat.m03 = (-2F * plane[3] * plane[0]);

        reflectionMat.m10 = (-2F * plane[1] * plane[0]);
        reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
        reflectionMat.m12 = (-2F * plane[1] * plane[2]);
        reflectionMat.m13 = (-2F * plane[3] * plane[1]);

        reflectionMat.m20 = (-2F * plane[2] * plane[0]);
        reflectionMat.m21 = (-2F * plane[2] * plane[1]);
        reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
        reflectionMat.m23 = (-2F * plane[3] * plane[2]);

        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;
    }
}
