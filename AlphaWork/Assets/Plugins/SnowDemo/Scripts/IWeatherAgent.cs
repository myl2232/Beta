using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeatherAgent {
    void Init();

    List<WeatherComponent> GetWeatherComponentList();

    void AddWeatherComponentObj(WeatherComponent comp);
    void RemoveWeatherComponentObj(WeatherComponent comp);

    List<Material> GetWeatherMatList();

    void AddWeatherMat(Material mat);
    void RemoveWeatherMat(Material mat);

    List<string> GetWeatherShaderNameList();

    void AddWeatherShaderName(string shaderName);

    void ClearList();

    void OnWeatherBegin(Weather weatherCur);

    void OnWeatherExcute(Weather weatherCur);

    void OnWeatherStopping(Weather weatherCur);

    void OnWeatherStopped(Weather weatherCur);
}
