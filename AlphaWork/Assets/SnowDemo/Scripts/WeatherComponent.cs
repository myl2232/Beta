using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherComponent : MonoBehaviour {

    //public bool updatePos = false;

    protected Renderer[] renderArr = null;

    public Renderer[] RenderArr { get { return renderArr; } }

	// Use this for initialization
	void Start () {
        renderArr = this.gameObject.GetComponentsInChildren<Renderer>();

        for (int index = 0; index < renderArr.Length; index++)
        {
#if UNITY_EDITOR
            WeatherSystem.Instance.AddWeatherMats(renderArr[index].materials);
#else
                WeatherSystem.Instance.AddWeatherMats(renderArr[index].sharedMaterials);
#endif
        }
	}

	// Update is called once per frame
    //纯测试代码
    //void Update()
    //{
    //    if (updatePos == true)
    //    {
    //        BeginUpdateWorldPos();
    //    }
    //    else
    //    {
    //        StopUpdateWorldPos();
    //    }
    //}

    public void BeginUpdateWorldPos()
    {
        if (renderArr.Length > 0)
        {
            WeatherSystem.Instance.AddWeatherObj(this);
        }
    }

    public void StopUpdateWorldPos()
    {
        if (renderArr.Length > 0)
        {
            WeatherSystem.Instance.RemoveWeatherObj(this);
        }
    }

    void OnDestroy()
    {
        WeatherSystem.Instance.RemoveWeatherObj(this);
    }
}
