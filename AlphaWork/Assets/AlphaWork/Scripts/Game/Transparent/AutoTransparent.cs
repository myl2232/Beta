using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoTransparent : MonoBehaviour {
	private Material[] mats = null;
    private Material[] shadermats = null;
	//private float m_cutoff = 0.0f;
	private float m_Transparency = 1.0f;
	public float TargetTransparency { get; set; }
	
	public float FadeInTimeout { get; set; }
	
	public float FadeOutTimeout { get; set; }
		
	private bool shouldBeTransparent = true;

	public void BeTransparent() {
		shouldBeTransparent = true;
	}
	
	private void Start() {
		// reset the transparency;
		m_Transparency = 1.0f;

#if UNITY_EDITOR
        shadermats = GetComponent<Renderer>().materials;
        mats = new Material[shadermats.Length];
        for (int i = 0; i < shadermats.Length; i++)
        {
            mats[i] = new Material(shadermats[i]);
            mats[i].name = i.ToString();            // 赋值0、1
        }
        GetComponent<Renderer>().materials = mats;  // 这个时候新的材质实例mats（0、1）成功赋值给了materials，但是materials立马又创建了自己的实例（0(instance)、1(instance)），照理说已经是材质实例后再次调用materials不会产生新实例才对。。。
        for (int i = 0; i < mats.Length; i++ )      // 所以这里要把mats（0、1）销毁
        {
            Destroy(mats[i]);
        }
        mats = GetComponent<Renderer>().materials;  // 重新让mats指向materials（0(instance)、1(instance)）供后续使用。Unity蛋疼的材质实例系统。。。。。。
#else
        shadermats = GetComponent<Renderer>().sharedMaterials;
		mats = GetComponent<Renderer>().materials;
#endif

        for (int index = 0; index < mats.Length; index++) 
		{
			string shadername = mats[index].shader.name + "_OccTransparent"; 			
			if(Shader.Find(shadername))
				mats[index].shader = Shader.Find(shadername);
			else
				mats[index].shader = Shader.Find("Diffuse_OccTransparent");

            if(mats[index].HasProperty("_Color") /*&& shadermats[index].HasProperty("_Color")*/)
            {
                Color sColor = shadermats[index].color;
                mats[index].SetColor("_Color", sColor);
            }
                

            //if(mats[index] && shadermats[index] && 
            //    mats[index].GetColor("_Color") != null && shadermats[index].GetColor("_Color") != null)
            //    mats[index].color = shadermats[index].color;
        }
        
	}
	
	// Update is called once per frame
	private void Update()
    {
        GetComponent<Renderer>().enabled = true;
        
		if (!shouldBeTransparent && m_Transparency >= 1.0) {
			Destroy(this);
		}
		
		//Are we fading in our out?
		if (shouldBeTransparent) {
           
			//Fading in
            if (m_Transparency > TargetTransparency)
            { 
                m_Transparency -= (1.0f - TargetTransparency) * (Time.deltaTime / FadeInTimeout);
                if (m_Transparency < TargetTransparency)
                {
                    m_Transparency = TargetTransparency;
                }
            }
		}
		else {
			//Fading out
            m_Transparency += ((1.0f - TargetTransparency) * Time.deltaTime) / FadeOutTimeout;
		}

		for (int index = 0; index < mats.Length; index++) 
		{
            if (mats[index].HasProperty("_Color"))
            {
                Color col = mats[index].GetColor("_Color");
                col.a = m_Transparency;
                mats[index].SetColor("_Color", col);
            }
		}

		//The object will start to become visible again if BeTransparent() is not called
		shouldBeTransparent = false;
    
	}

    private void OnDestroy()
    {
        this.GetComponent<Renderer>().enabled = true;
        
        foreach (Material mtl in GetComponent<Renderer>().materials)
            Object.Destroy(mtl);

        GetComponent<Renderer>().materials = shadermats;
    }
}
