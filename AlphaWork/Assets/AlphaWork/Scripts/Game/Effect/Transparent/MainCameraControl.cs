using UnityEngine;
using System.Collections;

public class MainCameraControl : MonoBehaviour {

    private static MainCameraControl m_Instance = null;


    public static MainCameraControl Instance()
    {
        return m_Instance;
    }
    //摄像机朝向的目标模型
    public Transform target;
    //摄像机与模型保持的距离
    public float distance = 10.0f;
    //射线机与模型保持的高度
    public float height = 5.0f;
    //高度阻尼
    public float heightDamping = 2.0f;
    //旋转阻尼
    public float rotationDamping = 3.0f;
    //主角对象
    private GameObject controller;

    public Camera mainCamera;
    //public WaterWaveEffect waterEffect;
   // private TweenFOV position;

    //相机跟随的目标，不一定是主角
    public Transform followTarget;

    //黑屏特效相关参数
    public Camera blackScreenCamera;
    public GameObject blackScreenMask;

    public float blackScreenSpeed = 0.0f;
    public float blackScreenCurrAlpha = 1.0f;
    public float blackScreenCurrTime = 0.0f;
    public bool blackScreenStarted = false;

    // 水波消失的时间
    public float waterEffectTime = 5f;

    // 目标坐标偏移
    public Vector3 CenterOffset = new Vector3(0, 1, 0);
    // 默认俯仰角
    public float DefaultPitch = 100.0f;
    // 默认偏转角  
    public float DefaultRaw = 0.0f;
    // 默认Camera距离目标距离
    public float DefaultDistance = 22.0f;

    // 最大偏航角
    public float MaxYaw = 90.0f;
    // 最小偏航角
    public float MinYaw = -90.0f;

    // 最大俯仰角
    public float MaxPitch = 50;
    // 最小俯视角   
    public float MinPitch = 20;

    // raw转向速度  
    public float xSpeed = 0f;
    // pitch转向速度
    public float ySpeed = 100.0f;
    // 缩放方向速度     
    public float zSpeed = 0f;

    // 摄像机距离目标当前距离  
    public float CurrentDistance = 15.0f;
    // 距离目标最近距离
    public float MinDistance = 8.0f;
    // 距离目标最远距离
    public float MaxDistance = 22.0f;

    //默认fov
    public float FOV = 28;

    private float m_fYaw = 0;
    private float m_fPitch = 0;
    private float m_minDistance = 0;

    private Vector2 m_vLastPosition1;
    private Vector2 m_vLastPosition2;
    private bool m_bLastFrameIsMultiTouch = false;


    //是否暂停镜头跟随
    public bool Pause = false;

    //是否是对话摄像机拉近状态
    public bool IsCameraZoomIn = false;

    public void SetCameraZoomin(bool bZoom)
    {
        IsCameraZoomIn = bZoom;
    }

    public bool IsPlayingCutscene = false;

    //是否摄像机跟随主角
    public bool isCameraFollow
    {
        get
        {
            if (Pause)
            {
                return false;
            }

            if (IsCameraZoomIn)
            {
                return false;
            }

            if (moveCameraStarted)
            {
                return false;
            }

            if (IsPlayingCutscene)
            {
                return false;
            }

            return true;
        }
    }

    //镜头移动相关变量
    private Vector3 startPosition;
    private Quaternion startQuat;
    private Vector3 endPosition;
    private Quaternion endQuat;
    private float moveCameraTime = 0.0f;
    private float moveCameraTotalTime = 0.0f;
    private float moveCameraCurrTime = 0.0f;
    public bool moveCameraStarted = false;


    //镜头抖动相关变量
    private float cameraShakeCurrTime;
    private float cameraShakeTotalTime;
    private Vector3 cameraShakeDirection;

    Transform mCameraTransform;
    public float SmoothFollowValue = 6;
    private Vector3 lastFollowPos;
    //private Vector3 initialPos;
    //private Tab_SceneInfo sceneInfo;

    //音源相关变量
    public static GameObject audioListenerObj;

    void Awake()
    {
        m_Instance = this;
    }

