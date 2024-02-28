using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Roguelike
{
    public class UIBattle : View
    {
        private Image m_health;
        private Image m_shadowHealth;
        private Button m_pauseButton;

        private float m_currentHealth;
        private float m_healthUpper;
        private float m_healthBefore;

        private Input m_input;

        private bool m_isFreeze = false;

        private void Awake()
        {
            Init();
            m_input = new Input();
            m_input.KeyboardAndMouse.Pause.started += Interaction_performed;
        }

        private void OnEnable()
        {
            EventManager.AddEventListener<float>("UpdateCurrentHealth", UpdateCurrentHealth);
            EventManager.AddEventListener("UIBattleUnlock", Unlock);
            m_input.KeyboardAndMouse.Enable();
        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener<float>("UpdateCurrentHealth", UpdateCurrentHealth);
            EventManager.RemoveEventListener("Unlock", Unlock);
            m_input.KeyboardAndMouse.Disable();
        }

        private void Interaction_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (m_input.KeyboardAndMouse.Pause.IsPressed() && m_isFreeze == false)
            {
                Pause();
            }
        }

        public override void Init()
        {
            m_health = this.transform.Find("Health").GetComponent<Image>();
            m_shadowHealth = this.transform.Find("ShadowHealth").GetComponent<Image>();
            m_pauseButton = this.transform.Find("Pause").GetComponent<Button>();

            m_pauseButton.onClick.AddListener(Pause);

            m_isFreeze = false;
        }

        /// <summary>
        /// 血条变动效果
        /// </summary>
        /// <param name="change"></param>
        private void UpdateCurrentHealth(float change)
        {
            m_healthBefore = m_currentHealth;
            m_currentHealth += change;
            m_health.fillAmount = m_currentHealth / m_healthUpper;
            m_shadowHealth.fillAmount = Mathf.Lerp(m_healthBefore, m_currentHealth, 1f);
        }

        /// <summary>
        /// 暂停游戏
        /// </summary>
        private void Pause()
        {
            m_isFreeze = true;
            //EventManager.TriggerEvent("Pause");
            UIManager.Show<View>("UIPause");
        }

        private void Unlock()
        {
            m_isFreeze = false;
        }
    }
}

