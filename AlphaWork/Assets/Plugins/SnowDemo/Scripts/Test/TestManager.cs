using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager{
    protected List<Material> testMatList;

    //protected 

    private static TestManager s_Instance;
    public static TestManager Instance
    {
        get
        {
            return s_Instance ?? (s_Instance = new TestManager());
        }
    }

    public void Init()
    {
        testMatList = new List<Material>();
    }

    public void AddMat(Material mat)
    {
        if (testMatList.Contains(mat) == false)
        {
            testMatList.Add(mat);
        }        
    }

    public void Tick()
    {

    }
}
