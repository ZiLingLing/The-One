using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    public class PlayerBullet : BulletBase
    {
        /// <summary>
        /// 子弹的最大存在时间
        /// </summary>
        public float m_bulletLifeTime;

        /// <summary>
        /// 飞行速度
        /// </summary>
        public float m_dashSpeed=2;

        /// <summary>
        /// 飞行方向
        /// </summary>
        public Vector3 m_dashDirection;

        /// <summary>
        /// 飞行的速度倍率
        /// </summary>
        public float m_speedImpactFactor=1;

        #region 生命周期函数

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

        #region 子弹相关的逻辑
        /// <summary>
        /// 子弹飞行
        /// </summary>
        private void Dash()
        {
            this.transform.position += m_dashDirection.normalized * m_dashSpeed * m_speedImpactFactor * Time.deltaTime;
        }

        /// <summary>
        /// 获取子弹的飞行速度和飞行方向
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
        /// 玩家基础类型子弹摧毁
        /// </summary>
        private void BulletDestory()
        {
            GameObjectPoolManager.ReturnObjectToPool(this.gameObject);
        }

        /// <summary>
        /// 射线检测子弹是否命中
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
                            Debug.Log("射线击中了物体：" + hitObject.name);break;
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

        #region 协程
        /// <summary>
        /// 用于子弹生命周期的协程
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

