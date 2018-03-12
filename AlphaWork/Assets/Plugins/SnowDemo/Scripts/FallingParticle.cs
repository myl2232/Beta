using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingParticle : MonoBehaviour {
    public float Height = 35.0f;
    public float ForwardOffset = -7.0f;

    public Camera gameCamera;
    public ParticleSystem srcParticle = null;
    

    public float Intensity;
    public float IntensityMultiplier = 1.0f;
    public int BaseEmissionRate = 10;

    private ParticleSystem particleSystem;

    private float lastIntensityValue = -1.0f;
    private float lastIntensityMultiplierValue = -1.0f;

    protected float InitialStartSpeed { get; private set; }
    protected float InitialStartSize { get; private set; }
    protected Material Material { get; private set; }

    private void CreateMeshEmitter(ParticleSystem p)
    {
        if (p == null || p.shape.shapeType != ParticleSystemShapeType.Mesh)
        {
            return;
        }

        Mesh emitter = new Mesh();
        emitter.vertices = new Vector3[]
            {
                new Vector3(-5.0f, 0.0f, 0.0f),
                new Vector3(5.0f, 0.0f, 0.0f),
                new Vector3(-70.0f, 0.0f, 50.0f),
                new Vector3(70.0f, 0.0f, 50.0f)
            };
        emitter.triangles = new int[] { 0, 1, 2, 2, 1, 3 };

        var s = p.shape;
        s.mesh = emitter;
    }

    private void TransformParticleSystem(ParticleSystem p, float forward, float height, float rotationYModifier)
    {
        if (p == null)
        {
            return;
        }

        Vector3 pos = gameCamera.transform.position;
        Vector3 anchorForward = gameCamera.transform.forward;
        pos.x += anchorForward.x * forward;
        pos.y += height;
        pos.z += anchorForward.z * forward;
        p.transform.position = pos;
        Vector3 angles = p.transform.rotation.eulerAngles;
        p.transform.rotation = Quaternion.Euler(angles.x, gameCamera.transform.rotation.eulerAngles.y * rotationYModifier, angles.z);
    }

    protected virtual void UpdatePositions()
    {
        // keep particles and mist above the player
        TransformParticleSystem(particleSystem, ForwardOffset, Height, 1.0f);
    }

    private Material InitParticleSystem(ParticleSystem p, bool perPixelParticles)
    {
        if (p == null)
        {
            return null;
        }

        Renderer renderer = p.GetComponent<Renderer>();
        Material m = new Material(renderer.material);
        if (perPixelParticles && SystemInfo.graphicsShaderLevel >= 30)
        {
            m.EnableKeyword("PER_PIXEL_LIGHTING");
        }
        else
        {
            m.DisableKeyword("PER_PIXEL_LIGHTING");
        }
        renderer.material = m;

        return m;
    }

    private void CheckForIntensityChange()
    {
        Intensity = TintManager.Instance.Speed;

        if (lastIntensityValue == Intensity && lastIntensityMultiplierValue == IntensityMultiplier)
        {
            return;
        }

        lastIntensityValue = Intensity;
        lastIntensityMultiplierValue = IntensityMultiplier;

        if (Intensity < 0.001f)
        {
            if (particleSystem != null)
            {
                particleSystem.Stop();
            }
        }
        else
        {
            if (particleSystem != null)
            {
                var e = particleSystem.emission;
                ParticleSystem.MinMaxCurve rate = particleSystem.emission.rateOverTime;
                rate.mode = ParticleSystemCurveMode.Constant;
                rate.constantMin = rate.constantMax = BaseEmissionRate * Intensity * IntensityMultiplier;
                var m = particleSystem.main;
                m.maxParticles = (int)Mathf.Max(particleSystem.main.maxParticles, rate.constantMax * particleSystem.main.startLifetimeMultiplier);
                e.rateOverTime = rate;
                if (!particleSystem.isPlaying)
                {
                    particleSystem.Play();
                }
                Debug.Log("Rate: " + particleSystem.emission.rateOverTime.constantMax);
            }           
        }
    }

	// Use this for initialization
	void Start () {
        gameCamera = Camera.main;

        particleSystem = GameObject.Instantiate(srcParticle) as ParticleSystem;

        if(particleSystem == null)
        {
            Debug.LogError("ParticleSystem == null");
            return;
        }

        InitialStartSpeed = particleSystem.main.startSpeedMultiplier;
        InitialStartSize = particleSystem.main.startSpeedMultiplier;

        Material = InitParticleSystem(particleSystem, false);

        CreateMeshEmitter(particleSystem);
	}
	
	// Update is called once per frame
	void Update () {
        CheckForIntensityChange();
	}
}