    void Start()
    {
        GameObject mainCameraObj = GameObject.Find("Main Camera");
        if (mainCameraObj)
        {
            mainCamera = mainCameraObj.GetComponent<Camera>();
            //waterEffect = mainCameraObj.GetComponent<WaterWaveEffect>();
        }

        if (mainCamera)
        {
            //mainCamera.cullingMask &= ~((1 << LayerMask.NameToLayer("DramaActor")) | (1 << LayerMask.NameToLayer("HideLayer")) | (1 << LayerMask.NameToLayer("NGUI")) | (1 << LayerMask.NameToLayer("UIBase"))
            //    | (1 << LayerMask.NameToLayer("UIPop")) | (1 << LayerMask.NameToLayer("UIStory")) | (1 << LayerMask.NameToLayer("UITip"))
            //    | (1 << LayerMask.NameToLayer("UIMenuPop")) | (1 << LayerMask.NameToLayer("UIMessage")) | (1 << LayerMask.NameToLayer("UIDeath")));
        }

        InitCamera();
        InitAudioListener();
    }

    public float minFov = 15f;
    public float maxFov = 90f;
    public float sensitivity = 10f;

    public Transform player = null;

    //推进
    //public void CarryForwardCamera()
    //{
    //    //得到主角对象
    //    if (player == null)
    //    {
    //        return;
    //    }
    //    controller = player.gameObject;
    //    if (player.targetNPC == null) return;
    //    target = player.targetNPC.transform;
    //    if (!target)
    //    {
    //        return;
    //    }
    //    MainCameraControl.Instance().SetCameraZoomin(true);
    //    GameObject mainCameraObj = GameObject.Find("Main Camera");
    //    if (mainCameraObj)
    //    {
    //        mainCamera = mainCameraObj.GetComponent<Camera>();
    //    }
    //    //这里是计算射线的方向，从主角发射方向是摄像机方向
    //  //  Vector3 aim = target.position;
    //    //得到方向
    //    Vector3 ve = (target.position - controller.transform.position).normalized;
    //   // float an = transform.eulerAngles.x;
    //    //aim -= an * ve;
    //    ////在场景视图中可以看到这条射线
    //    //Debug.DrawLine(target.position, aim, Color.red);
    //    ////主角朝着这个方向发射射线
    //    //RaycastHit hit;

    //    //if (Physics.Linecast(target.position, aim, out hit))
    //    //{
    //    //    string name = hit.collider.gameObject.tag;
    //    //    //name = "12132";
    //    //    if (name != "Main Camera" && name != "terrain" && hit.collider.gameObject.name == player.targetNPC.charID.ToString())
    //    //    {
    //    //        //当碰撞的不是摄像机也不是地形 那么直接移动摄像机的坐标
    //    //        //transform.position = hit.point;

    //    //    }
    //    //}

    //    // 让摄像机永远看着NPC
    //   // Tab_RoleModel roleModel = TableManager.GetRoleModelByID(player.targetNPC.objModelID, 0);
    //    //if (roleModel != null)
    //    //{
    //      //  Vector3 tempPosition = target.transform.position + target.transform.forward * roleModel.CameraDistance;
    //        //transform.position = new Vector3(tempPosition.x, tempPosition.y + roleModel.CameraHeightOffset, tempPosition.z);
    //    //}
    //    //else //设置默认值
    //    {
    //        Vector3 tempPosition = target.transform.position + target.transform.forward * 3;
    //        transform.position = new Vector3(tempPosition.x, tempPosition.y + 1.5f, tempPosition.z);
    //    }
    //    float mag = ve.magnitude;
    //    if (mag > 0.001f || mag == 0f)
    //    {
    //        Quaternion lookRot = GlobalFunction.OppositeQuaternion(target.rotation);
    //        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookRot, 2f);
    //    }
    //    //隐藏猪脚和部分UI
    //    controller.layer = (int)UIPathData.UIType.TYPE_HIDE;         
    //}

    public void BlackScreen(float toAlpha, float fadeTime)
    {
        //如果是播放剧情的情况下，不触发黑屏，可能导致相机mask不对
        if (IsPlayingCutscene)
        {
            return;
        }

        if (!EnableBlackScreenCamera())
        {
            return;
        }

        if (fadeTime <= 0.0f)
        {
            SetBlackScreenAlpha(toAlpha);
            BlackScreenFinish();
            return;
        }

        blackScreenSpeed = (toAlpha - blackScreenCurrAlpha) / fadeTime;
        blackScreenCurrTime = fadeTime;
        blackScreenStarted = true;
    }

