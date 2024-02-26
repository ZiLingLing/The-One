using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Roguelike
{
    public class UIPause : View
    {
        private Button m_settingButton;
        private Button m_continueButton;
        private Button m_exitButton;

        private Input m_input;
        public Animator m_pauseAnimator;

        private bool m_isFreeze = true;

        private void Awake()
        {
            Init();
            m_pauseAnimator = this.GetComponent<Animator>();
            m_input = new Input();
            m_input.KeyboardAndMouse.Pause.started += Pause_started;
        }

        private void OnEnable()
        {
            m_input.Enable();
        }

        private void OnDisable()
        {
            m_input.Disable();
        }

        private void Pause_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (m_input.KeyboardAndMouse.Pause.IsPressed() && m_isFreeze == false)
            {
                Continue();
            }
        }

        public override void Init()
        {
            m_settingButton = this.transform.Find("Setting").GetComponent<Button>();
            m_continueButton = this.transform.Find("Continue").GetComponent<Button>();
            m_exitButton = this.transform.Find("Exit").GetComponent<Button>();

            m_continueButton.onClick.AddListener(Continue);
            m_settingButton.onClick.AddListener(Setting);
            m_exitButton.onClick.AddListener(Exit);

            m_isFreeze = true;
        }

        private void Continue()
        {

        }

        private void ContinueEvent()
        {
            this.Hide();
            EventManager.TriggerEvent("Continue");
            Time.timeScale = 1;
        }

        private void Setting()
        {
            m_isFreeze = true;
            Debug.Log("显示设置界面,显示UISetting");
        }

        private void Exit()
        {
            Debug.Log("回到开始界面，显示UIMain");
        }

        private void ChangeState()
        {
            if (m_isFreeze == true)
            {
                m_isFreeze = false;
            }
            else
            {
                m_isFreeze = true;
            }
        }
    }
}

