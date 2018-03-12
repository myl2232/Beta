
using UnityEngine;
using System.Collections;

/// <summary>
/// 帧率显示类
/// </summary>
public class FPS : MonoBehaviour
{
    /// <summary>
    /// 每次刷新计算的时间      帧/秒
    /// </summary>
    public float updateInterval = 0.5f;
    /// <summary>
    /// 最后间隔结束时间
    /// </summary>
    private double lastInterval;
    private int frames = 0;
    private float currFPS;
    private GUIStyle frontStyle;

    // Use this for initialization
    void Start()
    {
        //DontDestroyOnLoad(this.gameObject);
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
        frontStyle = new GUIStyle();
        frontStyle.normal.textColor = Color.red;
    }

    // Update is called once per frame
    void Update()
    {

        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            currFPS = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 10, 200, 200), "FPS:" + currFPS.ToString("f2"),frontStyle);
    }

}
