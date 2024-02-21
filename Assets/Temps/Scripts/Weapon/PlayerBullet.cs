using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    public class PlayerBullet : BulletBase
    {
        /// <summary>
        /// �ӵ���������ʱ��
        /// </summary>
        public float m_bulletLifeTime;

        /// <summary>
        /// �����ٶ�
        /// </summary>
        public float m_dashSpeed=2;

        /// <summary>
        /// ���з���
        /// </summary>
        public Vector3 m_dashDirection;

        /// <summary>
        /// ���е��ٶȱ���
        /// </summary>
        public float m_speedImpactFactor=1;

        #region �������ں���

        private void Awake()
        {

        }

        private void OnEnable()
        {
            EventManager.AddEventListener<Vector3, float, Object>("ShootBullet", GetBulletProperty);
            StartCoroutine(TimeSpanCoroutine());
        }

        private void Start()
        {

        }

        private void Update()
        {
            Dash();
            RayDetect();
        }

        private void OnDisable()
        {
            
            EventManager.RemoveEventListener<Vector3,float,Object>("ShootBullet", GetBulletProperty);
            StopCoroutine(TimeSpanCoroutine());

        }
        #endregion

        #region �ӵ���ص��߼�
        /// <summary>
        /// �ӵ�����
        /// </summary>
        private void Dash()
        {
            this.transform.position += m_dashDirection.normalized * m_dashSpeed * m_speedImpactFactor * Time.deltaTime;
        }

        /// <summary>
        /// ��ȡ�ӵ��ķ����ٶȺͷ��з���
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="speedImpactFactor"></param>
        private void GetBulletProperty(Vector3 direction, float speedImpactFactor, Object info)
        {
            if ((info as GameObject).GetComponent<PlayerBullet>() == this)
            {
                m_speedImpactFactor = speedImpactFactor;
                m_dashDirection = direction;

            }

        }

        /// <summary>
        /// ��һ��������ӵ��ݻ�
        /// </summary>
        private void BulletDestory()
        {
            GameObjectPoolManager.ReturnObjectToPool(this.gameObject);
        }

        /// <summary>
        /// ���߼���ӵ��Ƿ�����
        /// </summary>
        private void RayDetect()
        {
            float rayLength = this.GetComponent<SphereCollider>().radius;
            int rayCount = 12;
            float angleStep = 15f;

            Vector3 origin = transform.position;

            for (int i = 0; i < rayCount; i++)
            {
                float angle = i * angleStep;
                Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
                Vector3 direction = rotation * transform.forward;

                RaycastHit hitInfo;
                if (Physics.Raycast(origin, direction, out hitInfo, rayLength))
                {
                    GameObject hitObject = hitInfo.collider.gameObject;
                    switch (hitObject.tag)
                    {
                        case "Enemy":
                            BulletDestory();
                            Debug.Log("���߻��������壺" + hitObject.name);break;
                        case "Wall":
                            BulletDestory();break;
                        default:
                            break;
                    }
                    //EventManager.TriggerEvent<>

                }
            }
        }
        #endregion

        #region Э��
        /// <summary>
        /// �����ӵ��������ڵ�Э��
        /// </summary>
        /// <returns></returns>
        private IEnumerator TimeSpanCoroutine()
        {
            yield return new WaitForSeconds(m_bulletLifeTime);
            this.BulletDestory();
        }

        #endregion
    }
}

