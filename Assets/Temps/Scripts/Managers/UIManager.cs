using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    public class UIManager
    {
        public static Dictionary<string, View> s_viewsDic = new Dictionary<string, View>();//接收所有UI预制体
        public UIManager()
        {
            s_viewsDic = UIRoot.m_views;//赋值
            Initilization();
        }

        /// <summary>
        /// 初始化赋值
        /// </summary>
        private static void Initilization()
        {
            foreach (var view in s_viewsDic)
            {
                Debug.Log(view.Value == null );
                view.Value.Init();
                view.Value.Hide();
            }
            //Show<UIMain>();
        }

        /// <summary>
        /// 获取当前UI视图
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetView<T>() where T : View
        {
            foreach (var itemView in s_viewsDic)
            {
                if (itemView.Value is T tView)
                {
                    return tView;
                }
            }
            return null;
        }

        /// <summary>
        /// 显示所有UI视图
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Show<T>() where T : View
        {
            foreach (var itemView in s_viewsDic)
            {
                if (itemView.Value is T)
                {
                    itemView.Value.Show();
                }
            }
        }

        /// <summary>
        /// 显示某一个UI视图
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        public static void Show<T>(string name) where T : View
        {
            if (s_viewsDic.ContainsKey(name))
            {
                View itemView = s_viewsDic[name];
                itemView.Show();
            }
        }
        /// <summary>
        /// 关闭UI视图
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Close<T>() where T : View
        {
            foreach (var itemView in s_viewsDic)
            {
                if (itemView.Value is T)
                {
                    itemView.Value.Hide();
                }
            }
        }
    }
}