    private bool EnableBlackScreenCamera()
    {
        if (mainCamera == null)
        {
            return false;
        }

        if (blackScreenCamera == null)
        {
            return false;
        }

        //mainCamera.cullingMask &= ~((1 << LayerMask.NameToLayer("BackLayer")) | (1 << LayerMask.NameToLayer("CityCha")) | (1 << LayerMask.NameToLayer("CollisionCha")));
        //blackScreenCamera.cullingMask = (1 << LayerMask.NameToLayer("BackLayer")) | (1 << LayerMask.NameToLayer("CityCha")) | (1 << LayerMask.NameToLayer("CollisionCha"));

        blackScreenCamera.enabled = true;
        return true;
    }

    private bool DisableBlackScreenCamera()
    {
        if (mainCamera == null)
        {
            return false;
        }

        if (blackScreenCamera == null)
        {
            return false;
        }

       // mainCamera.cullingMask |= (1 << LayerMask.NameToLayer("BackLayer")) | (1 << LayerMask.NameToLayer("CityCha")) | (1 << LayerMask.NameToLayer("CollisionCha"));
        blackScreenCamera.cullingMask = 0;

        blackScreenCamera.enabled = false;

        return true;
    }

    private void SetBlackScreenAlpha(float alpha)
    {
        alpha = Mathf.Clamp01(alpha);
        blackScreenCurrAlpha = alpha;

        if (blackScreenMask != null && blackScreenMask.GetComponent<Renderer>() != null && blackScreenMask.GetComponent<Renderer>().material != null)
        {
            if (blackScreenMask.GetComponent<Renderer>().material.HasProperty("_Transparent"))
            {
                blackScreenMask.GetComponent<Renderer>().material.SetFloat("_Transparent", alpha);
            }
        }
    }

    void UpdateBlackScreen(float delta)
    {
        if (!blackScreenStarted)
        {
            return;
        }

        if (blackScreenCurrTime <= 0.0f)
        {
            blackScreenStarted = false;

            BlackScreenFinish();

            return;
        }

        blackScreenCurrTime -= delta;
        SetBlackScreenAlpha(blackScreenCurrAlpha + blackScreenSpeed * delta);
    }

    void BlackScreenFinish()
    {
        if (blackScreenCurrAlpha <= 0.0f)
        {
            DisableBlackScreenCamera();
        }
    }

    public void ForceStopBlackScreen()
    {
        blackScreenCurrAlpha = 0.0f;
        blackScreenCurrTime = 0.0f;
        blackScreenStarted = false;
        DisableBlackScreenCamera();
    }

    public void ForceStopCameraShake()
    {
        cameraShakeCurrTime = 0.0f;
        cameraShakeTotalTime = 0.0f;
    }

    //public void SetWaterEffect(float waterTime)
    //{
    //    waterEffectTime = waterTime;
    //}

    //void UpdateWaterEffect(float delta)
    //{
    //    if (waterEffect == null || !waterEffect.isActiveAndEnabled)
    //    {
    //        return;
    //    }

    //    if (waterEffectTime <= 0.0f)
    //    {
    //        waterEffect.enabled = false;
    //        return;
    //    }

    //    waterEffectTime -= delta;
    //}

    public void ShakeCamera(Vector3 shakeDirection, float shakeTime)
    {
        //剧情时不震屏
        if (IsPlayingCutscene)
            return;

        cameraShakeDirection = shakeDirection;
        cameraShakeCurrTime = shakeTime;
        cameraShakeTotalTime = shakeTime;
    }

    public bool IsCameraShaking()
    {
        return cameraShakeCurrTime > 0.0f;
    }

    public void SetFollowTarget(Transform _target)
    {
        followTarget = _target;
    }

