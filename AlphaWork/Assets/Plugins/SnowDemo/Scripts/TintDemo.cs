using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TintDemo : MonoBehaviour {

    private bool activeSnow = false;
    private bool isStop = true;
    [Range(0.0f, 10.0f)]
    public float tintSpeed = 0.5f;
    public float tintPower = 0.0f;      //写成public可以监控数值是不是正确
    [Range(0.0f, 15.0f)]
    public float maxTintPower = 10.0f;
    public float tintSpeedFix = 10.0f;
    public float snowHeavy = 0.0f;
    [Range(0.0f, 1.0f)]
    public float snowHeavyMax = 1.0f;
    [Range(0.0f, 10.0f)]
    public float snowDelay = 5.0f;  //这个时间是为了让雪花飘下来，房子才开始变化，否则，雪花还没落下来，房子已经开始变了
    public float delayTimeLeft;         //public是为了监控方便
    private bool bSnowChange = false;
    private bool beginCountDown = false;
    
    public Color tintColor = Color.white;   //统一修改雪的颜色

    [Range(1.0f, 500.0f)]
    public float tintPowerStep = 200.0f;     //步进变化值

    private GUIStyle frontStyle;
	// Use this for initialization
	void Awake () {
        delayTimeLeft = snowDelay;
        frontStyle = new GUIStyle();
        frontStyle.normal.textColor = Color.red;

        TintManager.Instance.Init();
	}
	
	// Update is called once per frame
	void Update () {
        //不希望有雪效果的时候，要把这个Tint功能关掉，刷新常量也比较消耗
		if (activeSnow == true) 
        {

            if (bSnowChange == true)
            {
                beginCountDown = true;
                bSnowChange = false;
                delayTimeLeft = snowDelay;
            }
            
            if (beginCountDown == true)
            {
                delayTimeLeft -= Time.deltaTime;
                delayTimeLeft = delayTimeLeft > 0.0f ? delayTimeLeft : 0.0f; 
            }

            if (isStop == false)
            {
                if (delayTimeLeft == 0.0f)
                {
                    tintPower += Time.deltaTime * tintSpeed / tintSpeedFix;
                    tintPower = tintPower < maxTintPower ? tintPower : maxTintPower;
                }
                snowHeavy += Time.deltaTime * tintSpeed / (tintSpeedFix / 2);
                snowHeavy = snowHeavy < snowHeavyMax ? snowHeavy : snowHeavyMax;
            }
            else
            {
                if (delayTimeLeft == 0.0f)
                {
                    tintPower -= Time.deltaTime * tintSpeed / tintSpeedFix;
                    tintPower = tintPower > 0.0f ? tintPower : 0.0f;
                }
                snowHeavy -= Time.deltaTime * tintSpeed / (tintSpeedFix / 2);
                snowHeavy = snowHeavy > 0.0f ? snowHeavy : 0.0f;
            }
            TintManager.Instance.TintPower = tintPower;
            TintManager.Instance.Speed = snowHeavy;
            TintManager.Instance.TintColorValue = tintColor;
            TintManager.Instance.PowerStep = tintPowerStep;
            TintManager.Instance.Tick();
        }
	}

    public void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width - 250, 20, 200, 200), "Snow"))
        {
            if (activeSnow == false)
                activeSnow = true;

            if (activeSnow == true)
            {
                isStop = !isStop;
                bSnowChange = true;
            }
        }

        if (activeSnow == true)
        {
            if (isStop == true)
            {
                GUI.Label(new Rect(100, 10, 200, 200), "SnowStopping", frontStyle);
            }
            else
            {
                GUI.Label(new Rect(100, 10, 200, 200), "Snowing", frontStyle);
            }
        }
    }
}
