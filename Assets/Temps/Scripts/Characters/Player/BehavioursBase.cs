using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    /// <summary>
    /// 角色行为的基类
    /// </summary>
    public class BehavioursBase : MonoBehaviour,IAbstractActions
    {
        /// <summary>
        /// 角色移速
        /// </summary>
        public float m_speed;

        /// <summary>
        /// 角色当前血量
        /// </summary>
        public float m_currentHp;

        /// <summary>
        /// 角色攻击间隔
        /// </summary>
        public float m_attackInverval;

        /// <summary>
        /// 角色攻击力
        /// </summary>
        public float m_attack;

        /// <summary>
        /// 角色血量上限
        /// </summary>
        public float m_hpUpperLimit;

        /// <summary>
        /// 攻击
        /// </summary>
        public virtual void Attack()
        {

        }

        /// <summary>
        /// 移动
        /// </summary>
        public virtual void Move()
        {
            
        }

        /// <summary>
        /// 受伤
        /// </summary>
        public virtual void Wound(float hit,Object woundCharacter)
        {
            m_currentHp -= hit;
            if(m_currentHp==0)
            {
                Dead();
            }
        }

        /// <summary>
        /// 死亡
        /// </summary>
        public virtual void Dead()
        {
            Destroy(this);
        }

        /// <summary>
        /// 朝向
        /// </summary>
        public virtual void FaceDirection()
        {
            
        }
    }

}
