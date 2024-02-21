using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    /// <summary>
    /// 对象池
    /// </summary>
    public class GameObjectPoolManager : MonoBehaviour
    {
        public static List<PoolObjectInfo> m_objectPools = new List<PoolObjectInfo>();

        private GameObject m_objectPoolEmptyHolder;
        private static GameObject s_gameObjectEmpty;
        private static GameObject s_particleSystemEmpty;

        public enum EPoolType
        {
            GameObject,
            ParticleSystem,
            None,
        }

        public static EPoolType s_poolingType;

        private void Awake()
        {
            SetupEmpties();
        }

        #region 创建对象
        /// <summary>
        /// 通过对象池创建对象
        /// </summary>
        /// <param name="objectToSpawn"></param>
        /// <param name="spawnPosition"></param>
        /// <param name="spawnRotation"></param>
        /// <returns></returns>
        public static GameObject SpawnObject(GameObject objectToSpawn,Vector3 spawnPosition,Quaternion spawnRotation,EPoolType poolType=EPoolType.None,float scaleX=1f,float scaleY=1f,float scaleZ=1f)
        {
            PoolObjectInfo pool = m_objectPools.Find(p => p.m_lookUpString == objectToSpawn.name);
            //PoolObjectInfo pool = null;
            //foreach(PoolObjectInfo p in m_objectPools)
            //{
            //    if(p.m_lookUpString==objectToSpawn.name)
            //    {
            //        pool = p;
            //        break;
            //    }
            //}

            //如果对象池列表中没有该对象池则开辟一个新的对象池并添加到列表中
            if(pool==null)
            {
                pool = new PoolObjectInfo(objectToSpawn.name);
                m_objectPools.Add(pool);
            }

            //检查池中有无失活物体，循环检查池中所有的失活对象，将失活对象赋给新创建的对象
            GameObject spawnAbleObj = null;
            foreach(GameObject obj in pool.m_InactiveObjects)
            {
                if(obj!=null)
                {
                    spawnAbleObj = obj;
                    break;
                }
            }

            //如果没有找到一个失活的该池中的GameObject，实例化一个新的对象赋给它，
            //否则给位置以及朝向赋值并从失活对象列表中删除该对象然后重新激活该物体
            if (spawnAbleObj == null)
            {
                //找到创建物体对应的父物体
                GameObject parentObject = SetParentObject(poolType);

                spawnAbleObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);

                if (parentObject != null)
                {
                    spawnAbleObj.transform.SetParent(parentObject.transform);
                }
            }
            else
            {
                spawnAbleObj.transform.position = spawnPosition;
                spawnAbleObj.transform.rotation = spawnRotation;
                pool.m_InactiveObjects.Remove(spawnAbleObj);
                spawnAbleObj.SetActive(true);
            }

            spawnAbleObj.transform.localScale = new Vector3(scaleX,scaleY,scaleZ);
            return spawnAbleObj;
        }

        public static GameObject SpawnObject(GameObject objectToSpawn, Transform parentTransform)
        {
            PoolObjectInfo pool = m_objectPools.Find(p => p.m_lookUpString == objectToSpawn.name);

            if (pool == null)
            {
                pool = new PoolObjectInfo(objectToSpawn.name);
                m_objectPools.Add(pool);
            }

            GameObject spawnAbleObj = null;
            foreach (GameObject obj in pool.m_InactiveObjects)
            {
                if (obj != null)
                {
                    spawnAbleObj = obj;
                    break;
                }
            }

            if (spawnAbleObj == null)
            {
                spawnAbleObj = Instantiate(objectToSpawn, parentTransform);

                spawnAbleObj.transform.SetParent(parentTransform);
                spawnAbleObj.transform.position = parentTransform.position;
                spawnAbleObj.transform.rotation = parentTransform.rotation;
            }
            else
            {
                pool.m_InactiveObjects.Remove(spawnAbleObj);
                spawnAbleObj.SetActive(true);
            }

            return spawnAbleObj;
        }
        #endregion
        /// <summary>
        /// 把物体返回该池中
        /// </summary>
        /// <param name="obj"></param>
        public static void ReturnObjectToPool(GameObject obj)
        {
            string trueName = obj.name.Substring(0, obj.name.Length - 7);
            Debug.Log(obj.name+"--"+ trueName);
            PoolObjectInfo pool = m_objectPools.Find(p=>trueName == p.m_lookUpString);

            if(pool!=null)
            {
                obj.SetActive(false);
                pool.m_InactiveObjects.Add(obj);
            }
            else
            {
                Debug.Log("正在释放一个不在对象池中的物体");
            }
        }

        /// <summary>
        /// 创建出不同类型的池子对应的空物体，以及它们的父物体
        /// </summary>
        private void SetupEmpties()
        {
            m_objectPoolEmptyHolder = new GameObject("Pool Objects");

            s_gameObjectEmpty = new GameObject("GameObjects");
            s_gameObjectEmpty.transform.SetParent(m_objectPoolEmptyHolder.transform);

            s_particleSystemEmpty = new GameObject("Particle Effects");
            s_particleSystemEmpty.transform.SetParent(m_objectPoolEmptyHolder.transform);

        }

        /// <summary>
        /// 把不同生成的不同类型的物体放在对应的父物体下
        /// </summary>
        /// <param name="poolType"></param>
        /// <returns></returns>
        private static GameObject SetParentObject(EPoolType poolType)
        {
            switch (poolType)
            {
                case EPoolType.GameObject:
                    return s_gameObjectEmpty;
                case EPoolType.ParticleSystem:
                    return s_particleSystemEmpty;
                case EPoolType.None:
                    return null;
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// 对象池信息
    /// </summary>
    public class PoolObjectInfo
    {
        public string m_lookUpString;
        public List<GameObject> m_InactiveObjects = new List<GameObject>();

        public PoolObjectInfo(string lookUpString)
        {
            m_lookUpString = lookUpString;
        }
    }
}