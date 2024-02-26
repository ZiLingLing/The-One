using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    /// <summary>
    /// 挂载在UI预制体脚本上的抽象类
    /// </summary>
    public abstract class View : MonoBehaviour
    {
        /// <summary>
        /// //定义一个抽象方法，每个UI界面继承该脚本都必须实现Init方法
        /// </summary>
        public abstract void Init();

        /// <summary>
        /// 设置当前物体隐藏
        /// </summary>
        public virtual void Hide()
        {
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// 设置当前物体显示
        /// </summary>
        public virtual void Show()
        {
             this.gameObject.SetActive(true);
        }
    }
}
