using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Roguelike
{

    public class ScenesManager : MonoBehaviour
    {
        private static ScenesManager m_instance;//私有单例

        private Action m_onSceneLoaded = null;//场景加载完成回调

        private string m_strNextSceneName = null;//将要加载的场景名
        private string m_strCurSceneName = null;//当前场景名，如若没有场景，则默认返回 Login
        private string m_strPreSceneName = null;//上一个场景名

        private bool m_bLoading = false;//是否正在加载中

        private bool m_bDestroyAuto = true;//自动删除 loading 背景

        private const string m_strLoadSceneName = "LoadingScene";//加载场景名字

        private GameObject m_objLoadProgress = null;//加载进度显示对象

        //获取当前场景名
        public static string s_strLoadedSceneName => m_instance.m_strCurSceneName;

        public static void CreateInstance(GameObject go)
        {
            if (null != m_instance)
            {
                return;
            }
            m_instance = go.AddComponent<ScenesManager>();
            DontDestroyOnLoad(m_instance);
            m_instance.m_strCurSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }

        public static void LoadPreScene()
        {
            if (string.IsNullOrEmpty(m_instance.m_strPreSceneName))
            {
                return;
            }
            LoadScene(m_instance.m_strPreSceneName);
        }

        public static void LoadScene(string strLevelName)
        {
            m_instance.LoadLevel(strLevelName, null);
        }

        public static void LoadScene(string strLevelName, Action onSecenLoaded)
        {
            m_instance.LoadLevel(strLevelName, onSecenLoaded);
        }

        private void LoadLevel(string strLevelName, Action onSecenLoaded, bool isDestroyAuto = true)
        {
            if (m_bLoading || m_strCurSceneName == strLevelName)
            {
                return;
            }

            m_bLoading = true;  // 锁屏
                                // 开始加载    
            m_onSceneLoaded = onSecenLoaded;
            m_strNextSceneName = strLevelName;
            m_strPreSceneName = m_strCurSceneName;
            m_strCurSceneName = m_strLoadSceneName;
            m_bDestroyAuto = isDestroyAuto;

            //先异步加载 Loading 界面
            StartCoroutine(StartLoadSceneOnEditor(m_strLoadSceneName, OnLoadingSceneLoaded, null));
        }

        /// <summary>
        /// 过渡场景加载完成回调
        /// </summary>
        private void OnLoadingSceneLoaded()
        {
            // 过渡场景加载完成后加载下一个场景
            StartCoroutine(StartLoadSceneOnEditor(m_strNextSceneName, OnNextSceneLoaded, OnNextSceneProgress));
        }

        /// <summary>
        /// 开始加载
        /// </summary>
        /// <param name="strLevelName"></param>
        /// <param name="OnSecenLoaded"></param>
        /// <param name="OnSceneProgress"></param>
        /// <returns></returns>
        private IEnumerator StartLoadSceneOnEditor(string strLevelName, Action OnSecenLoaded, Action<float> OnSceneProgress)
        {
            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(strLevelName);
            if (null == async)
            {
                yield break;
            }

            //*加载进度
            while (!async.isDone)
            {
                float fProgressValue;
                if (async.progress < 0.9f)
                {
                    fProgressValue = async.progress;
                }
                else
                {
                    fProgressValue = 1.0f;
                }
                OnSceneProgress?.Invoke(fProgressValue);
                yield return null;
            }
            OnSecenLoaded?.Invoke();
        }

        /// <summary>
        /// 加载下一场景完成回调
        /// </summary>
        private void OnNextSceneLoaded()
        {
            m_bLoading = false;
            OnNextSceneProgress(1);
            m_strCurSceneName = m_strNextSceneName;
            m_strNextSceneName = null;
            m_onSceneLoaded?.Invoke();
        }

        /// <summary>
        /// 场景加载进度变化
        /// </summary>
        /// <param name="fProgress"></param>
        private void OnNextSceneProgress(float fProgress)
        {
            if (null == m_objLoadProgress)
            {
                m_objLoadProgress = GameObject.Find("TextLoadProgress");
            }
            Text textLoadProgress = m_objLoadProgress.GetComponent<Text>();
            if (null == textLoadProgress)
            {
                return;
            }
            textLoadProgress.text = (fProgress * 100).ToString() + "%";
        }
    }
}