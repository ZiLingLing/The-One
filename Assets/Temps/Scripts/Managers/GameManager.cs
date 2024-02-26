using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    /// <summary>
    /// ��Ϸ�����������ڿ�����Ϸ����
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public GameObject m_player;
        public GameObject prefab;

        /// <summary>
        /// BOSS�б�
        /// </summary>
        public List<GameObject> m_boss;

        /// <summary>
        /// �����б�
        /// </summary>
        public List<GameObject> m_enemys;

        /// <summary>
        /// ������Դ�б�
        /// </summary>
        public List<GameObject> m_reward;

        /// <summary>
        /// ��¼���з�����б�
        /// </summary>
        private List<GameObject> m_roomList;

        /// <summary>
        /// ��¼�Ƿ��һ��̤���Ӧ����
        /// </summary>
        private Dictionary<GameObject, bool> m_roomInfo;

        /// <summary>
        /// ��ǰ���β���
        /// </summary>
        private int m_level = 0;

        //���ɵ��ˡ�Boss�������ĸ���Ȩ��
        private float rewardWeight;
        private float enemyWeight;
        private float bossWeight;

        private void Awake()
        {

            LoadConfigData();
            prefab = Resources.Load<GameObject>("Character/Player");
        }

        private void Start()
        {
            //������ͼ
            GenerateMap();

            //���ؽ�ɫ
            Vector3 entryPosition = EventManager.TriggerEvent<Vector3>("GetEntryPoint");
            Vector3 playerPosition = new Vector3(entryPosition.x, entryPosition.y + 1.5f, entryPosition.z);
            m_player = Instantiate(prefab, playerPosition, Quaternion.identity);

            //���������
            //GameObject virtualCameraObj = new GameObject("CinemachineVirtualCamera");
            Debug.Log("��ʼ���������" + m_player.transform);
            EventManager.TriggerEvent<Transform>("SetCameraLookAt", VirtualCameraLookAt(m_player.transform));
            EventManager.TriggerEvent<Transform>("SetCameraFollow", VirtualCameraFollow(m_player.transform));
            EventManager.TriggerEvent<Vector3>("InitCamera", entryPosition);
            Debug.Log("������������");

            //�����ص��¼�
            EventManager.AddEventListener<Transform>("GetPlayerTransform", GetPlayerTransform);

            //ͨ���ֵ��Ƿ����Ƿ�����
            m_roomList = EventManager.TriggerEvent<List<GameObject>>("GetRoomList");
            m_roomInfo = new Dictionary<GameObject, bool>();
            foreach(var item in m_roomList)
            {
                m_roomInfo.Add(item, false);
                Debug.Log(item);
            }
        }

        private void Update()
        {
            EventManager.TriggerEvent<Transform>("SetCameraFollow", VirtualCameraFollow(m_player.transform));
            OnEntryRoom();
        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener<Transform>("GetPlayerTransform", GetPlayerTransform);
        }

        /// <summary>
        /// ����������Ϣ
        /// </summary>
        private void LoadConfigData()
        {

        }

        /// <summary>
        /// �����������������LookAt����
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public Transform VirtualCameraFollow(Transform lookAt)
        {
            Transform roomTransform = null;
            RaycastHit hitInfo;
            Vector3 origin = new Vector3(lookAt.transform.position.x, lookAt.transform.position.y, lookAt.transform.position.z);
            if (Physics.Raycast(origin, Vector3.down, out hitInfo))
            {
                roomTransform = hitInfo.transform;
                //Debug.Log("��⵽����" + hitInfo.transform.gameObject.name);
            }
            return roomTransform;
        }

        /// <summary>
        /// �����������������Follow����
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public Transform VirtualCameraLookAt(Transform follow)
        {
            Transform characterTransform = follow.transform;
            return characterTransform;
        }

        /// <summary>
        /// ̤��ĳ������ʱ����Ƿ��ǵ�һ�ν��룬�����������Ӧ����
        /// </summary>
        public void OnEntryRoom()
        {
            string roomTag = null;
            RaycastHit hitInfo;
            Vector3 origin = m_player.transform.position;
            if (Physics.Raycast(origin, Vector3.down, out hitInfo))
            {
                roomTag = hitInfo.transform.parent.parent.tag;
                Debug.Log(roomTag);
                if (m_roomInfo[hitInfo.transform.parent.parent.gameObject] == false)
                {
                    m_roomInfo[hitInfo.transform.parent.parent.gameObject] = true;
                    Debug.Log(roomTag== "FightRoom");
                    switch (roomTag)
                    {
                        case "StartRoom":
                            break;
                        case "FightRoom":
                            OnEntryFightRoom(Vector3.zero);
                            break;
                        case "RewardRoom":
                            OnEntryRewardRoom(Vector3.zero);
                            break;
                        case "EndRoom":
                            OnEntryEndRoom();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// ��һ��̤��ս����
        /// </summary>
        private void OnEntryFightRoom(Vector3 spawnPosition)
        {
            GameObjectPoolManager.SpawnObject(m_enemys[Random.Range(0, m_enemys.Count)], spawnPosition, Quaternion.identity);
        }

        /// <summary>
        /// ��һ��̤�뽱����
        /// </summary>
        private void OnEntryRewardRoom(Vector3 spawnPosition)
        {
            GameObjectPoolManager.SpawnObject(m_reward[Random.Range(0, m_reward.Count)], spawnPosition, Quaternion.identity);
        }

        /// <summary>
        /// ��һ��̤��Boss��
        /// </summary>
        private void OnEntryEndRoom()
        {
            if (m_level % 3 == 0)
            {
                GameObjectPoolManager.SpawnObject(m_boss[Random.Range(0, m_boss.Count)], Vector3.zero,Quaternion.identity,GameObjectPoolManager.EPoolType.None,3f,3f,3f);
            }
        }

        /// <summary>
        /// ��ȡplayer��Transform���
        /// </summary>
        /// <returns></returns>
        private Transform GetPlayerTransform()
        {
            if (m_player != null)
            {
                return m_player.transform;
            }
            Debug.LogWarning("���˻�ȡ��ɫ��TransformΪ��");
            return null;
        }


        private void GenerateMap()
        {
            m_level++;
            EventManager.TriggerEvent("GenerateMap");
        }
    }
}

