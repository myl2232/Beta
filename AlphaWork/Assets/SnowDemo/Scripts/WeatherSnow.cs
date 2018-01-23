using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSnow : Weather
{
    private float snowPower;
    private Color tintColor;

    public override void Init()
    {
        base.Init();

        ParticleSystem fallParticle = WeatherSystem.Instance.SnowFallParticleSys;
        if (fallParticle != null)
        {
            FallParticle = GameObject.Instantiate(fallParticle) as ParticleSystem;
            FallParticle.Stop();
        }

        snowPower = 0.0f;
        tintColor = Color.white;
        PowerStepMultiplier = 0.05f;
        weatherType = WeatherSystem.WeatherType.SNOW;
    }

    public override void Tick()
    {
        base.Tick();

        List<WeatherComponent> renderList = WeatherSystem.Instance.GetRenderList();
        snowPower = weatherPower * 2;
        tintColor = WeatherSystem.Instance.WeatherTintColor;

        if (fallParticle != null && snowPower < 1.0f)
        {
            var e = fallParticle.emission;
            ParticleSystem.MinMaxCurve rate = fallParticle.emission.rateOverTime;
            rate.mode = ParticleSystemCurveMode.Constant;
            rate.constantMin = rate.constantMax = 100 * snowPower;
            e.rateOverTime = rate;
        }

        for (int index = 0; index < renderList.Count; index++)
        {
            WeatherComponent wc = (WeatherComponent)(renderList[index]);
            for (int indexInRenderArr = 0; indexInRenderArr < wc.RenderArr.Length; indexInRenderArr++)
            {
                wc.RenderArr[indexInRenderArr].material.SetVector("_CenterWorldPos", wc.RenderArr[indexInRenderArr].gameObject.transform.position);
            }
        }

        Shader.SetGlobalFloat(WeatherSystem.Instance.ID_TintPower, snowPower);
        Shader.SetGlobalColor(WeatherSystem.Instance.ID_TintColor, tintColor);
    }

    public override void WeatherBegin()
    {
        base.WeatherBegin();

        //开始播放粒子效果
        if (fallParticle != null && gameCamera != null)
        {
            fallParticle.transform.parent = gameCamera.transform;
            Vector3 newPos = gameCamera.transform.forward * 2.0f;
            newPos.y += 2.0f;
            fallParticle.transform.localPosition = particleOffset == Vector3.zero ? newPos : particleOffset;

            if (fallParticle.isPlaying == false)
            {
                fallParticle.Play();
            }
        }
    }

    public override void WeatherExcute()
    {
        base.WeatherExcute();
    }

    public override void WeatherStop()
    {
        base.WeatherStop();

        //开始播放粒子效果
        if (fallParticle != null)
        {
            fallParticle.transform.parent = null;

            if (fallParticle.isPlaying == true)
            {
                fallParticle.Stop();
            }
        }
    }
}
