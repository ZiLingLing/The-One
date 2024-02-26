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
        /// �Ƿ��ڹ��������
        /// </summary>
        private bool m_attackTrigger = false;

        /// <summary>
        /// �Ƿ��Ѿ����빥��������
        /// </summary>
        private float m_judgeAttack;

        /// <summary>
        /// ��ɫ�ƶ�����
        /// </summary>
        private Vector3 m_moveDirection;

        /// <summary>
        /// ��ɫ����
        /// </summary>
        private Vector3 m_faceDirection;
        private Vector3 localFaceDirection;

        private bool isPause = false;

        private CharacterController m_characterController;
        private Animator m_animator;
        private Collider m_collider;

        public GameObject m_bullet;

        #region �������ں���
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

        #region ���ݽ�ɫ��Ϊ��ط���

        #region InputSystem���
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
        /// ��ɫ�泯���λ��
        /// </summary>
        public override void FaceDirection()
        {
            // ��ȡ���λ��
            Vector3 mousePosition = Mouse.current.position.ReadValue();

            // ����Ļ����ת��Ϊ��������
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // �����ɫӦ�ó���ķ���
                //Vector3 lookDirection = hit.point - this.gameObject.transform.position;
                //lookDirection.y = 0f; // ȷ��ֻ��ˮƽ��������ת
                //Debug.Log(hit.transform.gameObject.name);
                m_faceDirection = (hit.point - this.gameObject.transform.position).normalized;
                m_faceDirection.y = 0f;
                //Debug.Log("��ɫ����"+m_faceDirection);
                // ����ɫ�������λ��
                this.gameObject.transform.rotation = Quaternion.LookRotation(m_faceDirection);
            }
        }

        /// <summary>
        /// ��ҲٿصĽ�ɫ�ƶ�
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
        /// ��ɫ����
        /// </summary>
        public override void Attack()
        {
            if(m_attackTrigger==false)
            {
                m_attackTrigger = true;
                GameObject bullet = GameObjectPoolManager.SpawnObject(m_bullet, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity, GameObjectPoolManager.EPoolType.GameObject,
                    0.3f, 0.3f, 0.3f);
                Debug.Log("�������" + m_faceDirection);
                EventManager.TriggerEvent<Vector3, float, Object>("ShootBullet", m_faceDirection, 1f, bullet);

            }
        }

        /// <summary>
        /// ��ɫ����
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
        /// ��ɫ������Ϊ
        /// </summary>
        public void InterActive()
        {
            
        }


        /// <summary>
        /// ����һ����Ҫ�����������б�Ĳ�����һ��һ֡�ڻ���Ĳ���
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

        #region Э��
        /// <summary>
        /// �жϿ�����Э��
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
