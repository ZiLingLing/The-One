using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    /// <summary>
    /// Entry场景内的Launcher
    /// </summary>
    public class Launcher : MonoBehaviour
    {
        void Awake()
        {
            //实例化所有Root
            GameObject prefab = Instantiate(Resources.Load<GameObject>("Roots/UIRoot"));
            prefab.name = prefab.name.Replace("(Clone)", "");
            new UIManager();

            //var roots = Resources.LoadAll<GameObject>("Roots");//加载UI文件夹下的所有UI预制体
            //foreach (GameObject root in roots)
            //{
            //    GameObject prefab = Instantiate(Resources.Load<GameObject>("Roots/"+root.name));
            //    prefab.name = prefab.name.Replace("(Clone)", "");
            //}


        }
    }
}

