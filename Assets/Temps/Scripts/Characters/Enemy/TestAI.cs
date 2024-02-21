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
            // ������Ϊ���ڵ�
            BehaviourTreeBaseNode root = new BehaviourTreeSequenceNodeOneFrameOneChild();

            // ����ӽڵ�
            root.AddChild(
                new BehaviourTreeConditionNode(CheckIfEnemyInRange),
                new BehaviourTreeActionNode(AttackEnemy)
            );

            return root;
        }

        // ����Ƿ��е����ڹ�����Χ��
        private bool CheckIfEnemyInRange()
        {
            // ������ʵ�־���������ж��߼�
            // ����е����ڹ�����Χ�ڣ����� true�����򷵻� false
            Debug.Log("��");
            return true;
        }

        // ��������
        private ENodeState AttackEnemy()
        {
            // ������ʵ�־���Ĺ����߼�
            // ��������ɹ������� ENodeState.Success
            // �������ʧ�ܣ����� ENodeState.Failed
            // ����������ڽ����У����� ENodeState.Running
            Debug.Log("����");
            return ENodeState.Success;
        }
    }
}