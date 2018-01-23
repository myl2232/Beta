using UnityEngine;
using System.Collections;

public class OccTransparent : MonoBehaviour {
	public GameObject m_Hero;  //得到主角
	public float FadeInTimeout = 0.6f;
	public float FadeOutTimeout = 0.2f;
	public float TargetTransparency = 0.3f;
    public Vector3 Height = Vector3.up * 1f;
    public float RayExtendLen = 1000.0f;
    private Transform cachedTransform;

    private int rayCastMask;

    private void Awake()
    {
        rayCastMask = 1 << LayerMask.NameToLayer("TransparentFX");
    }

	private void Update() {

        if (m_Hero == null)
        {
            return;
        }

        cachedTransform = m_Hero.transform;
		RaycastHit[] hits;
        //Ray ray = new Ray(cachedTransform.position + Height, this.transform.position - cachedTransform.position - Height);
        Vector3 vecDir = cachedTransform.position + Height - this.transform.position;
        vecDir.Normalize();
        Vector3 rayOriPos = this.transform.position + vecDir * -RayExtendLen;
        Ray ray = new Ray(rayOriPos, vecDir);

        float dis = Vector3.Distance(rayOriPos, cachedTransform.position + Height);
        hits = Physics.RaycastAll(ray, dis, rayCastMask);
       
        foreach (RaycastHit hit in hits)
        {
            Renderer R = hit.collider.GetComponent<Renderer>();
           
            Occtrans(R);
            for (int i = 0; i < hit.collider.transform.childCount; i++)
            {
                Renderer childRender = hit.collider.transform.GetChild(i).gameObject.GetComponent<Renderer>();
                Occtrans(childRender);
            }

        }
    }

    void Occtrans(Renderer R)
    {
        if (R == null)
        {
            return;
        }

        AutoTransparent AT = R.GetComponent<AutoTransparent>();
        if (AT == null) // if no script is attached, attach one
        {
            AT = R.gameObject.AddComponent<AutoTransparent>();
            AT.FadeInTimeout = FadeInTimeout;
            AT.FadeOutTimeout = FadeOutTimeout;
            AT.TargetTransparency = TargetTransparency;
        }
        AT.BeTransparent(); // get called every frame to reset the falloff
    }

}
