using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    /// <summary>
    /// 规范所有输入行为
    /// </summary>
    public interface IAction
    {

    }

    /// <summary>
    /// 对应答输入的抽象
    /// </summary>
    public interface IAbstractActions
    {
        public abstract void Move();

        public abstract void Attack();

        public abstract void Wound(float hit,Object woundCharacter);

        public abstract void Dead();

        public abstract void FaceDirection();
    }
}

