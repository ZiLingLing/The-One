using UnityEngine;

namespace Roguelike
{
    public class TestAI : MonoBehaviour
    {
        private BehaviourTreeBaseNode m_root;
        private DataBase m_database = new DataBase();

        private void Start()
        {
            m_root = SetUpTree();
        }

        private void Update()
        {
            if (m_root != null)
            {
                m_root.Excute();
            }
        }

        private BehaviourTreeBaseNode SetUpTree()
        {
            // 创建行为树节点
            BehaviourTreeBaseNode root = new BehaviourTreeSequenceNodeOneFrameOneChild();

            // 添加子节点
            root.AddChild(
                new BehaviourTreeConditionNode(CheckIfEnemyInRange),
                new BehaviourTreeActionNode(AttackEnemy)
            );

            return root;
        }

        // 检查是否有敌人在攻击范围内
        private bool CheckIfEnemyInRange()
        {
            // 在这里实现具体的条件判断逻辑
            // 如果有敌人在攻击范围内，返回 true，否则返回 false
            Debug.Log("有");
            return true;
        }

        // 攻击敌人
        private ENodeState AttackEnemy()
        {
            // 在这里实现具体的攻击逻辑
            // 如果攻击成功，返回 ENodeState.Success
            // 如果攻击失败，返回 ENodeState.Failed
            // 如果攻击正在进行中，返回 ENodeState.Running
            Debug.Log("攻击");
            return ENodeState.Success;
        }
    }
}