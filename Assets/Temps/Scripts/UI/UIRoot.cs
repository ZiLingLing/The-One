using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    public class UIRoot : MonoBehaviour
    {
        public static Dictionary<string, View> m_views = new Dictionary<string, View>(); //定义一个字典存储所有UI界面
        private void Awake()
        {
            var prefabs = Resources.LoadAll<GameObject>("UI");//加载UI文件夹下的所有UI预制体
            foreach (GameObject view in prefabs)
            {
                if (!m_views.ContainsKey(view.name))
                {
                    GameObject prefab = Instantiate(view,this.transform);
                    prefab.name = prefab.name.Replace("(Clone)", "");//去除实例化物体的(Clone)后缀，和预制体名字统一
                    m_views.Add(prefab.name, prefab.GetComponent<View>());//存入字典
                }
            }

           
        }
    }
}
