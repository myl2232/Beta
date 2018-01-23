using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TintColor : MonoBehaviour {

    protected Renderer[] renderArr = null;
    

	// Use this for initialization
	void Start () {
        renderArr = this.gameObject.GetComponentsInChildren<Renderer>();
        if (renderArr.Length > 0)
        {
            TintManager.Instance.AddTintObj(this);
        }
	}
	
	// Update is called once per frame
    public void Tick()
    {
        TintColorExcute();
    }

    virtual protected void TintColorExcute()
    {
        float tintPower = TintManager.Instance.TintPower;
        Color tintColor = TintManager.Instance.TintColorValue;

        float step = TintManager.Instance.PowerStep;
        float offsetPower = (int)(tintPower * step) / step;

        for(int index = 0;index < renderArr.Length;index++)
        {
            renderArr[index].material.SetFloat("_TintPower", offsetPower);
            renderArr[index].material.SetColor("_TintColor", tintColor);
        }
    }
}
