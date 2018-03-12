using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather{
    protected const float powerStepValue = 0.01f;

    protected Camera gameCamera;
    public Camera GameCamera { set { gameCamera = value; } }

    protected ParticleSystem fallParticle;
    public ParticleSystem FallParticle { set { fallParticle = value; } get { return fallParticle; } }

    protected ParticleSystem explodeParticle;
    public ParticleSystem ExplodeParticle { set { explodeParticle = value; } get { return explodeParticle; } }

    //天气的强度
    protected float weatherPower;
    public float WeatherPower { set { weatherPower = value; } get { return weatherPower; } }

    protected float weatherPowerStep;
    public float WeatherPowerStep { set { weatherPowerStep = value; } }

    private float powerStepMultiplier;
    public float PowerStepMultiplier { set { powerStepMultiplier = value; } get { return powerStepMultiplier; } }

    protected WeatherState currentState;
    public WeatherState CurrebtState { get { return currentState; } }

    protected WeatherSystem.WeatherType weatherType;
    public WeatherSystem.WeatherType GetWeatherType { get { return weatherType; } }

    /// <summary>
    /// 下落粒子相对摄像机的偏移，x为纵向，y为高度偏移
    /// </summary>
    protected Vector3 particleOffset;
    public Vector3 ParticleOffset { set { particleOffset = value; } get { return particleOffset; } }
    public enum WeatherState
    {
        BEGIN,
        RUN,
        STOPPING,
        STOPED
    }

    public virtual void Init()
    {
        currentState = WeatherState.STOPED;
        weatherPowerStep = powerStepValue;
        powerStepMultiplier = 1.0f;

        gameCamera = Camera.main;
        weatherType = WeatherSystem.WeatherType.MAX;
        particleOffset = new Vector3(0,0,0);
    }

    public virtual void Tick()
    {
        if (currentState == WeatherState.STOPED)
            return;

        weatherPower += weatherPowerStep * powerStepMultiplier;

        if (weatherPower > 1.0f)
        {
            weatherPower = 1.0f;
            currentState = WeatherState.RUN;
        }
        else if (weatherPower < 0.0f)
        {
            weatherPower = 0.0f;
            currentState = WeatherState.STOPED;
            weatherPowerStep = powerStepValue;
            particleOffset = Vector3.zero;
            WeatherSystem.Instance.GetWeatherAgent().OnWeatherStopped(this);
        }
    }

    public virtual void WeatherBegin()
    {
        currentState = WeatherState.BEGIN;
        WeatherSystem.Instance.GetWeatherAgent().OnWeatherBegin(this);
    }

    public virtual void WeatherExcute()
    {
        WeatherSystem.Instance.GetWeatherAgent().OnWeatherExcute(this);
    }

    public virtual void WeatherStop()
    {
        currentState = WeatherState.STOPPING;
        weatherPowerStep = -1 * weatherPowerStep;
        WeatherSystem.Instance.GetWeatherAgent().OnWeatherStopping(this);
    }
}
