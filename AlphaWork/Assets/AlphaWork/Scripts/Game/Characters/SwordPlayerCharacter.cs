using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace AlphaWork
{
    public class SwordPlayerCharacter : BaseCharacter
    {

        public Animator anim;
        public Rigidbody rbody;
        //public float speed = 1;
        public float rotationSpeed = 100;

        private float inputH;
        private float inputV;
        
        private Vector3 inputVec;
        private bool run;
        private JoystackCc m_Joystack;
        private bool bAttack = false;

        private string attstr = "";

        // Use this for initialization
        void Start()
        {
            anim = GetComponent<Animator>();
            rbody = GetComponent<Rigidbody>();
            inputVec = new Vector3();
            run = false;
            m_Joystack = FindObjectOfType<JoystackCc>();

            if (IsMainActor())
                rbody.useGravity = true;
            else
                rbody.useGravity = false;
        }
        

        // Update is called once per frame
        void Test()
        {
            if (Input.GetMouseButtonDown(0))
            {
                int n = Random.Range(0, 2);

                if (n == 0)
                {
                    anim.Play("Dame_01", -1, 0F);
                }
                else
                {
                    anim.Play("Dame_02", -1, 0F);
                }
            }
            if (Input.GetKeyDown("1"))
            {
                anim.Play("Attack_01", -1, 0F);
            }
            if (Input.GetKeyDown("2"))
            {
                anim.Play("Attack_02", -1, 0F);
            }
            if (Input.GetKeyDown("3"))
            {
                anim.Play("Attack_03", -1, 0F);
            }
            if (Input.GetKeyDown("4"))
            {
                anim.Play("Attack_04", -1, 0F);
            }

            if (Input.GetKeyDown("5"))
            {
                anim.Play("Attack_05", -1, 0F);
            }
            if (Input.GetKeyDown("6"))
            {
                anim.Play("Attack_06", -1, 0F);
            }
            if (Input.GetKeyDown("7"))
            {
                anim.Play("Attack_07", -1, 0F);
            }
            if (Input.GetKeyDown("8"))
            {
                anim.Play("Death_01", -1, 0F);
            }
            if (Input.GetKeyDown("9"))
            {
                anim.Play("Death_02", -1, 0F);
            }
            if (Input.GetKeyDown("0"))
            {
                anim.Play("Idle_nonWeapon", -1, 0F);
            }

            if (Input.GetKeyDown("g"))
            {
                anim.Play("Crouch", -1, 0F);
            }
            if (Input.GetKeyDown("t"))
            {
                anim.Play("Crouch_Move_F", -1, 0F);
            }
            if (Input.GetKeyDown("r"))
            {
                anim.Play("Crouch_Move_L", -1, 0F);
            }
            if (Input.GetKeyDown("y"))
            {
                anim.Play("Crouch_Move_R", -1, 0F);
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                run = true;
            }
            else
            {
                run = false;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                anim.SetBool("jump", true);
            }
            else
            {
                anim.SetBool("jump", false);
            }
        }

        private void FixedUpdate()
        {
            if (!IsActive() || !IsMainActor())
                return;

            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");           

            h += m_Joystack.MovePosiNorm.x;
            v += m_Joystack.MovePosiNorm.y;
            
            Move(h, v); 

            CleanInputs();
        }
        private void CleanInputs()
        {
            inputH = 0;
            inputV = 0;
        }

        private Vector3 CameraRelativeMovement(float inH,float inV)
        {
            //converts control input vectors into camera facing vectors
            Transform cameraTransform = Camera.main.transform;
            //Forward vector relative to the camera along the x-z plane   
            Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
            forward.y = 0;
            forward = forward.normalized;
            //Right vector relative to the camera always orthogonal to the forward vector
            Vector3 right = new Vector3(forward.z, 0, -forward.x);
            
            return inH * right + inV * forward;
        }

        protected void Move(float inH, float inV)
        {
            anim.SetBool("run", run);

            inputVec = CameraRelativeMovement(inH, inV);
            anim.SetFloat("inputV", inputVec.magnitude);//第三人称只需要向前动作

            if (inputVec.magnitude > 0.1)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVec), Time.deltaTime * rotationSpeed);
               //List<GestureRecognizer> recognizers = FingerGestures.RegisteredGestureRecognizers;
                //for(int i = 0; i < recognizers.Count; ++i)
                //{
                //    recognizers[i].UseSendMessage = false;
                //}
            }            

            inputH = inputVec.z;
            inputV = inputVec.x;

            float moveX;
            float moveZ;
           
            if (run)
            {
                moveX = inputV * runSpeed * baseSpeed * Time.deltaTime;
                moveZ = inputH * runSpeed * baseSpeed * Time.deltaTime;
            }
            else
            {
                moveX = inputV * walkSpeed * baseSpeed * Time.deltaTime;
                moveZ = inputH * walkSpeed * baseSpeed * Time.deltaTime;
            }

            rbody.velocity = new Vector3(moveX, 0f, moveZ);
        }

        protected override void OnAttack1(object sender, GameEventArgs arg)
        {
            if (!IsMainActor())
                return;

            if(anim == null)
                anim = GetComponent<Animator>();

            float rand = Random.Range(0.0f, 1.0f);
            if(rand < 0.5)
                anim.Play("Attack_01", -1, 0F);
            else
                anim.Play("Attack_02", -1, 0F);
        }
        protected override void OnAttack2(object sender, GameEventArgs arg)
        {
            if (!IsMainActor())
                return;

            if (anim == null)
                anim = GetComponent<Animator>();

            float rand = Random.Range(0.0f, 1.0f);
            if (rand < 0.5)
                anim.Play("Attack_03", -1, 0F);
            else
                anim.Play("Attack_04", -1, 0F);
        }
        protected override void OnKick1(object sender, GameEventArgs arg)
        {
            if (!IsMainActor())
                return;

            if (anim == null)
                anim = GetComponent<Animator>();

            anim.Play("Attack_05", -1, 0F);
        }
        protected override void OnKick2(object sender, GameEventArgs arg)
        {
            if (!IsMainActor())
                return;

            if (anim == null)
                anim = GetComponent<Animator>();

            anim.Play("Attack_06", -1, 0F);
        }

        #region
        //for npc
        public override void SyncStatus(int status)
        {
            base.SyncStatus(status);

            if (anim != null)
            {
                if (status == (int)LogicStatus.ELogic_TRACK)
                    anim.SetBool("run", true);
                else if (status == (int)LogicStatus.ELogic_PATROL)
                    anim.SetBool("run", false);
                else if (status == (int)LogicStatus.ELogic_IDLE)
                    anim.SetFloat("inputV", 0.0f);
                else if (status == (int)LogicStatus.ELogic_ATTACK)
                {
                    anim.SetBool("run", false);
                    anim.SetFloat("inputV", 0.0f);
                }
            }                
        }

        public override void ActionPatrol(float speed)
        {
            if (anim != null)
            {
                anim.SetFloat("inputV", 0.2f);
            }
        }

        public override void ActionAttack(float attackParam)
        {
            base.ActionAttack(attackParam);
            if (bAttack)
                return;

            int index = Random.Range(1, 8);
            attstr = "Attack_0" + index.ToString();
            anim.Play(attstr, -1, 0F);
        }
        //from animation event
        public void AttackStart()
        {
            bAttack = true;
            Debug.Log(string.Format("AttackStart:{0}",attstr));
        }
        //from animation event
        public void AttackEnd()
        {
            bAttack = false;
            Debug.Log(string.Format("AttackEnd:{0}", attstr));
        }
        #endregion

        public override void ActionHurt()
        {
            if (anim == null)
                anim = GetComponent<Animator>();

            int n = Random.Range(0, 2);
            if (n == 0)
            {
                anim.Play("Dame_01", -1, 0F);
            }
            else
            {
                anim.Play("Dame_02", -1, 0F);
            }
        }

        public override void ActionDead()
        {
            int n = Random.Range(0, 2);
            if (n == 0)
            {
                anim.Play("Death_01", -1, 0F);
            }
            else
            {
                anim.Play("Death_02", -1, 0F);
            }          
        }
    }

}



