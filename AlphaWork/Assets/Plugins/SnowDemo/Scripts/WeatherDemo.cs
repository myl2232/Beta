using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherDemo : MonoBehaviour {

    public ParticleSystem rainFallParticle;
    public ParticleSystem rainExplodeParticle;
    public GameObject rainExplodeHeight;
    public ParticleSystem snowFallParticle;
    public Color snowColor = Color.white;
    public string[] changeMatShaderNameArr;

    private bool bRain;
    private bool bSnow;
	// Use this for initialization
	void Awake () {
        //if (rainFallParticle != null)
        //{
        //    WeatherSystem.Instance.RainFallParticleSys = rainFallParticle;
        //}
        //if (rainExplodeParticle != null)
        //{
        //    WeatherSystem.Instance.RainExplodeParticleSys = rainExplodeParticle;
        //}
        //if (snowFallParticle != null)
        //{
        //    WeatherSystem.Instance.SnowFallParticleSys = snowFallParticle;
        //}

        WeatherSystem.Instance.SetWeatherAgent(new WeatherDemoAgent());
        WeatherSystem.Instance.Init();

        if (rainExplodeHeight != null)
        {
            WeatherSystem.Instance.SetParticleExplodeHeight(rainExplodeHeight.transform.position.y);
        }

        bRain = false;
        bSnow = false;

        //for(int index = 0;index < changeMatShaderNameArr.Length;index++)
        //{
        //    WeatherSystem.Instance.AddChangeMatShaderName(changeMatShaderNameArr[index]);
        //}
	}
	
	// Update is called once per frame
	void Update () {
        WeatherSystem.Instance.Tick();
	}

    void FixedUpdate()
    {
        //WeatherSystem.Instance.Tick();
    }

    public void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width - 250, 20, 200, 200), "Rain"))
        {
            bRain = !bRain;
            if (bRain == true)
            {
                WeatherSystem.Instance.SetTargetWeather(WeatherSystem.WeatherType.RAIN);
                bSnow = false;
            }
            else
            {
                WeatherSystem.Instance.ResumeDefaultWeather();
            }
        }

        if (GUI.Button(new Rect(Screen.width - 550, 20, 200, 200), "Snow"))
        {
            bSnow = !bSnow;
            if (bSnow == true)
            {
                WeatherSystem.Instance.SetTargetWeather(WeatherSystem.WeatherType.SNOW);
                bRain = false;
            }
            else
            {
                WeatherSystem.Instance.ResumeDefaultWeather();
            }
        }
    }
}
