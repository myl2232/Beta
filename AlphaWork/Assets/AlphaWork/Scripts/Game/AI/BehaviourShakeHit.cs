using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourShakeHit : MonoBehaviour {

    public string SkeletonRoot = "EthanHips";
    public float Duration = 0.2f;    
    public Vector3 Amplitude = new Vector3(10, 10, 10);
    public Vector3 Frequence = new Vector3(15, 15, 15);
    protected Vector3 shakeVarint = new Vector3();

    private GameObject Parent;
    private float m_lastShrinkTime;
    private Vector3 m_rootPos; 
    private float m_startTime = 0;    
    private bool m_bShake = false;
    private int m_forward = 1;
    private float m_updateTime = 0;

    // Use this for initialization
    void Start () {
        //给Parent赋值
        Parent = gameObject?gameObject:transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update () {

        Detect();
	}

    //shake调用通知
    public void OnShake()
    {
        m_lastShrinkTime = Time.realtimeSinceStartup;
        m_rootPos = Parent.transform.position;
        m_startTime = Time.realtimeSinceStartup;
        m_bShake = true;
    }

    private void LateUpdate()
    {
        float[] listFrequence = { Frequence[0], Frequence[1], Frequence[2] };
        if (m_bShake && Time.realtimeSinceStartup-m_updateTime >= 1.0f/ Mathf.Max(listFrequence))
        {
            m_forward *= -1;

            float lastTime = Time.realtimeSinceStartup - m_startTime;
            float mTime = lastTime / Duration;
            float k = 1.0f - mTime;
            //选择第一个skinmesh做为可以抖动的模型
            SkinnedMeshRenderer[] smr = Parent.GetComponentsInChildren<SkinnedMeshRenderer>();
            
            for (int i = 0; i < smr[0].bones.Length; ++i)
            {
                if (smr[0].bones[i].name.Equals(SkeletonRoot))//bone_root01
                {                   
                    for (int j = 0; j < 3; ++j)
                    {
                        shakeVarint[j] = m_forward * Parent.transform.forward[j] * Mathf.Sin(Frequence[j] * lastTime) * k * Amplitude[j];
                    }

                    smr[0].bones[i].transform.position += shakeVarint;

                    return;
                }
            }

            m_updateTime = Time.realtimeSinceStartup;
        }
    }

    protected void Detect()
    {
        float curTime = Time.realtimeSinceStartup;
        if (curTime - m_lastShrinkTime > Duration && m_bShake)
        {
            m_lastShrinkTime = curTime;
            
            SkinnedMeshRenderer[] smr = Parent.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < smr[0].bones.Length; ++i)
            {
                if (smr[0].bones[i].name.Equals(SkeletonRoot))
                {
                    smr[0].bones[i].transform.position = m_rootPos;
                    break;
                }
            }

            m_bShake = false;
            m_startTime = 0;
        }
    }



}
