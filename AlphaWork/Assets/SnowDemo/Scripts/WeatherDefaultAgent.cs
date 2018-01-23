using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherDefaultAgent : IWeatherAgent
{
    protected List<WeatherComponent> weatherObjList;
    protected List<Material> weatherMatList;
    protected List<string> changeMatShaderNameList;

    public virtual void Init()
    {
        
    }
    public WeatherDefaultAgent()
    {
        weatherObjList = new List<WeatherComponent>();
        weatherMatList = new List<Material>();
        changeMatShaderNameList = new List<string>();
    }

    public virtual List<WeatherComponent> GetWeatherComponentList()
    {
        return weatherObjList;
    }

    public virtual void AddWeatherComponentObj(WeatherComponent comp)
    {
        if (weatherObjList.Contains(comp) == false)
        {
            weatherObjList.Add(comp);
        }
    }
    public virtual void RemoveWeatherComponentObj(WeatherComponent comp)
    {
        if (weatherObjList.Contains(comp) == true)
        {
            weatherObjList.Remove(comp);
        }
    }

    public virtual List<Material> GetWeatherMatList()
    {
        return weatherMatList;
    }

    public virtual void AddWeatherMat(Material mat)
    {
        if (weatherMatList.Contains(mat) == false && IsMatInChangeMatList(mat))
        {
            mat.SetFloat("_TintPower", 0.0f);
            weatherMatList.Add(mat);
        }
    }

    protected bool IsMatInChangeMatList(Material mat)
    {
        string shaderName = mat.shader.name;

        if (changeMatShaderNameList.Contains(shaderName) == true)
        {
            //Debug.Log(" IsMatInChangeMatList " + true);
            return true;
        }

        return false;
    }

    public virtual void RemoveWeatherMat(Material mat)
    {
        if (weatherMatList.Contains(mat) == true)
        {
            weatherMatList.Remove(mat);
        }
    }

    public virtual List<string> GetWeatherShaderNameList()
    {
        return changeMatShaderNameList;
    }

    public virtual void AddWeatherShaderName(string shaderName)
    {
        if (changeMatShaderNameList.Contains(shaderName) == false)
        {
            changeMatShaderNameList.Add(shaderName);
        }
    }

    public virtual void ClearList()
    {
        weatherObjList.Clear();
        weatherMatList.Clear();
        changeMatShaderNameList.Clear();
    }

    public virtual void OnWeatherBegin(Weather weatherCur)
    {

    }

    public virtual void OnWeatherExcute(Weather weatherCur)
    {

    }

    public virtual void OnWeatherStopping(Weather weatherCur)
    {

    }

    public virtual void OnWeatherStopped(Weather weatherCur)
    {

    }
}
