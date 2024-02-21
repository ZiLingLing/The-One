using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Roguelike
{
    /// <summary>
    /// 行为树类
    /// </summary>
    public abstract class BehaviourTree : MonoBehaviour
    {
        protected BehaviourTreeBaseNode m_root;

        protected DataBase m_database = new DataBase();//为每个树声明一个黑板节点

        public virtual void Start()
        {
            m_root = SetUpTree();
        }

        private void Update()
        {
            if (m_root != null)
            {
                m_root.Excute();
            }
        }

        public abstract BehaviourTreeBaseNode SetUpTree();

    }

    /// <summary>
    /// 行为树节点的执行结果
    /// </summary>
    public enum ENodeState
    {
        Success,
        Failed,
        Running
    }

    /// <summary>
    /// 行为节点基类
    /// </summary>
    public abstract class BehaviourTreeBaseNode
    {
        /// <summary>
        /// 行为树子节点列表
        /// </summary>
        protected List<BehaviourTreeBaseNode> m_childList = new List<BehaviourTreeBaseNode>();

        public BehaviourTreeBaseNode m_parent;

        /// <summary>
        /// 执行当前逻辑的子节点序号
        /// </summary>
        protected int m_nodeIndex;

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="nodes"></param>
        public virtual void AddChild(params BehaviourTreeBaseNode[] nodes)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].m_parent = this;
                m_childList.Add(nodes[i]);
            }
        }

        /// <summary>
        /// 移除子节点
        /// </summary>
        /// <param name="index"></param>
        public virtual void RemoveChild(int index)
        {
            m_childList.Remove(m_childList[index]);
        }

        /// <summary>
        /// 执行节点逻辑
        /// </summary>
        /// <returns></returns>
        public abstract ENodeState Excute();
    }

    /// <summary>
    /// 行为节点:
    /// 行为节点通常作为最后的叶子结点，它在完成具体的一次行为之后根据计算或配置返回返回值。
    /// 行为节点可以是执行一次得到结果(返回失败或成功)，也可以分步执行很多次。
    /// </summary>
    public class BehaviourTreeActionNode : BehaviourTreeBaseNode
    {
        public Func<ENodeState> m_action;

        private ENodeState m_nodeState;

        public ENodeState NodeState
        {
            get
            {
                return m_nodeState;
            }
        }

        public BehaviourTreeActionNode(Func<ENodeState> action)
        {
            this.m_action = action;
        }

        public override ENodeState Excute()
        {
            if (m_action == null)
            {
                m_nodeState = ENodeState.Failed;
                return m_nodeState;
            }

            switch (m_action.Invoke())
            {
                case ENodeState.Failed:
                    m_nodeState = ENodeState.Failed;
                    return ENodeState.Failed;
                case ENodeState.Running:
                    m_nodeState = ENodeState.Running;
                    return ENodeState.Running;
            }
            m_nodeState = ENodeState.Success;
            return m_nodeState;
        }
    }

    /// <summary>
    /// 条件节点：
    /// 若条件满足返回True，否则返回Fasle。
    /// </summary>
    public class BehaviourTreeConditionNode : BehaviourTreeBaseNode
    {

        public Func<bool> action;

        public BehaviourTreeConditionNode(Func<bool> action)
        {
            this.action = action;
        }

        public override ENodeState Excute()
        {
            if (action == null)
            {
                return ENodeState.Failed;
            }

            return action.Invoke() ? ENodeState.Success : ENodeState.Failed;
        }

    }

    #region 选择节点
    // 当执行本类型节点时，他将从头到尾迭代执行自己的子节点，如果遇到一个子节点执行后返回True，则停止迭代，本节点会向自己得上层父节点也会返回True。
    // 否则所有子节点都将返回Fasle，那么本节点也会向自己的父节点返回Fasle。

    /// <summary>
    /// 选择节点(每一帧执行一个子节点)
    /// </summary>
    public class BehaviourTreeSelectNodeOneFrameOneChild : BehaviourTreeBaseNode
    {
        public BehaviourTreeSelectNodeOneFrameOneChild() : base() { }

        public override ENodeState Excute()
        {
            BehaviourTreeBaseNode childNode;
            if (m_childList.Count != 0)
            {
                childNode = m_childList[m_nodeIndex];
                switch (childNode.Excute())//返回执行子节点的结果
                {
                    case ENodeState.Success:
                        m_nodeIndex = 0;
                        return ENodeState.Success;
                    case ENodeState.Failed:
                        m_nodeIndex++;
                        if (m_nodeIndex == m_childList.Count)
                        {
                            m_nodeIndex = 0;
                            return ENodeState.Failed;
                        }
                        break;
                    case ENodeState.Running:
                        return ENodeState.Running;
                }
            }
            return ENodeState.Failed;
        }
    }

    /// <summary>
    /// 选择节点(一帧执行全部子节点)
    /// </summary>
    public class BehaviourTreeSelectNodeOneFrameAllChild : BehaviourTreeBaseNode
    {
        public BehaviourTreeSelectNodeOneFrameAllChild() : base() { }
        public override ENodeState Excute()
        {
            foreach (BehaviourTreeBaseNode childNode in m_childList)
            {
                ENodeState result = childNode.Excute();
                if (result != ENodeState.Failed)
                {
                    return result;
                }
            }
            return ENodeState.Failed;
        }
    }
    #endregion

    #region 顺序节点
    // 当执行本类型节点时，他将从头到尾迭代执行自己的子节点，如果遇到一个子节点执行后返回Fasle,就会立即停止迭代，同时本节点会向自己的父节点也会返回Fasle，
    // 相反所有子节点都将返回True ，那么本节点也会向自己的父节点返回Ture。

    /// <summary>
    /// 顺序节点(每一帧执行一个子节点)
    /// </summary>
    public class BehaviourTreeSequenceNodeOneFrameOneChild : BehaviourTreeBaseNode
    {
        public BehaviourTreeSequenceNodeOneFrameOneChild() : base() { }
        public override ENodeState Excute()
        {
            BehaviourTreeBaseNode childNode;
            if (m_childList.Count != 0)
            {
                childNode = m_childList[m_nodeIndex];
                switch (childNode.Excute())
                {
                    case ENodeState.Success:
                        m_nodeIndex++;
                        if (m_nodeIndex == m_childList.Count)
                        {
                            m_nodeIndex = 0;
                            return ENodeState.Success;
                        }
                        break;
                    case ENodeState.Failed:
                        m_nodeIndex = 0;
                        return ENodeState.Failed;
                    case ENodeState.Running:
                        return ENodeState.Running;
                    default:
                        break;
                }
            }
            return ENodeState.Failed;
        }
    }

    /// <summary>
    /// 顺序节点(每一帧执行全部子节点)
    /// </summary>
    public class BehaviourTreeSequenceNodeOneFrameAllChild : BehaviourTreeBaseNode
    {
        public BehaviourTreeSequenceNodeOneFrameAllChild() : base() { }
        public override ENodeState Excute()
        {
            foreach(BehaviourTreeBaseNode childNode in m_childList)
            {
                ENodeState result = childNode.Excute();
                if(result != ENodeState.Success)
                {
                    return result;
                }
            }
            return ENodeState.Success;
        }
    }
    #endregion

    /// <summary>
    /// 反向装饰节点：
    /// 将结果反置返回给上级处理，即当子节点为True时，返回给自己的父节点为Fasle，反之同理。
    /// </summary>
    public class BehaviourTreeDecoratorNot : BehaviourTreeBaseNode
    {
        public Func<bool> action;

        public BehaviourTreeDecoratorNot(Func<bool> action)
        {
            this.action = action;
        }

        public override ENodeState Excute()
        {
            if (action == null)
            {
                return ENodeState.Failed;
            }

            return action.Invoke() ? ENodeState.Failed : ENodeState.Success;
        }
    }

    /// <summary>
    /// 黑板结点：
    /// 用于读取和写入公共数据，以便行为树节点之间共享信息，比如存储和传递状态信息，或者存储敌人的位置，其他的环境变量等
    /// </summary>
    public class DataBase
    {
        public Dictionary<string, object> m_dataContext = new Dictionary<string, object>();

        /// <summary>
        /// 写入公共数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Setdata(string key, object value)
        {
            if (m_dataContext.ContainsKey(key))
            {
                m_dataContext[key] = value;
            }
            else
            {
                m_dataContext.Add(key, value);
            }
        }

        /// <summary>
        /// 读取公共数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetData(string key)
        {
            if (m_dataContext.ContainsKey(key))
            {
                return m_dataContext[key];
            }
            Debug.LogWarning("没有此数据，无法获取");
            return null;
        }

        /// <summary>
        /// 清空公共数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ClearData(string key)
        {
            if (m_dataContext.ContainsKey(key))
            {
                m_dataContext.Remove(key);

                return true;
            }
            Debug.LogWarning("没有此数据，无法清除");
            return false;
        }
    }


}
