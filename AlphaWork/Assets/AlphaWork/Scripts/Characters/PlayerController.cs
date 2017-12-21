using UnityEngine;
using System.Collections;

namespace AlphaWork
{
    public class PlayerController : MonoBehaviour
    {

        CharacterController m_CharacterController;
        public float m_Speed = 10.0f;
        public MovementStateInfo m_MovementStateInfo = new MovementStateInfo();
        private JoystackCc m_Joystack;
        public float gravity = -100.0f;
        private Transform m_CameraTrans;
        //private Animation m_Animation;
        public bool IsControl = false;
        // Use this for initialization
        public static PlayerController Instance;

        void Start()
        {
            Instance = this;
            m_CharacterController = GetComponent<CharacterController>();
            //m_Animation = GetComponentInChildren<Animation>();
            m_Joystack = FindObjectOfType<JoystackCc>();
            if (m_Joystack == null)
            {
                Debug.LogWarning("cann't fiond joystack");
            }

            //m_CameraTrans = FindObjectOfType<GameCamera>().transform;
        }

        private void CheckJoystickControl()
        {

            if (m_Joystack == null)
                return;
            float dir = m_Joystack.GetJoystackDir();

            if (m_Joystack.IsDrag())
            {
                m_MovementStateInfo.IsMoving = true;
                m_MovementStateInfo.SetMoveDir(dir);
                m_MovementStateInfo.TargetPosition = Vector3.zero;
            }
            else
            {
                m_MovementStateInfo.IsMoving = false;
                m_MovementStateInfo.SetMoveDir(dir);
                m_MovementStateInfo.TargetPosition = Vector3.zero;
            }
        }

        // Update is called once per frame
        void Update()
        {
            CheckJoystickControl();

            if (m_MovementStateInfo.IsMoving)
            {

                var forward = m_CameraTrans.TransformDirection(Vector3.forward);
                forward.y = 0;
                forward = forward.normalized;

                var right = new Vector3(forward.z, 0, -forward.x);

                var v = m_Joystack.MovePosiNorm.y;
                var h = m_Joystack.MovePosiNorm.x;

                var MoveDir = (h * right + v * forward).normalized;

                Vector3 Move = MoveDir * m_Speed * Time.deltaTime;
                Move.y = -gravity * Time.deltaTime;


                m_CharacterController.Move(Move);
                transform.rotation = Quaternion.LookRotation(MoveDir);
                
//                 if (!m_Animation.IsPlaying("run"))
//                     m_Animation.CrossFade("run");

                IsControl = true;
            }
            else
            {
//                 if (!m_Animation.IsPlaying("Idle"))
//                     m_Animation.CrossFade("Idle");

                IsControl = false;
            }
        }
    }

}
