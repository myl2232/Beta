using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowProceduralMat : TintColor {

	// Use this for initialization
    //void Start () {
		
    //}
	
	// Update is called once per frame
    //void Update () {
		
    //}


     override protected  void TintColorExcute()
    {
        float tintPower = TintManager.Instance.TintPower;
        Color tintColor = TintManager.Instance.TintColorValue;

        float offsetPower = (int)(tintPower * 50.0f) / 50.0f;

        for (int index = 0; index < renderArr.Length; index++)
        {
            ProceduralMaterial subtance = renderArr[index].material as ProceduralMaterial;
            subtance.SetProceduralFloat("Opacitymult", Mathf.Min(offsetPower, 1.0f));
            subtance.SetProceduralVector("Channelsweights", new Vector4(1, 1, 1, Mathf.Min(offsetPower - 1, 1.0f)));
            subtance.RebuildTextures();
        }
    }
}
