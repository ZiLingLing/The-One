using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Roguelike
{


    /// <summary>
    /// 规定接收行为的接口
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="gamePlyaer"></param>
        public void Execute(BehavioursBase gamePlyaer);
        //public void Undo();
    }

    /// <summary>
    /// 无效按键行为
    /// </summary>
    public class EmptyCommand : ICommand
    {
        private BehavioursBase m_gamePlayer;
        public void Execute(BehavioursBase gamePlayer)
        {
            
        }
    }

    /// <summary>
    /// 移动行为
    /// </summary>
    public class MoveCommand : ICommand
    {
        private BehavioursBase m_actor;

        public void Execute(BehavioursBase actor)
        {
            actor.Move();

            Vector3 position = actor.gameObject.transform.position;
        }
    }

    /// <summary>
    /// 角色旋转朝向鼠标位置
    /// </summary>
    public class RotateTowardMouseCommand : ICommand
    {
        private BehavioursBase m_actor;

        public void Execute(BehavioursBase actor)
        {
            actor.FaceDirection();

            Quaternion position = actor.gameObject.transform.rotation;
        }
    }

    /// <summary>
    /// 角色的攻击行为
    /// </summary>
    public class AttackCommand : ICommand
    {
        private BehavioursBase m_actor;

        public void Execute(BehavioursBase actor)
        {
            actor.Attack();

        }
    }
}
