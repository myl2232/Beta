using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherDemoAgent : WeatherDefaultAgent
{
    private GameObject weatherDemoObj = null;


    /// <summary>
    /// 初始化WeatherSystem
    /// </summary>
    public override void Init()
    {
        //base.Init();

        weatherDemoObj = GameObject.Find("GameManager");
        if (weatherDemoObj == null)
        {
            Debug.LogError("Can not Find GameManager Obj");
            return;
        }

        WeatherDemo wd = weatherDemoObj.GetComponent<WeatherDemo>();
        if (wd == null)
        {
            Debug.LogError("Can not Find WeatherDemo Component");
            return;
        }

        if (wd.rainFallParticle != null)
        {
            WeatherSystem.Instance.RainFallParticleSys = wd.rainFallParticle;
        }
        if (wd.rainExplodeParticle != null)
        {
            WeatherSystem.Instance.RainExplodeParticleSys = wd.rainExplodeParticle;
        }
        if (wd.snowFallParticle != null)
        {
            WeatherSystem.Instance.SnowFallParticleSys = wd.snowFallParticle;
        }

        WeatherSystem.Instance.WeatherTintColor = wd.snowColor;

        GameObject go = GameObject.Find("Plane");
        if (go != null)
        {
            WeatherSystem.Instance.SetParticleExplodeHeight(go.transform.position.y);
        }

        for (int index = 0; index < wd.changeMatShaderNameArr.Length; index++)
        {
            WeatherSystem.Instance.AddChangeMatShaderName(wd.changeMatShaderNameArr[index]);
        }
    }

}
