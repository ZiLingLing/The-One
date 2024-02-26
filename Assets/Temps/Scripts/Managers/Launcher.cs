using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    /// <summary>
    /// Entry�����ڵ�Launcher
    /// </summary>
    public class Launcher : MonoBehaviour
    {
        void Awake()
        {
            //ʵ��������Root
            GameObject prefab = Instantiate(Resources.Load<GameObject>("Roots/UIRoot"));
            prefab.name = prefab.name.Replace("(Clone)", "");
            new UIManager();

            //var roots = Resources.LoadAll<GameObject>("Roots");//����UI�ļ����µ�����UIԤ����
            //foreach (GameObject root in roots)
            //{
            //    GameObject prefab = Instantiate(Resources.Load<GameObject>("Roots/"+root.name));
            //    prefab.name = prefab.name.Replace("(Clone)", "");
            //}


        }
    }
}

