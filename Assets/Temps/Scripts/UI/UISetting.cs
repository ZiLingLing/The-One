using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Roguelike
{
    public class UISetting : View
    {
        private Button m_close;
        private Slider m_brightness;
        private Slider m_voice;

        private Input m_input;

        private void Awake()
        {
            m_input = new Input();
            m_input.KeyboardAndMouse.Pause.started += Pause_started;

            Init();
        }

        private void OnEnable()
        {
            m_input.Enable();
        }

        private void OnDisable()
        {
            m_input.Disable();
        }

        private void Pause_started(InputAction.CallbackContext obj)
        {
            Close();
        }

        public override void Init()
        {
            m_close = this.transform.Find("Close").GetComponent<Button>();
            m_brightness = this.transform.Find("Brightness").GetComponent<Slider>();
            m_voice = this.transform.Find("Voice").GetComponent<Slider>();

            m_close.onClick.AddListener(Close);

            m_brightness.value = 0.8f;
            m_voice.value = 0.8f;
        }

        /// <summary>
        /// 关闭按钮绑定函数
        /// </summary>
        private void Close()
        {
            this.Hide();
            EventManager.TriggerEvent("UIPauseUnlock");
        }

        /// <summary>
        /// 亮度条绑定函数
        /// </summary>
        private void Brightness()
        {

        }

        /// <summary>
        /// 声音条绑定函数
        /// </summary>
        private void Voice()
        {

        }
    }
}
