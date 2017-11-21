using UnityEngine;
using System.Collections;

public class JoystackCc : MonoBehaviour
{
    private Vector3 Origin;

    Transform mTrans;

    private Vector3 _deltaPos;
    private bool _drag = false;

    private Vector3 deltaPosition;

 
    [SerializeField]
    private float MoveMaxDistance = 80;          

    [HideInInspector]
    public Vector3 FiexdMovePosiNorm; 

    [HideInInspector]
    public Vector3 MovePosiNorm;  

    [SerializeField]
    private float ActiveMoveDistance = 1;              




    void Awake()
    {
        EventTriggerListener.Get(gameObject).onDrag = OnDrag;
        EventTriggerListener.Get(gameObject).onDragOut = OnDragOut;

        EventTriggerListener.Get(gameObject).onDown = OnMoveStart;
    }

    // Use this for initialization
    void Start()
    {
        Origin = transform.localPosition; 
        mTrans = transform;
    }

    // Update is called once per frame
    void Update()
    {
        float dis = Vector3.Distance(transform.localPosition, Origin);
   
        if (dis >= MoveMaxDistance)       
        {
            Vector3 vec = Origin + (transform.localPosition - Origin) * MoveMaxDistance / dis;   
            transform.localPosition = vec;
        }
        if (Vector3.Distance(transform.localPosition, Origin) > ActiveMoveDistance)
        {
            MovePosiNorm = (transform.localPosition - Origin).normalized;
            MovePosiNorm = new Vector3(MovePosiNorm.x, MovePosiNorm.y, 0);
        }
        else
            MovePosiNorm = Vector3.zero;
    }
    public float GetJoystackDir()
    {
        return  Mathf.Atan2(MovePosiNorm.x, MovePosiNorm.y) * Mathf.Rad2Deg  - 90;
    }
    
    void OnDrag(GameObject go, Vector2 delta)
    {
    
        if (!_drag)
        {
            _drag = true;
        }
        _deltaPos = delta;

        mTrans.localPosition += new Vector3(_deltaPos.x, _deltaPos.y, 0);
       // Debug.Log("-----------OnDrag------------ " + _drag);
    }

    void OnDragOut(GameObject go)
    {
       // Debug.Log("-----------OnDragOut------------");
        _drag = false;
        mTrans.localPosition = Origin;       
    }
    public bool IsDrag()
    {
        return _drag;
    }

    void OnMoveStart(GameObject go)
    {
       // if (PlayerMoveControl.moveStart != null) PlayerMoveControl.moveStart();
    }
}