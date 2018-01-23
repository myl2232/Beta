using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherRain : Weather
{
    private float rainPower;
    

    public override void Init()
    {
        base.Init();

        ParticleSystem fallParticle = WeatherSystem.Instance.RainFallParticleSys;
        if (fallParticle != null)
        {
            FallParticle = GameObject.Instantiate(fallParticle) as ParticleSystem;
            FallParticle.Stop();
        }

        ParticleSystem expParticle = WeatherSystem.Instance.RainExplodeParticleSys;
        if (expParticle != null)
        {
            ExplodeParticle = GameObject.Instantiate(expParticle) as ParticleSystem;
            ExplodeParticle.Stop();
        }

        rainPower = 0.0f;
        weatherType = WeatherSystem.WeatherType.RAIN;
    }

    public override void Tick()
    {
        base.Tick();

        List<WeatherComponent> renderList = WeatherSystem.Instance.GetRenderList();
        rainPower = weatherPower;

        for(int index = 0; index < renderList.Count; index++)
        {
            WeatherComponent wc = (WeatherComponent)(renderList[index]);
            for(int indexInRenderArr = 0; indexInRenderArr < wc.RenderArr.Length; indexInRenderArr++)
            {
                //wc.RenderArr[indexInRenderArr].material.SetFloat("_NormalNoisePower", rainPower);
                wc.RenderArr[indexInRenderArr].material.SetVector("_CenterWorldPos", wc.RenderArr[indexInRenderArr].gameObject.transform.position);
            }
        }

        Shader.SetGlobalFloat(WeatherSystem.Instance.ID_NormalNoisePower, rainPower);
        //List<Material> matList = WeatherSystem.Instance.GetMatList();
        //for (int index = 0; index < matList.Count; index++)
        //{
        //    matList[index].SetFloat("_NormalNoisePower", rainPower);
        //}
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

        if (explodeParticle != null && gameCamera != null)
        {
            explodeParticle.transform.parent = gameCamera.transform;
            Vector3 newPosExp = gameCamera.transform.position + gameCamera.transform.forward * 2.0f;
            newPosExp.y = WeatherSystem.Instance.GetParticleExplodeHeight() + 0.1f;
            explodeParticle.transform.position = particleOffset == Vector3.zero ?
                newPosExp : new Vector3(newPosExp.x + particleOffset.x, newPosExp.y, newPosExp.z + particleOffset.z);

            if (explodeParticle.isPlaying == false)
            {
                explodeParticle.Play();
            }
        }
        //材质开始变成雨
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

        if (explodeParticle != null)
        {
            explodeParticle.transform.parent = null;

            if (explodeParticle.isPlaying == true)
            {
                explodeParticle.Stop();
            }
        }

        //rainPower = 0.0f;
    }
}
