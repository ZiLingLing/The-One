using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Roguelike
{
    public class VolumeScript : MonoBehaviour
    {
        public Volume m_volume;

        private void Awake()
        {
            m_volume = this.GetComponent<Volume>();
        }

        private void OnEnable()
        {
            EventManager.AddEventListener<float>("ChangeBrightness", ChangeBrightness);
        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener<float>("ChangeBrightness", ChangeBrightness);
        }

        /// <summary>
        /// 调整亮度
        /// </summary>
        /// <param name="brightnessValue"></param>
        private void ChangeBrightness(float brightnessValue)
        {
            // 获取VolumeProfile
            VolumeProfile volumeProfile = m_volume.profile;

            // 检查VolumeProfile是否存在
            if (volumeProfile == null)
            {
                return;
            }

            // 在VolumeProfile中查找对应的VolumeComponent（假设为BrightnessSaturationAndContrast）
            if (volumeProfile.TryGet(out BrightnessSaturationAndContrast brightnessSaturationAndContrast))
            {
                brightnessSaturationAndContrast.m_brightness.value = brightnessValue;
            }
        }

        /// <summary>
        /// 调整Y轴上雾效的高度
        /// </summary>
        /// <param name="yValue"></param>
        private void ChangeFogHeight(float yValue)
        {

        }
    }
}
