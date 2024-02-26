using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Roguelike
{
    public class PlayerBehaviour : BehavioursBase
    {
        private ICommand m_move;
        private ICommand m_atk;
        private ICommand m_wound;
        private ICommand m_rotate;
        private ICommand m_empty;

        private Input m_input;

        /// <summary>
        /// 是否处于攻击间隔中
        /// </summary>
        private bool m_attackTrigger = false;

        /// <summary>
        /// 是否已经输入攻击的命令
        /// </summary>
        private float m_judgeAttack;

        /// <summary>
        /// 角色移动方向
        /// </summary>
        private Vector3 m_moveDirection;

        /// <summary>
        /// 角色朝向
        /// </summary>
        private Vector3 m_faceDirection;
        private Vector3 localFaceDirection;

        private bool isPause = false;

        private CharacterController m_characterController;
        private Animator m_animator;
        private Collider m_collider;

        public GameObject m_bullet;

        #region 生命周期函数
        private void Awake()
        {
            m_input = new Input();
            m_input.KeyboardAndMouse.Movement.performed += Movement_performed;
            m_input.KeyboardAndMouse.Attack.performed += Attack_performed;
        }

        private void OnEnable()
        {
            m_input.KeyboardAndMouse.Enable();
            EventManager.AddEventListener<float, Object>("Wound", Wound);
            EventManager.AddEventListener("Pause", Pause);
            EventManager.AddEventListener("Continue", Continue);
            StartCoroutine(AttackIntervalCoroutine(m_attackInverval));
        }

        private void Start()
        {
            m_move = CommandFactory.CreateCommand("Move");
            m_rotate = CommandFactory.CreateCommand("Aim");
            m_empty = CommandFactory.CreateCommand("Empty");
            m_atk = CommandFactory.CreateCommand("Attack");

            m_characterController = this.GetComponent<CharacterController>();
            m_collider = this.GetComponent<CapsuleCollider>();
            m_animator = this.GetComponent<Animator>();

        }

        private void Update()
        {
            if (isPause == false)
            {
                m_rotate.Execute(this);
                m_move.Execute(this);
                GetSingleChoiceCommand().Execute(this);
            }
        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener<float, Object>("Wound", Wound);
            EventManager.RemoveEventListener("Pause", Pause);
            EventManager.RemoveEventListener("Continue", Continue);
            m_input.KeyboardAndMouse.Disable();
            StopCoroutine(AttackIntervalCoroutine(m_attackInverval));

        }

        private void OnDestroy()
        {
            m_input.Disable();
        }
        #endregion

        #region 操纵角色行为相关方法

        #region InputSystem相关
        private void Attack_performed(InputAction.CallbackContext obj)
        {
            if (isPause == false)
            {
                m_judgeAttack = obj.ReadValue<float>();
            }
        }

        private void Movement_performed(InputAction.CallbackContext obj)
        {
            //m_atk.Execute(this);
            m_moveDirection = obj.ReadValue<Vector3>();

        }
        #endregion

        private void Pause()
        {
            isPause = true;
        }

        private void Continue()
        {
            isPause = false;
        }

        /// <summary>
        /// 角色面朝鼠标位置
        /// </summary>
        public override void FaceDirection()
        {
            // 获取鼠标位置
            Vector3 mousePosition = Mouse.current.position.ReadValue();

            // 将屏幕坐标转换为世界坐标
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 计算角色应该朝向的方向
                //Vector3 lookDirection = hit.point - this.gameObject.transform.position;
                //lookDirection.y = 0f; // 确保只在水平方向上旋转
                //Debug.Log(hit.transform.gameObject.name);
                m_faceDirection = (hit.point - this.gameObject.transform.position).normalized;
                m_faceDirection.y = 0f;
                //Debug.Log("角色朝向"+m_faceDirection);
                // 将角色朝向鼠标位置
                this.gameObject.transform.rotation = Quaternion.LookRotation(m_faceDirection);
            }
        }

        /// <summary>
        /// 玩家操控的角色移动
        /// </summary>
        public override void Move()
        {
            //float m_speed = 3f;
            //Vector3 moveDirection = new Vector3(m_faceDirection.x, 0f, m_faceDirection.z);
            Vector3 direction = this.transform.InverseTransformVector(m_moveDirection);
            m_animator.SetFloat("Vertical Speed", direction.z * m_speed, 0.1f, Time.deltaTime);
            m_animator.SetFloat("Horizontal Speed", direction.x * m_speed, 0.1f, Time.deltaTime);
        }

        /// <summary>
        /// 角色攻击
        /// </summary>
        public override void Attack()
        {
            if(m_attackTrigger==false)
            {
                m_attackTrigger = true;
                GameObject bullet = GameObjectPoolManager.SpawnObject(m_bullet, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity, GameObjectPoolManager.EPoolType.GameObject,
                    0.3f, 0.3f, 0.3f);
                Debug.Log("射击方向" + m_faceDirection);
                EventManager.TriggerEvent<Vector3, float, Object>("ShootBullet", m_faceDirection, 1f, bullet);

            }
        }

        /// <summary>
        /// 角色受伤
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="woundCharacter"></param>
        public override void Wound(float hit, Object info)
        {
            if(info==this)
            {
                base.Wound(hit,this);
                EventManager.TriggerEvent("WoundShakeCamera");
            }

        }

        /// <summary>
        /// 角色交互行为
        /// </summary>
        public void InterActive()
        {
            
        }


        /// <summary>
        /// 返回一个将要被加入输入列表的操作，一个一帧内互斥的操作
        /// </summary>
        /// <returns></returns>
        public ICommand GetSingleChoiceCommand()
        {
            ICommand command = m_empty;
            if(m_judgeAttack==1)
            {
                command = m_atk;
            }
            return command;
        }

        #endregion

        #region 协程
        /// <summary>
        /// 判断开火间隔协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator AttackIntervalCoroutine(float fireInterval)
        {
            while(true)
            {
                if (m_attackTrigger == true)
                {
                    yield return new WaitForSeconds(fireInterval);
                    m_attackTrigger = false;
                }
                yield return null;
            }
        }
        #endregion
    }
}
