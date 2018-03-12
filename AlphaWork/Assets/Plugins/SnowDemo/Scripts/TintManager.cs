using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TintManager{
    private static TintManager s_Instance;
    public static TintManager Instance
    {
        get
        {
            return s_Instance ?? (s_Instance = new TintManager());
        }
    }

    protected List<TintColor> tintColorObjList;// = new List<TintColor>();
    protected float fTintPower;
    public float TintPower { set { fTintPower = value; } get { return fTintPower; } }
    
    protected float fSpeed;
    public float Speed { set { fSpeed = value; } get { return fSpeed; } }

    protected Color tintColor = Color.white;
    public Color TintColorValue { set { tintColor = value; } get { return tintColor; } }

    protected float powerStep;
    public float PowerStep { set { powerStep = value; } get { return powerStep; } }

    public void Init()
    {
        tintColorObjList = new List<TintColor>();
        fSpeed = 0.0f;
        fTintPower = 0.0f;
        powerStep = 50.0f;
    }

    public void AddTintObj(TintColor obj)
    {
        if (tintColorObjList.Contains(obj) == false)
        {
            tintColorObjList.Add(obj);
        }
    }

    public void RemoveTintObj(TintColor obj)
    {
        if (tintColorObjList.Contains(obj) == true)
        {
            tintColorObjList.Remove(obj);
        }
    }

    public void Tick()
    {
        for(int index = 0;index < tintColorObjList.Count; index++)
        {
            TintColor tc = tintColorObjList[index];
            tc.Tick();
        }
    }

    public void CleanUp()
    {
        tintColorObjList.Clear();
    }
}
