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

        private bool m_isFreeze = false;

        private void Awake()
        {
            m_pauseAnimator = this.GetComponent<Animator>();

            m_input = new Input();
            m_input.KeyboardAndMouse.Pause.started += Pause_started;
            
            Init();

        }

        private void OnEnable()
        {
            m_input.Enable();
            m_pauseAnimator.Play("UIPauseShow");
            EventManager.AddEventListener("UIPauseUnlock", Unlock);
        }

        private void OnDisable()
        {
            m_input.Disable();
            EventManager.RemoveEventListener("UIPauseUnlock", Unlock);
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

            AddAnimatorClipEvent();
            m_isFreeze = false;
        }

        /// <summary>
        /// 绑定在显示动画上的事件
        /// </summary>
        private void OnPanelShowEvent()
        {
            m_isFreeze = false;
            Time.timeScale = 0;
        }

        /// <summary>
        /// 绑定在关闭动画上的事件
        /// </summary>
        private void OnPanelCloseEvent()
        {
            this.Hide();
            m_isFreeze = true;
            EventManager.TriggerEvent("Continue");
            EventManager.TriggerEvent("UIBattleUnlock");
        }

        /// <summary>
        /// 点击Continue按钮事件
        /// </summary>
        private void Continue()
        {
            Time.timeScale = 1;
            m_pauseAnimator.Play("UIPauseClose");
            Debug.Log("Continue");
        }

        /// <summary>
        /// 点击Setting按钮事件
        /// </summary>
        private void Setting()
        {
            m_isFreeze = true;
            UIManager.Show<View>("UISetting");
        }

        /// <summary>
        /// 点击Exit按钮事件
        /// </summary>
        private void Exit()
        {
            Debug.Log("回到开始界面，显示UIMain");
        }

        private void AddAnimatorClipEvent()
        {
            AnimationClip[] clips = m_pauseAnimator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips)
            {
                //找到对应的动画的名称
                if (clip.name.Equals("UIPauseShow"))
                {
                    //创建新事件
                    AnimationEvent m_castSpellEvent = new AnimationEvent();
                    //对应事件触发相应函数的名称
                    m_castSpellEvent.functionName = "OnPanelShowEvent";
                    //设定对应事件在相应动画上的触发时间点
                    m_castSpellEvent.time = clip.length;
                    clip.AddEvent(m_castSpellEvent);
                }
                else if(clip.name.Equals("UIPauseClose"))
                {
                    AnimationEvent m_castSpellEvent = new AnimationEvent();
                    m_castSpellEvent.functionName = "OnPanelCloseEvent";
                    m_castSpellEvent.time = clip.length * 0.9f;
                    clip.AddEvent(m_castSpellEvent);
                }
            }
            //重新绑定动画器的所有动画的属性
            m_pauseAnimator.Rebind();
        }

        /// <summary>
        /// 关闭Setting面板后使得Pause面板可用
        /// </summary>
        private void Unlock()
        {
            m_isFreeze = false;
        }
    }
}

