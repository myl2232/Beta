using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowFPSTest : MonoBehaviour {
    public GameObject houseNoTopUV = null;
    public GameObject houseHasTopUV = null;
    public GameObject houseSub = null;

    private bool noTopUVShow;
    private bool hasTopUVShow;
    private bool subShow;
	// Use this for initialization
    void Start()
    {
        noTopUVShow = true;
        hasTopUVShow = false;
        subShow = false;
    }
	
    //// Update is called once per frame
    //void Update () {
		
    //}

    void OnGUI()
    {
        //if (GUI.Button(new Rect(Screen.width - 1150, 20, 200, 200), "noTopUVShow"))
        // {
        //     noTopUVShow = !noTopUVShow;
        //     houseNoTopUV.SetActive(noTopUVShow);
        //    if (noTopUVShow == true)
        //    {
        //        hasTopUVShow = false;                
        //        subShow = false;
        //        houseHasTopUV.SetActive(hasTopUVShow);
        //        houseSub.SetActive(subShow);
        //    }
            
        // }

        //if (GUI.Button(new Rect(Screen.width - 850, 20, 200, 200), "hasTopUVShow"))
        //{
        //    hasTopUVShow = !hasTopUVShow;
        //    houseHasTopUV.SetActive(hasTopUVShow);
        //    if (houseHasTopUV == true)
        //    {
        //        noTopUVShow = false;
        //        subShow = false;
        //        houseNoTopUV.SetActive(noTopUVShow);
        //        houseSub.SetActive(subShow);
        //    }
        //}

        //if (GUI.Button(new Rect(Screen.width - 550, 20, 200, 200), "subShow"))
        //{
        //    subShow = !subShow;
        //    houseSub.SetActive(subShow);
        //    if (subShow == true)
        //    {
        //        noTopUVShow = false;
        //        hasTopUVShow = false;
        //        houseNoTopUV.SetActive(noTopUVShow);
        //        houseHasTopUV.SetActive(hasTopUVShow);
        //    }
        //}
    }
}
