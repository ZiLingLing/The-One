using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Threading.Tasks;

namespace Roguelike
{
    public class FollowCamera : MonoBehaviour
    {
        public CinemachineVirtualCamera m_cinemachineVirtualCamera;
        private CinemachineBasicMultiChannelPerlin m_noiseProfile;
        #region 生命周期函数
        private void Awake()
        {
            m_cinemachineVirtualCamera = this.GetComponent<CinemachineVirtualCamera>();
            m_noiseProfile = m_cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            Debug.Log("CinemachineVirtualCamera: " + m_cinemachineVirtualCamera);
        }

        private void OnEnable()
        {
            Debug.Log("准备添加设置摄像机的相关方法");
            EventManager.AddEventListener<Transform>("SetCameraLookAt",SetCameraLookAt);
            EventManager.AddEventListener<Transform>("SetCameraFollow",SetCameraFollow);
            EventManager.AddEventListener<Vector3>("InitCamera", InitVirtualCamera);
            EventManager.AddEventListener("WoundShakeCamera", WoundShakeCamera);
            Debug.Log("设置摄像机相关方法已添加完毕");
        }

        private void Start()
        {

        }

        private void Update()
        {

        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener<Transform>("SetCameraLookAt", SetCameraLookAt);
            EventManager.RemoveEventListener<Transform>("SetCameraFollow", SetCameraFollow);
            EventManager.RemoveEventListener<Vector3>("InitCamera", InitVirtualCamera);
            EventManager.RemoveEventListener("WoundShakeCamera", WoundShakeCamera);
        }
        #endregion

        /// <summary>
        /// 用于设置虚拟相机的LookAt属性
        /// </summary>
        /// <param name="lookAt"></param>
        private void SetCameraLookAt(Transform lookAt)
        {
            //Debug.Log("设置lookat"+lookAt==null+lookAt.gameObject.name);
            m_cinemachineVirtualCamera.LookAt = lookAt;
        }

        /// <summary>
        /// 用于设置虚拟相机的Follow属性
        /// </summary>
        /// <param name="follow"></param>
        private void SetCameraFollow(Transform follow)
        {
            //Debug.Log("设置follow"+follow==null);
            m_cinemachineVirtualCamera.Follow = follow;
        }

        /// <summary>
        /// 受伤镜头抖动
        /// </summary>
        private void WoundShakeCamera()
        {
            float shakeTime = 1f;

            m_noiseProfile.m_PivotOffset = new Vector3(5f, 0, 5f);
            m_noiseProfile.m_AmplitudeGain = 3f;
            m_noiseProfile.m_FrequencyGain = 0.15f;
            StartCoroutine(ShakeCoroutine(shakeTime));

        }

        /// <summary>
        /// 初始化虚拟相机的各项参数
        /// </summary>
        private void InitVirtualCamera(Vector3 position)
        {
            Vector3 initPosition = new Vector3(position.x, position.y, position.z);
            this.transform.position = initPosition;
            this.transform.rotation = Quaternion.Euler(90f, 0, 0);

            //设置transposer
            CinemachineTransposer transposer = m_cinemachineVirtualCamera.AddCinemachineComponent<CinemachineTransposer>();
            //transposer = m_cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
            transposer.m_FollowOffset = new Vector3(0, 15.5f, 0);
            transposer.m_XDamping = transposer.m_YDamping = transposer.m_ZDamping = 1.5f;

        }

        /// <summary>
        /// 振动时间的计时协程
        /// </summary>
        /// <param name="shakeTime"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        private IEnumerator ShakeCoroutine(float shakeTime)
        {
            yield return new WaitForSeconds(shakeTime);
            m_noiseProfile.m_AmplitudeGain = 0;
            m_noiseProfile.m_FrequencyGain = 0;
        }

    }


}