    void InitCamParam()
    {
        //sceneInfo = TableManager.GetSceneInfoByID(GameManager.gameManager.RunningScene, 0);
        //if (sceneInfo == null)
        //{
        //    return;
        //}
        ////是否为低端机
        //if (GameManager.gameManager.IsLow)
        //{
        //    DefaultPitch = sceneInfo.DefaultPitch;
        //    MaxPitch = sceneInfo.MaxPitch;
        //}
        //else
        //{
        //    DefaultPitch = sceneInfo.DefaultPitch;
        //    MaxPitch = sceneInfo.MaxPitch;
        //}

        //Pause = sceneInfo.Pause;
        //DefaultRaw = sceneInfo.DefaultRaw;
        //DefaultDistance = sceneInfo.DefaultDistance;
        //CurrentDistance = DefaultDistance;
        //xSpeed = sceneInfo.XSpeed;
        //ySpeed = sceneInfo.YSpeed;
        //zSpeed = sceneInfo.ZSpeed;
        //MaxYaw = sceneInfo.MaxRaw;
        //MinYaw = sceneInfo.MinRaw;
        //MinPitch = sceneInfo.MinPitch;
        //MinDistance = sceneInfo.MinDistance;
        //m_minDistance = MinDistance;
        //MaxDistance = sceneInfo.MaxDistance;

        //FOV = sceneInfo.FOV;
    }

    void InitAudioListener()
    {
        if (audioListenerObj != null)
        {
            return;
        }

        AudioListener listener = gameObject.GetComponent<AudioListener>();
        if (listener != null)
        {
            Destroy(listener);
        }

        audioListenerObj = new GameObject("ListenerObject");

        if (audioListenerObj == null)
        {
            return;
        }

        audioListenerObj.AddComponent<AudioListener>();
    }

    public void InitCamera()
    {
        InitCamParam();

        mainCamera = Camera.main;

        if (mainCamera)
        {
            mainCamera.transform.rotation = Quaternion.Euler(DefaultPitch, DefaultRaw, 0);
            mainCamera.fieldOfView = FOV;
            mainCamera.backgroundColor = UnityEngine.Color.black;
            mCameraTransform = mainCamera.transform;

            lastFollowPos = mCameraTransform.position;
            //initialPos = mCameraTransform.position;
            //Vector3 angles = mainCamera.transform.eulerAngles;
            m_fYaw = DefaultRaw;
            m_fPitch = DefaultPitch;

            if (GetComponent<Rigidbody>())
            {
                //让刚体不再旋转
                mainCamera.GetComponent<Rigidbody>().freezeRotation = true;
            }
        }

        //加载屏幕变暗遮罩
        PreLoadObjects();

        //初始化黑屏特效
        InitBlackScreenObjects();

        InvokeRepeating("UpdateOnMountCamera", 0f, 0.01f);
    }

    void PreLoadObjects()
    {
        //iTween.CameraFadeAdd();
    }

    void InitBlackScreenObjects()
    {
        if (mainCamera == null)
        {
            return;
        }

        GameObject blackScreenCameraObj = new GameObject("BlackScreenCamera");

        blackScreenCamera = blackScreenCameraObj.AddComponent<Camera>();

        if (blackScreenCamera == null)
        {
            DestroyImmediate(blackScreenCameraObj);
            return;
        }

        blackScreenCamera.transform.parent = mainCamera.transform;
        blackScreenCamera.transform.localPosition = Vector3.zero;
        blackScreenCamera.transform.localRotation = Quaternion.identity;
        blackScreenCamera.transform.localScale = Vector3.one;
        blackScreenCamera.farClipPlane = mainCamera.farClipPlane;
        blackScreenCamera.nearClipPlane = mainCamera.nearClipPlane;

        blackScreenCamera.fieldOfView = mainCamera.fieldOfView;

        blackScreenCamera.fieldOfView = mainCamera.fieldOfView;
        blackScreenCamera.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        blackScreenCamera.clearFlags = CameraClearFlags.Nothing;
        blackScreenCamera.cullingMask = 0;
        blackScreenCamera.depth = 0;

        //blackScreenMask = ResourceManager.InstantiateResource("Prefab/Effect/FlashPlane") as GameObject;

        if (blackScreenMask == null)
        {
            DestroyImmediate(blackScreenCameraObj);
            return;
        }

        blackScreenMask.transform.parent = blackScreenCamera.transform;
        blackScreenMask.transform.localPosition = Vector3.zero;
        blackScreenMask.transform.localRotation = Quaternion.identity;
        blackScreenMask.transform.localScale = Vector3.one;
        //blackScreenMask.layer = LayerMask.NameToLayer("BackLayer");

        blackScreenCamera.enabled = false;

        SetBlackScreenAlpha(0.0f);
    }

