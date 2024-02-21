using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace Roguelike
{
    /// <summary>
    /// 事件类接口
    /// </summary>
    public interface IEventInfo
    {

    }

    #region 事件类
    /// <summary>
    /// 含一个参数无返回值的事件类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventInfo<T> : IEventInfo
    {
        public UnityAction<T> m_event;
    }

    /// <summary>
    /// 无参无返回值事件类
    /// </summary>
    public class EventInfo : IEventInfo
    {
        public UnityAction m_event;
    }

    public class EventInfo<T1,T2> : IEventInfo
    {
        public UnityAction<T1,T2> m_event;
    }

    public class EventInfo<T1, T2, T3> : IEventInfo
    {
        public UnityAction<T1, T2, T3> m_event;
    }

    /// <summary>
    /// 无参有返回值事件类
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class FuncEventInfo<TResult> : IEventInfo
    {
        public Func<TResult> m_event;
    }
    #endregion


    #region 事件管理器
    /// <summary>
    /// 事件管理器
    /// </summary>
    public static class EventManager
    {
        private static Dictionary<string, IEventInfo> s_eventDictionary = new Dictionary<string, IEventInfo>();

        #region 注册事件
        /// <summary>
        /// 注册事件
        /// </summary>
        public static void AddEventListener(string eventName, UnityAction myEvent)
        {
            if(s_eventDictionary.ContainsKey(eventName))
            {
                (s_eventDictionary[eventName] as EventInfo).m_event += myEvent;
            }
            else
            {
                s_eventDictionary.Add(eventName, new EventInfo() { m_event = myEvent});
            }
        }

        public static void AddEventListener<T>(string eventName, UnityAction<T> myEvent)
        {
            if (s_eventDictionary.ContainsKey(eventName))
            {
                (s_eventDictionary[eventName] as EventInfo<T>).m_event += myEvent;
            }
            else
            {
                s_eventDictionary.Add(eventName, new EventInfo<T>() { m_event = myEvent });
            }
        }

        public static void AddEventListener<T1,T2>(string eventName, UnityAction<T1,T2> myEvent)
        {
            if (s_eventDictionary.ContainsKey(eventName))
            {
                (s_eventDictionary[eventName] as EventInfo<T1,T2>).m_event += myEvent;
            }
            else
            {
                s_eventDictionary.Add(eventName, new EventInfo<T1,T2>() { m_event = myEvent });
            }
        }

        public static void AddEventListener<T1, T2, T3>(string eventName, UnityAction<T1, T2, T3> myEvent)
        {
            if (s_eventDictionary.ContainsKey(eventName))
            {
                (s_eventDictionary[eventName] as EventInfo<T1, T2, T3>).m_event += myEvent;
            }
            else
            {
                s_eventDictionary.Add(eventName, new EventInfo<T1, T2, T3>() { m_event = myEvent });
            }
        }

        public static void AddEventListener<TResult>(string eventName, Func<TResult> myEvent)
        {
            if (s_eventDictionary.ContainsKey(eventName))
            {
                (s_eventDictionary[eventName] as FuncEventInfo<TResult>).m_event += myEvent;
            }
            else
            {
                s_eventDictionary.Add(eventName, new FuncEventInfo<TResult>() { m_event = myEvent });
            }
        }
        #endregion

        #region 移除事件
        /// <summary>
        /// 移除事件
        /// </summary>
        public static void RemoveEventListener(string eventName,UnityAction myEvent)
        {
            if(s_eventDictionary.ContainsKey(eventName))
            {
                (s_eventDictionary[eventName] as EventInfo).m_event -= myEvent;
            }
        }

        public static void RemoveEventListener<T>(string eventName, UnityAction<T> myEvent)
        {
            if (s_eventDictionary.ContainsKey(eventName))
            {
                (s_eventDictionary[eventName] as EventInfo<T>).m_event -= myEvent;
            }
        }

        public static void RemoveEventListener<T1,T2>(string eventName, UnityAction<T1,T2> myEvent)
        {
            if (s_eventDictionary.ContainsKey(eventName))
            {
                (s_eventDictionary[eventName] as EventInfo<T1,T2>).m_event -= myEvent;
            }
        }

        public static void RemoveEventListener<T1, T2, T3>(string eventName, UnityAction<T1, T2, T3> myEvent)
        {
            if (s_eventDictionary.ContainsKey(eventName))
            {
                (s_eventDictionary[eventName] as EventInfo<T1, T2, T3>).m_event -= myEvent;
            }
        }

        public static void RemoveEventListener<TResult>(string eventName, Func<TResult> myEvent)
        {
            if (s_eventDictionary.ContainsKey(eventName))
            {
                (s_eventDictionary[eventName] as FuncEventInfo<TResult>).m_event -= myEvent;
            }
        }
        #endregion

        #region 触发事件
        /// <summary>
        /// 无返回值的事件触发
        /// </summary>
        /// <param name="eventName"></param>
        public static void TriggerEvent(string eventName)
        {
            if(s_eventDictionary.ContainsKey(eventName))
            {
                (s_eventDictionary[eventName] as EventInfo).m_event?.Invoke();
            }
        }

        public static void TriggerEvent<T>(string eventName,T param)
        {
            if (s_eventDictionary.ContainsKey(eventName))
            {
                (s_eventDictionary[eventName] as EventInfo<T>).m_event?.Invoke(param);
            }
        }

        public static void TriggerEvent<T1,T2>(string eventName, T1 param1, T2 param2)
        {
            if (s_eventDictionary.ContainsKey(eventName))
            {
                (s_eventDictionary[eventName] as EventInfo<T1,T2>).m_event?.Invoke(param1,param2);
            }
        }

        public static void TriggerEvent<T1, T2, T3>(string eventName, T1 param1, T2 param2, T3 param3)
        {
            if (s_eventDictionary.ContainsKey(eventName))
            {
                (s_eventDictionary[eventName] as EventInfo<T1, T2, T3>).m_event?.Invoke(param1, param2, param3);
            }
        }

        /// <summary>
        /// 有返回值的事件触发
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public static TResult TriggerEvent<TResult>(string eventName)
        {
            TResult result = default(TResult);
            if (s_eventDictionary.ContainsKey(eventName))
            {
                FuncEventInfo<TResult> eventInfo = s_eventDictionary[eventName] as FuncEventInfo<TResult>;
                if (eventInfo != null && eventInfo.m_event != null)
                {
                    result = eventInfo.m_event.Invoke();
                }
            }
            return result;
        }

        #endregion

        #region 清空事件
        /// <summary>
        /// 清空事件
        /// </summary>
        public static void Clear()
        {
            s_eventDictionary.Clear();
        }
        #endregion
    }
    #endregion
}

