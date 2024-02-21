using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    /// <summary>
    /// 全局存在一个管理输入队列的CommandManager
    /// </summary>
    public class CommandManager
    {
        private static CommandManager m_instance;

        private Deque<ICommand> m_commandBuffer = new Deque<ICommand>();

        public static CommandManager Instance
        {
            get
            {
                if(m_instance==null)
                {
                    m_instance = new CommandManager();
                }
                return m_instance;
            }
        }

        /// <summary>
        /// 向队列中添加命令
        /// </summary>
        /// <param name="command"></param>
        public void AddCommands(ICommand command)
        {
            m_commandBuffer.BackEnqueue(command);
        }

        /// <summary>
        /// 撤销命令
        /// </summary>
        public void UndoCommands()
        {
            while(m_commandBuffer.Count!=0)
            {
                m_commandBuffer.BackDequeue();
            }
        }
    }

    /// <summary>
    /// 双向队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Deque<T>
    {
        private LinkedList<T> m_linkList;

        /// <summary>
        /// 队列中元素的数量
        /// </summary>
        private int m_count;

        /// <summary>
        /// 读取队列中元素的数量
        /// </summary>
        public int Count
        {
            get { m_count = m_linkList.Count; return m_count; }
        }

        public Deque()
        {
            m_linkList = new LinkedList<T>();
        }

        /// <summary>
        /// 从队首入队
        /// </summary>
        /// <param name="t"></param>
        public void FrontEnqueue(T t)
        {
            m_linkList.AddFirst(t);
        }

        /// <summary>
        /// 从队首出队
        /// </summary>
        /// <returns></returns>
        public void FrontDequeue()
        {
            if(m_linkList.Count!=0)
            {
                m_linkList.RemoveFirst();
            }
        }

        /// <summary>
        /// 从队尾入队
        /// </summary>
        /// <param name="t"></param>
        public void BackEnqueue(T t)
        {
            m_linkList.AddLast(t);
        }

        /// <summary>
        /// 从队尾出队
        /// </summary>
        /// <returns></returns>
        public void BackDequeue()
        {
            if (m_linkList.Count != 0)
            {
                m_linkList.RemoveLast();
            }
        }

        /// <summary>
        /// 清空队列
        /// </summary>
        public void Clear()
        {
            m_linkList.Clear();
        }
    }
}