    void UpdateListener(float deltaTime)
    {
        if (audioListenerObj == null)
        {
            return;   
        }

        if (followTarget != null && !IsPlayingCutscene )
        {
            audioListenerObj.transform.rotation = transform.rotation;

            Vector3 toCamera = transform.position - followTarget.transform.position;

            audioListenerObj.transform.position = followTarget.position + (toCamera.normalized * 2.0f);
        }
        else
        {
            audioListenerObj.transform.position = transform.position;
        }

        //EngineEffectsInterface.SetNewAudioListener(audioListenerObj.transform);
    }

    void UpdateMoveCamera(float delta)
    {
        if (!moveCameraStarted)
        {
            return;
        }

        if (mainCamera == null)
        {
            return;
        }

        if (followTarget == null)
        {
            return;
        }

        if (moveCameraCurrTime <= moveCameraTotalTime)
        {
            float factor = moveCameraTime <= 0.0f ? 1.0f : (moveCameraCurrTime / moveCameraTime);
            Vector3 lerpPos = Vector3.Lerp(startPosition, endPosition, Mathf.Clamp01(factor));
            Quaternion lerpQuat = Quaternion.Slerp(startQuat, endQuat, Mathf.Clamp01(factor));

            mainCamera.transform.position = followTarget.position + lerpPos;
            mainCamera.transform.rotation = lerpQuat;

            moveCameraCurrTime += delta;
        }
        else
        {
            //结束时恢复摄像机
            m_fPitch = ClampAngle(m_fPitch, MinPitch, MaxPitch);
            m_fYaw = Mathf.Clamp(m_fYaw, MinYaw, MaxYaw);
            CurrentDistance = Mathf.Clamp(CurrentDistance, MinDistance, MaxDistance);

            Quaternion rotation = Quaternion.Euler(m_fPitch, m_fYaw, 0);

            Vector3 position = rotation * new Vector3(0, 0, -CurrentDistance) + followTarget.position + CenterOffset;

            mainCamera.transform.position = position;

            lastFollowPos = position;

            moveCameraCurrTime = 0.0f;
            moveCameraStarted = false;
        }
    }

    private bool IsOnMount = false;
    void UpdateCamera(float delta)
    {
        if (player == null)
        {
            return;
        }

        if (!mainCamera || IsCameraZoomIn)
        {
            //if (player.IsDie())
            //{
            //    IsCameraZoomIn = false;
            //}
            //return;
        }
        //if (CurrentDistance < MinDistance + 4f && player.mountManager.isOnMount && player.mountManager.isOnMount != IsOnMount)
        //{
        //    IsOnMount = true;
        //    m_minDistance += 4f;
        //}
        //if (player.mountManager != null && !player.mountManager.isOnMount && player.mountManager.isOnMount != IsOnMount)
        //{
        //    IsOnMount = false;
        //    m_minDistance = MinDistance;
        //}

        CameraCtl();

    }

    void UpdateOnMountCamera()
    {
        if (player == null)
        {
            return;
        }

        //if (player.mountManager != null && player.mountManager.isOnMount && isCameraFollow)
        //{
        //    if (CurrentDistance < m_minDistance && CurrentDistance <= MaxDistance)
        //    {
        //        CurrentDistance += zSpeed / 10f;
        //        m_fPitch += ySpeed * 0.01f / 10f;

        //    }
        //}
    }

