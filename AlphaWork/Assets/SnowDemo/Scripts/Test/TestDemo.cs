using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDemo : MonoBehaviour {

	// Use this for initialization
	void Start () {
        TestManager.Instance.Init();
	}
	
	// Update is called once per frame
	void Update () {
        TestManager.Instance.Tick();
	}
}
