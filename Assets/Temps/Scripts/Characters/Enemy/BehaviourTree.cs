using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Roguelike
{
    /// <summary>
    /// ��Ϊ����
    /// </summary>
    public abstract class BehaviourTree : MonoBehaviour
    {
        protected BehaviourTreeBaseNode m_root;

        protected DataBase m_database = new DataBase();//Ϊÿ��������һ���ڰ�ڵ�

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
    /// ��Ϊ���ڵ��ִ�н��
    /// </summary>
    public enum ENodeState
    {
        Success,
        Failed,
        Running
    }

    /// <summary>
    /// ��Ϊ�ڵ����
    /// </summary>
    public abstract class BehaviourTreeBaseNode
    {
        /// <summary>
        /// ��Ϊ���ӽڵ��б�
        /// </summary>
        protected List<BehaviourTreeBaseNode> m_childList = new List<BehaviourTreeBaseNode>();

        public BehaviourTreeBaseNode m_parent;

        /// <summary>
        /// ִ�е�ǰ�߼����ӽڵ����
        /// </summary>
        protected int m_nodeIndex;

        /// <summary>
        /// ����ӽڵ�
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
        /// �Ƴ��ӽڵ�
        /// </summary>
        /// <param name="index"></param>
        public virtual void RemoveChild(int index)
        {
            m_childList.Remove(m_childList[index]);
        }

        /// <summary>
        /// ִ�нڵ��߼�
        /// </summary>
        /// <returns></returns>
        public abstract ENodeState Excute();
    }

    /// <summary>
    /// ��Ϊ�ڵ�:
    /// ��Ϊ�ڵ�ͨ����Ϊ����Ҷ�ӽ�㣬������ɾ����һ����Ϊ֮����ݼ�������÷��ط���ֵ��
    /// ��Ϊ�ڵ������ִ��һ�εõ����(����ʧ�ܻ�ɹ�)��Ҳ���Էֲ�ִ�кܶ�Ρ�
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
    /// �����ڵ㣺
    /// ���������㷵��True�����򷵻�Fasle��
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

    #region ѡ��ڵ�
    // ��ִ�б����ͽڵ�ʱ��������ͷ��β����ִ���Լ����ӽڵ㣬�������һ���ӽڵ�ִ�к󷵻�True����ֹͣ���������ڵ�����Լ����ϲ㸸�ڵ�Ҳ�᷵��True��
    // ���������ӽڵ㶼������Fasle����ô���ڵ�Ҳ�����Լ��ĸ��ڵ㷵��Fasle��

    /// <summary>
    /// ѡ��ڵ�(ÿһִ֡��һ���ӽڵ�)
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
                switch (childNode.Excute())//����ִ���ӽڵ�Ľ��
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
    /// ѡ��ڵ�(һִ֡��ȫ���ӽڵ�)
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

    #region ˳��ڵ�
    // ��ִ�б����ͽڵ�ʱ��������ͷ��β����ִ���Լ����ӽڵ㣬�������һ���ӽڵ�ִ�к󷵻�Fasle,�ͻ�����ֹͣ������ͬʱ���ڵ�����Լ��ĸ��ڵ�Ҳ�᷵��Fasle��
    // �෴�����ӽڵ㶼������True ����ô���ڵ�Ҳ�����Լ��ĸ��ڵ㷵��Ture��

    /// <summary>
    /// ˳��ڵ�(ÿһִ֡��һ���ӽڵ�)
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
    /// ˳��ڵ�(ÿһִ֡��ȫ���ӽڵ�)
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
    /// ����װ�νڵ㣺
    /// ��������÷��ظ��ϼ����������ӽڵ�ΪTrueʱ�����ظ��Լ��ĸ��ڵ�ΪFasle����֮ͬ��
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
    /// �ڰ��㣺
    /// ���ڶ�ȡ��д�빫�����ݣ��Ա���Ϊ���ڵ�֮�乲����Ϣ������洢�ʹ���״̬��Ϣ�����ߴ洢���˵�λ�ã������Ļ���������
    /// </summary>
    public class DataBase
    {
        public Dictionary<string, object> m_dataContext = new Dictionary<string, object>();

        /// <summary>
        /// д�빫������
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
        /// ��ȡ��������
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetData(string key)
        {
            if (m_dataContext.ContainsKey(key))
            {
                return m_dataContext[key];
            }
            Debug.LogWarning("û�д����ݣ��޷���ȡ");
            return null;
        }

        /// <summary>
        /// ��չ�������
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
            Debug.LogWarning("û�д����ݣ��޷����");
            return false;
        }
    }


}