    public bool MoveCamera(Vector3 startPos, Quaternion _startQuat, Vector3 endPos, Quaternion _endQuat, float _moveTime, float _totalTime)
    {
        if (IsCameraZoomIn)
        {
            return false;
        }

        startPosition = Quaternion.Euler(0.0f, m_fYaw, 0.0f) * startPos;
        startQuat = Quaternion.Euler(0.0f, m_fYaw, 0.0f) * _startQuat;

        endPosition = Quaternion.Euler(0.0f, m_fYaw, 0.0f) * endPos;
        endQuat = Quaternion.Euler(0.0f, m_fYaw, 0.0f) * _endQuat;

        moveCameraTime = _moveTime;
        moveCameraTotalTime = _totalTime;
        moveCameraCurrTime = 0.0f;

        if (moveCameraTime > moveCameraTotalTime)
        {
            moveCameraTime = moveCameraTotalTime;
        }

        moveCameraStarted = true;

        return true;
    }

   
    void CameraCtl()
    {
        ////客户端主角死亡后，不能再拖动视角
        //ClientCha mainCha = Singleton<ObjManager>.GetInstance().PlayerCha;
        //if (mainCha != null && mainCha.IsDie())
        //{
        //    return;
        //}

        if (!isCameraFollow)
        {
            return;
        }

       // if (UIManager.Instance() != null && UIManager.Instance().pop_Layer != null && UIManager.Instance().pop_Layer.ActiveUICount() > 0)
        {
        //    return;
		}
#if UNITY_EDITOR || UNITY_STANDALONE_WIN

        ////编辑器内鼠标模拟
        //Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (UICamera.mainCamera == null)
        //{
        //    return;
        //}
        //Ray ray = UICamera.mainCamera.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hitTouch;
        //Physics.Raycast(ray, out hitTouch);
        //if (hitTouch.collider != null && (hitTouch.collider.gameObject.CompareTag("UI") || hitTouch.collider.gameObject.CompareTag("SubUI") || hitTouch.collider.gameObject.layer == LayerMask.NameToLayer("NGUI")))
        //{
        //    return;
        //}

        ////         if (Input.GetMouseButton(0))
        ////         {
        ////             m_fRaw += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
        ////             m_fPitch -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
        ////         }

        //if (Input.GetAxis("Mouse ScrollWheel") > 0)
        //{
        //    if (CurrentDistance > m_minDistance)
        //    {
        //        m_bLastFrameIsMultiTouch = true;
        //        CurrentDistance -= zSpeed;
        //        m_fPitch -= ySpeed * 0.01f;
        //    }
        //}
        //else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        //{
        //    if (CurrentDistance < MaxDistance)
        //    {
        //        m_bLastFrameIsMultiTouch = true;
        //        CurrentDistance += zSpeed;
        //        m_fPitch += ySpeed * 0.01f;

        //    }
        //}
        //else
        //{
        //    m_bLastFrameIsMultiTouch = false;
        //}

        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    m_fPitch = DefaultPitch;
        //    m_fYaw = DefaultRaw;
        //    CurrentDistance = DefaultDistance;
        //}
#else

        //for (int i = 0; i < Input.touchCount; i++)
        //{
        //    Touch touch = Input.GetTouch(i);
        //    Vector3 vecTouchPos = touch.position;
        //    Ray rayTouch = UICamera.mainCamera.ScreenPointToRay(vecTouchPos);
        //    RaycastHit hitTouch;
        //    if (Physics.Raycast(rayTouch, out hitTouch))
        //    {
        //        if (hitTouch.collider != null && 
        //(hitTouch.collider.gameObject.tag == "UI"|| hitTouch.collider.gameObject.tag == "SubUI" || hitTouch.collider.gameObject.layer == LayerMask.NameToLayer("NGUI")))
        //        {
        //            return;
        //        }
        //    }
        //}
//         if (Input.GetMouseButton(0))
//         {
//             m_fRaw += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
//             m_fPitch -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
//         }
// 
//         if (Input.GetAxis("Mouse ScrollWheel") > 0)
//         { 
//             if (CurrentDistance > MinDistance)
//             {
//                 CurrentDistance -= zSpeed;
//                 m_fPitch -=  ySpeed * 0.01f;
//             }
//         }
//         else if (Input.GetAxis("Mouse ScrollWheel") < 0)
//         {
//             if (CurrentDistance < MaxDistance)
//             {
//                 CurrentDistance += zSpeed;
//             }
//         }



        //判断触摸数量为单点触摸   
        //if (Input.touchCount == 1 && !m_bLastFrameIsMultiTouch)
        //{
        //    //触摸类型为移动触摸   
        //    if (Input.GetTouch(0).phase == TouchPhase.Moved)
        //    {
        //        m_fRaw += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
        //        m_fPitch -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

        //    }
        //}
        //判断触摸数量为多点触摸   
        //else
        if (Input.touchCount > 1)
        {
            m_bLastFrameIsMultiTouch = true;

            //前两只手指触摸类型都为移动触摸   
            if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                var touch1 = Input.GetTouch(0);
                var touch2 = Input.GetTouch(1);

                var tempPosition1 = touch1.position;
                var tempPosition2 = touch2.position;

                float len1 = (m_vLastPosition1 - m_vLastPosition2).magnitude;
                float len2 = (tempPosition1 - tempPosition2).magnitude;
             
                if (len1 < len2)
                {
                    if (CurrentDistance > m_minDistance)
                    {
                        CurrentDistance -= zSpeed;
                        m_fPitch -= ySpeed * 0.01f;
                    }
                }
                else if(len1 > len2)
                {
                    if (CurrentDistance < MaxDistance)
                    {
                        CurrentDistance += zSpeed;
                        m_fPitch +=  ySpeed * 0.01f;
                    }
                }

                m_vLastPosition1 = tempPosition1;
                m_vLastPosition2 = tempPosition2;
                
            }
        }
        else
            m_bLastFrameIsMultiTouch = false;
#endif


    }
    //函数返回真为放大，返回假为缩小   
    bool isEnlarge(Vector2 oP1, Vector2 oP2, Vector2 nP1, Vector2 nP2)
    {
        float leng1 = Mathf.Sqrt((oP1.x - oP2.x) * (oP1.x - oP2.x) + (oP1.y - oP2.y) * (oP1.y - oP2.y));
        float leng2 = Mathf.Sqrt((nP1.x - nP2.x) * (nP1.x - nP2.x) + (nP1.y - nP2.y) * (nP1.y - nP2.y));

        if (leng1 < leng2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    //根据Update中的计算结果设置摄像机的位置和旋转
    void CameraLateUpdate(float delta)
    {
        
        if (isCameraFollow && followTarget != null)
        {
            m_fPitch = ClampAngle(m_fPitch, MinPitch, MaxPitch);
            m_fYaw = Mathf.Clamp(m_fYaw, MinYaw, MaxYaw);
            CurrentDistance = Mathf.Clamp(CurrentDistance, MinDistance, MaxDistance);

            Quaternion rotation = Quaternion.Euler(m_fPitch, m_fYaw, 0);
            mainCamera.transform.rotation = rotation;

            Vector3 position = rotation * new Vector3(0, 0, -CurrentDistance) + followTarget.position + CenterOffset;
            //临时代码，需根据场景与地面（mesh？）做射线相交的点确定最终position
            if (position.y < 0.5f)
            {
                position.y = 0.5f;
            }

            //推进镜头不进行相机跟随
            if (!m_bLastFrameIsMultiTouch)
            {
                Vector3 vCurrentPos = position;
                if (Vector3.Distance(vCurrentPos, mCameraTransform.position) > 20)
                {
                    mCameraTransform.position = vCurrentPos;
                }
                else
                {
                    mCameraTransform.position = Vector3.Lerp(lastFollowPos, vCurrentPos, SmoothFollowValue * delta);
                }
            }
            else
            {
                mainCamera.transform.position = position;
            }

            lastFollowPos = mCameraTransform.position;
        }

        ///在定屏的时候不进行摄像机抖动
        if (Time.timeScale > 0)
        {
            if (cameraShakeCurrTime > 0.0f && cameraShakeTotalTime > 0.0f)
            {
                float percent = cameraShakeCurrTime / cameraShakeTotalTime;

                Vector3 shakePos = Vector3.zero;
                shakePos.x = UnityEngine.Random.Range(-Mathf.Abs(cameraShakeDirection.x) * percent, Mathf.Abs(cameraShakeDirection.x) * percent);
                shakePos.y = UnityEngine.Random.Range(-Mathf.Abs(cameraShakeDirection.y) * percent, Mathf.Abs(cameraShakeDirection.y) * percent);
                shakePos.z = UnityEngine.Random.Range(-Mathf.Abs(cameraShakeDirection.z) * percent, Mathf.Abs(cameraShakeDirection.z) * percent);

                if (!IsCameraZoomIn)
                {
                    mCameraTransform.position += shakePos;
                }

                cameraShakeCurrTime -= Time.deltaTime;
            }
            else
            {
                cameraShakeCurrTime = 0.0f;
                cameraShakeTotalTime = 0.0f;
            }
        }
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        UpdateBlackScreen(deltaTime);
        UpdateListener(deltaTime);
        //UpdateWaterEffect(deltaTime);
    }

    void LateUpdate()
    {
        float deltaTime = Time.deltaTime;

        UpdateCamera(deltaTime);
        CameraLateUpdate(deltaTime);
        UpdateMoveCamera(deltaTime);
    }
}
