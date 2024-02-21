using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Roguelike
{
    public class Soul : BehavioursBase
    {

        private ICommand m_move;
        private ICommand m_atk;
        private ICommand m_rotate;
        private ICommand m_empty;

        private BehaviourTreeBaseNode m_root;
        private DataBase m_database = new DataBase();

        private Animator m_soulAnimator;

        public GameObject m_bullet;

        private NavMeshAgent m_navMeshAgent;

        /// <summary>
        /// ��Ϊ���ڵ㵱ǰ״̬
        /// </summary>
        ENodeState m_behaviourTreeCondition=ENodeState.Success;

        /// <summary>
        /// ����Ŀ��
        /// </summary>
        private Transform m_target;

        /// <summary>
        /// �����ͷż��
        /// </summary>
        public float m_castSpellInterval;

        private float m_castSpellTimer;
        private float m_attackTimer;

        //�Ƿ��ڹ���������ȴ��
        private bool m_attackTrigger = false;
        private bool m_castSpellTrigger = true;

        #region �������ں���
        private void OnEnable()
        {
            EventManager.AddEventListener<float, Object>("Wound", Wound);
        }

        private void Start()
        {
            m_move = CommandFactory.CreateCommand("Move");
            m_rotate = CommandFactory.CreateCommand("Aim");
            m_empty = CommandFactory.CreateCommand("Empty");
            m_atk = CommandFactory.CreateCommand("Attack");

            m_soulAnimator = this.GetComponent<Animator>();
            m_navMeshAgent = this.GetComponent<NavMeshAgent>();

            m_target = EventManager.TriggerEvent<Transform>("GetPlayerTransform");

        }

        private void Update()
        {
            CoolDownTimer(m_castSpellInterval, ref m_castSpellTimer, ref m_castSpellTrigger);
            CoolDownTimer(m_attackInverval, ref m_attackTimer, ref m_attackTrigger);

            if (m_root != null && m_behaviourTreeCondition == ENodeState.Success)
            {
                m_behaviourTreeCondition = m_root.Excute();
            }

        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener<float, Object>("Wound", Wound);
        }
        #endregion

        #region ��Ϊ���ڵ����
        /// <summary>
        /// ����Soul����Ϊ��
        /// </summary>
        /// <returns></returns>
        private BehaviourTreeBaseNode SetUpTree()
        {
            // ������Ϊ���ڵ�
            BehaviourTreeBaseNode root = new BehaviourTreeSelectNodeOneFrameOneChild();

            // ����ӽڵ�
            root.AddChild(
                //new BehaviourTreeConditionNode(CheckIfEnemyInRange),
                new BehaviourTreeActionNode(NormalAttack),
                new BehaviourTreeActionNode(SkillAttack),
                new BehaviourTreeActionNode(ChangePosition)
            );

            return root;
        }

        /// <summary>
        /// �жϵ����Ƿ��ڹ�����Χ�ڵĽڵ�
        /// </summary>
        /// <returns></returns>
        private bool CheckIfEnemyInRange()
        {
            // ������ʵ�־���������ж��߼�
            // ����е����ڹ�����Χ�ڣ����� true�����򷵻� false
            Debug.Log("��");
            return true;
        }

        /// <summary>
        /// ��ͨ�������˵Ľڵ�
        /// </summary>
        /// <returns></returns>
        private ENodeState NormalAttack()
        {
            if (m_target == null || m_attackTrigger == true)
            {
                return ENodeState.Failed;
            }

            // ������ʵ�־���Ĺ����߼�
            // ��������ɹ������� ENodeState.Success
            // �������ʧ�ܣ����� ENodeState.Failed
            // ����������ڽ����У����� ENodeState.Running
            m_atk.Execute(this);
            Debug.Log("����");

            return ENodeState.Success;
        }

        /// <summary>
        /// ���ܹ������˵Ľڵ�
        /// </summary>
        /// <returns></returns>
        private ENodeState SkillAttack()
        {
            if (m_target == null || m_castSpellTrigger == true)
            {
                return ENodeState.Failed;
            }

            CastSpell();
            Debug.Log("����");
            return ENodeState.Success;
        }

        /// <summary>
        /// �ƶ��ڵ�
        /// </summary>
        /// <returns></returns>
        private ENodeState ChangePosition()
        {
            Vector3 destination = GetDestination();
            m_navMeshAgent.SetDestination(destination);
            m_move.Execute(this);
            if (this.transform.position == destination)
            {
                return ENodeState.Success;
            }

            return ENodeState.Running;
        }
        #endregion

        /// <summary>
        /// ��ʼ��Soul
        /// </summary>
        private void Init()
        {
            AddAnimatorClipEvent();
            m_root = SetUpTree();
        }

        /// <summary>
        /// Soul�������
        /// </summary>
        public override void Attack()
        {
            FaceDirection();
            m_soulAnimator.Play("Projectile Attack");
        }

        /// <summary>
        /// Soul�ƶ�
        /// </summary>
        public override void Move()
        {
            m_soulAnimator.Play("Fly Forward In Place");
        }

        /// <summary>
        /// Soul�ͷż���
        /// </summary>
        public void CastSpell()
        {
            m_soulAnimator.Play("Cast Spell");

            Vector3 target = new Vector3(Random.Range(m_target.position.x-0.3f,m_target.position.x+0.3f),this.transform.position.y,
                Random.Range(m_target.position.z-0.3f,m_target.position.z+0.3f));
            this.transform.position = target;

            FaceDirection();
            m_soulAnimator.Play("Bite Attack");
        }

        /// <summary>
        /// Soul�ܻ�
        /// </summary>
        /// <param name="hit"></param>
        /// <param name="woundCharacter"></param>
        public override void Wound(float hit, Object woundCharacter)
        {
            m_currentHp -= hit;
            if (m_currentHp == 0)
            {
                Dead();
            }
            m_soulAnimator.Play("Take Damage");
        }

        /// <summary>
        /// Soul����
        /// </summary>
        public override void Dead()
        {
            m_soulAnimator.Play("Die");
            GameObjectPoolManager.ReturnObjectToPool(this.gameObject);
        }

        /// <summary>
        /// Soul����
        /// </summary>
        public override void FaceDirection()
        {
            Vector3 target = new Vector3(m_target.position.x, this.transform.position.y, m_target.position.z);
            this.transform.LookAt(target);
        }

        /// <summary>
        /// �����������¼�
        /// </summary>
        private void AddAnimatorClipEvent()
        {
            AnimationClip[] clips = m_soulAnimator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips)
            {
                //�ҵ���Ӧ�Ķ���������
                if (clip.name.Equals("Cast Spell"))
                {
                    //�������¼�
                    AnimationEvent m_castSpellEvent = new AnimationEvent();
                    //��Ӧ�¼�������Ӧ����������
                    m_castSpellEvent.functionName = "OnCastSpellEvent";
                    //�趨��Ӧ�¼�����Ӧ�����ϵĴ���ʱ���
                    m_castSpellEvent.time = clip.length * 0.9f;
                    clip.AddEvent(m_castSpellEvent);
                }
                else if(clip.name.Equals("Projectile Attack"))
                {
                    AnimationEvent m_attackEvent = new AnimationEvent();
                    m_attackEvent.functionName = "OnAttackEvent";
                    m_attackEvent.time = clip.length * 0.4f;
                    clip.AddEvent(m_attackEvent);
                }
                else if(clip.name.Equals("Bite Attack"))
                {
                    AnimationEvent m_attackEvent = new AnimationEvent();
                    m_attackEvent.functionName = "OnBiteAttackEvent";
                    m_attackEvent.time = clip.length * 0.2f;
                    clip.AddEvent(m_attackEvent);
                }
            }
            //���°󶨶����������ж���������
            m_soulAnimator.Rebind();
        }

        /// <summary>
        /// �ͷż��ܶ��������¼�
        /// </summary>
        private void OnCastSpellEvent()
        {
            Material soulMaterial = this.transform.GetComponentInChildren<Renderer>().material;
            Debug.Log("����"+ (soulMaterial == null));
            Color soulMaterialColor = soulMaterial.color;
            float alpha = soulMaterialColor.a;
            while (alpha > 0)
            {
                alpha -= 5;
                if (alpha < 0)
                {
                    alpha = 0;
                }
                soulMaterialColor.a = alpha;
                soulMaterial.color = soulMaterialColor;
                Debug.Log(alpha);
            }

        }

        /// <summary>
        /// ���ܹ������������¼�
        /// </summary>
        private void OnBiteAttackEvent()
        {
            Material soulMaterial = this.transform.GetComponentInChildren<Renderer>().material;
            Color soulMaterialColor = soulMaterial.color;
            float alpha = soulMaterialColor.a;
            while (alpha > 0)
            {
                alpha += 5;
                if (alpha > 255)
                {
                    alpha = 255;
                }
                soulMaterialColor.a = alpha;
                soulMaterial.color = soulMaterialColor;
            }
        }

        /// <summary>
        /// ��ͨ�������������¼�
        /// </summary>
        private void OnAttackEvent()
        {
            if (m_attackTrigger == false)
            {
                m_attackTrigger = true;
                GameObject bullet = GameObjectPoolManager.SpawnObject(m_bullet, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity, GameObjectPoolManager.EPoolType.GameObject,
                    0.3f, 0.3f, 0.3f);
                Debug.Log("�������" + m_target.position);
                EventManager.TriggerEvent<Vector3, float, Object>("ShootBullet", m_target.position, 1f, bullet);

            }
        }

        /// <summary>
        /// Soulǰҧ����
        /// </summary>
        private void BiteAttack()
        {
            float radius = this.GetComponent<CapsuleCollider>().radius;

            float step = 0.1f;
            float originX = this.transform.position.x - radius;

            while (originX < this.transform.position.x + radius)
            {
                Vector3 origin = new Vector3(originX, this.transform.position.y, this.transform.position.z);
                Vector3 direction = this.transform.forward;

                RaycastHit hitInfo;
                if (Physics.Raycast(origin, direction, out hitInfo, radius))
                {
                    GameObject hitObject = hitInfo.collider.gameObject;
                    switch (hitObject.tag)
                    {
                        case "Player":
                            Debug.Log("���߻��������壺" + hitObject.name);
                            EventManager.TriggerEvent<float, Object>("Wound", m_attack,hitInfo.transform.gameObject);
                            break;
                        default:
                            break;
                    }
                }
                originX += step;
            }
        }

        /// <summary>
        /// ����������ȴ
        /// </summary>
        /// <returns></returns>
        private void CoolDownTimer(float interval,ref float timer, ref bool trigger)
        {
            if (trigger == false)
            {
                return;
            }
            else
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        trigger = false;
                        timer = interval;
                    }
                }
            }
        }

        /// <summary>
        /// Soul�ƶ���Ŀ�ĵ�
        /// </summary>
        private Vector3 GetDestination()
        {
            Vector3 destination = Vector3.zero;
            RaycastHit roomPoint;

            if(Physics.Raycast(this.transform.position,Vector3.down,out roomPoint))
            {
                //�ڲ���������ķ�Χ�ڡ���һ����Χ�ڽ����ƶ�
                float x = Random.Range(Mathf.Max(this.transform.position.x - 4.5f, roomPoint.transform.position.x - 7f),
                    Mathf.Min(this.transform.position.x + 4.5f, roomPoint.transform.position.x + 7f));
                float z = Random.Range(Mathf.Max(this.transform.position.z - 4.5f, roomPoint.transform.position.z - 7f),
                    Mathf.Min(this.transform.position.z + 4.5f, roomPoint.transform.position.z + 7f));
                destination = new Vector3(x, this.transform.position.y, z);
            }
            return destination;
        }
    }
}

