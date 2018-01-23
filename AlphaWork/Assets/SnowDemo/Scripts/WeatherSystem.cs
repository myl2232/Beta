using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSystem{

    private static WeatherSystem s_Instance;
    public static WeatherSystem Instance
    {
        get
        {
            return s_Instance ?? (s_Instance = new WeatherSystem());
        }
    }

    public enum WeatherType
    {
        SUN,
        RAIN,
        SNOW,
        MAX
    }

    protected ParticleSystem snowFallParticleSys;
    public ParticleSystem SnowFallParticleSys { set { snowFallParticleSys = value; } get { return snowFallParticleSys; } }

    protected ParticleSystem rainExplodeParticleSys;
    public ParticleSystem RainExplodeParticleSys { set { rainExplodeParticleSys = value; } get { return rainExplodeParticleSys; } }

    protected ParticleSystem rainFallParticleSys;
    public ParticleSystem RainFallParticleSys { set { rainFallParticleSys = value; } get { return rainFallParticleSys; } }

    protected Color weatherTintColor;
    public Color WeatherTintColor { set { weatherTintColor = value; } get { return weatherTintColor; } }

    protected WeatherRain weatherRain;
    protected WeatherSnow weatherSnow;
    protected Weather weatherDefault;

    //当前的天气
    protected Weather curWeather;
    public Weather CurWeather { get { return curWeather; } }
    //前一个天气
    protected Weather lastWeather;
    public Weather LastWeather { get { return lastWeather; } }
    //转换到的天气
    protected Weather targetWeather;
    public Weather TargetWeather { get { return targetWeather; } }

    protected Camera gameCurCam;
    public Camera GameCurCam { set { gameCurCam = value; } get { return gameCurCam; } }

	public virtual WeatherType CurWeatherType
    {
        get
        {
            return curWeatherType;
        }
    }
    protected float expParticleHeight;
    protected WeatherType curWeatherType;
    protected WeatherType lastWeatherType;
    protected WeatherType targetWeatherType;

    //protected List<WeatherComponent> weatherObjList = new List<WeatherComponent>();
    //protected List<Material> matList = new List<Material>();
    //protected List<string> changeMatShaderNameList = new List<string>();

    protected IWeatherAgent weatherAgent = null;

    //shader变量
    public int ID_NormalNoisePower { get; private set; }
    //public int ID_CenterWorldPos { get; private set; }
    public int ID_TintPower { get; private set; }
    public int ID_TintColor { get; private set; }

    public IWeatherAgent GetWeatherAgent()
    {
        if (weatherAgent == null)
        {
            Debug.Log(">>>>>>__WeatherAgent is null when you want get it,return Default one maybe has risk");
            return new WeatherDefaultAgent();
        }
        else
            return weatherAgent;
    }

    public void SetWeatherAgent(IWeatherAgent agent)
    {
        weatherAgent = agent;
    }

    public virtual void Init()
    {
        GetWeatherAgent().Init();

        weatherRain = new WeatherRain();
        weatherRain.Init();
        weatherSnow = new WeatherSnow();
        weatherSnow.Init();
        weatherDefault = new Weather();
        weatherDefault.Init();

        expParticleHeight = 0.0f;
        curWeatherType = WeatherType.SUN;
        lastWeatherType = WeatherType.SUN;
        targetWeatherType = WeatherType.SUN;

        curWeather = weatherDefault;
        lastWeather = weatherDefault;
        targetWeather = weatherDefault;

        ID_NormalNoisePower = Shader.PropertyToID("_NormalNoisePower");
        //ID_CenterWorldPos = Shader.PropertyToID("_CenterWorldPos");
        ID_TintPower = Shader.PropertyToID("_TintWeatherPower");
        ID_TintColor = Shader.PropertyToID("_TintWeatherColor");

        Shader.SetGlobalFloat(WeatherSystem.Instance.ID_TintPower, 0);
        Shader.SetGlobalColor(WeatherSystem.Instance.ID_TintColor, weatherTintColor);
        Shader.SetGlobalFloat(WeatherSystem.Instance.ID_NormalNoisePower, 0);
    }

    protected virtual void ChangeWeather(WeatherType weatherType)
    {
        //if (weatherType != curWeatherType && curWeather != null)
        //{
        //    curWeather.WeatherStop();
        //}

        switch(weatherType)
        {
            case WeatherType.RAIN:
                lastWeather = curWeather;
                curWeather = weatherRain;
                lastWeatherType = curWeatherType;
                curWeatherType = WeatherType.RAIN;
                weatherRain.WeatherBegin();
                break;
            case WeatherType.SNOW:
                lastWeather = curWeather;
                curWeather = weatherSnow;
                lastWeatherType = curWeatherType;
                curWeatherType = WeatherType.SNOW;
                weatherSnow.WeatherBegin();
                break;
            default:
                lastWeather = weatherDefault;
                curWeather = weatherDefault;
                lastWeatherType = WeatherType.SUN;
                curWeatherType = WeatherType.SUN;
                weatherDefault.WeatherBegin();
                break;
        }
    }

    public virtual void SetTargetWeather(WeatherType tarWeatherType)
    {
        targetWeatherType = tarWeatherType;
    }

    public virtual void ResumeLastWeather()
    {
        targetWeatherType = lastWeatherType;
    }

    public virtual void ResumeDefaultWeather()
    {
        targetWeatherType = WeatherType.SUN;
    }

    public virtual void Tick()
    {
        //目标天气不是当前天气的时候
        if (targetWeatherType != CurWeatherType)
        {
            //当前天气开始进入结束过程
            if (curWeather.CurrebtState != Weather.WeatherState.STOPED &&
                curWeather.CurrebtState != Weather.WeatherState.STOPPING)
            {
                curWeather.WeatherStop();
            }

            //当前的天气已结束
            if (curWeather.CurrebtState == Weather.WeatherState.STOPED)
            {
                ChangeWeather(targetWeatherType);
            }
        }

        curWeather.Tick();
    }

    public virtual void SetParticleExplodeHeight(float height)
    {
        expParticleHeight = height;
    }

    public virtual float GetParticleExplodeHeight()
    {
        return expParticleHeight;
    }

    public virtual void AddWeatherObj(WeatherComponent wc)
    {
        GetWeatherAgent().AddWeatherComponentObj(wc);
    }

    public virtual void RemoveWeatherObj(WeatherComponent wc)
    {
        GetWeatherAgent().RemoveWeatherComponentObj(wc);
    }

    public virtual void AddWeatherMat(Material weatherMat)
    {
        GetWeatherAgent().AddWeatherMat(weatherMat);
    }

    public virtual void AddWeatherMats(Material[] weatherMats)
    {
        for(int index = 0; index < weatherMats.Length; index++)
        {
            AddWeatherMat(weatherMats[index]);
        }
    }

    public virtual void RemoveWeatherObj(Material weatherMat)
    {
        GetWeatherAgent().RemoveWeatherMat(weatherMat);
    }

    public virtual void ClearList()
    {
        GetWeatherAgent().ClearList();
        //weatherObjList.Clear();
        //matList.Clear();
    }

    public virtual List<WeatherComponent> GetRenderList()
    {
        return GetWeatherAgent().GetWeatherComponentList();
        //return weatherObjList;
    }

    public virtual void SetSnowSpeed(float s)
    {
        this.weatherSnow.PowerStepMultiplier = s;
    }

    public virtual List<Material> GetMatList()
    {
        return GetWeatherAgent().GetWeatherMatList();
        //return matList;
    }

    public virtual void AddChangeMatShaderName(string shaderName)
    {
        GetWeatherAgent().AddWeatherShaderName(shaderName);
    }

    //public virtual void OnWeatherBegin(Weather weatherCur)
    //{

    //}

    //public virtual void OnWeatherExcute(Weather weatherCur)
    //{

    //}

    //public virtual void OnWeatherStop(Weather weatherCur)
    //{

    //}

}
