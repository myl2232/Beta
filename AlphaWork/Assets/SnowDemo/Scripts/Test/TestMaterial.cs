using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMaterial : MonoBehaviour {
    
    protected Renderer[] renderArr = null;

    public Renderer[] RenderArr { get { return renderArr; } }
	// Use this for initialization
	void Start () {
        renderArr = this.gameObject.GetComponentsInChildren<Renderer>();

        for (int index = 0; index < renderArr.Length; index++)
        {
            TestManager.Instance.AddMat(renderArr[index].sharedMaterial);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
