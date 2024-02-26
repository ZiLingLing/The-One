using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguelike
{
    /// <summary>
    /// 游戏管理器，用于控制游戏流程
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public GameObject m_player;
        public GameObject prefab;

        /// <summary>
        /// BOSS列表
        /// </summary>
        public List<GameObject> m_boss;

        /// <summary>
        /// 敌人列表
        /// </summary>
        public List<GameObject> m_enemys;

        /// <summary>
        /// 奖励资源列表
        /// </summary>
        public List<GameObject> m_reward;

        /// <summary>
        /// 记录所有房间的列表
        /// </summary>
        private List<GameObject> m_roomList;

        /// <summary>
        /// 记录是否第一次踏入对应房间
        /// </summary>
        private Dictionary<GameObject, bool> m_roomInfo;

        /// <summary>
        /// 当前地牢层数
        /// </summary>
        private int m_level = 0;

        //生成敌人、Boss、奖励的各项权重
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
            //创建地图
            GenerateMap();

            //加载角色
            Vector3 entryPosition = EventManager.TriggerEvent<Vector3>("GetEntryPoint");
            Vector3 playerPosition = new Vector3(entryPosition.x, entryPosition.y + 1.5f, entryPosition.z);
            m_player = Instantiate(prefab, playerPosition, Quaternion.identity);

            //设置摄像机
            //GameObject virtualCameraObj = new GameObject("CinemachineVirtualCamera");
            Debug.Log("开始设置摄像机" + m_player.transform);
            EventManager.TriggerEvent<Transform>("SetCameraLookAt", VirtualCameraLookAt(m_player.transform));
            EventManager.TriggerEvent<Transform>("SetCameraFollow", VirtualCameraFollow(m_player.transform));
            EventManager.TriggerEvent<Vector3>("InitCamera", entryPosition);
            Debug.Log("摄像机设置完毕");

            //添加相关的事件
            EventManager.AddEventListener<Transform>("GetPlayerTransform", GetPlayerTransform);

            //通过字典标记房间是否进入过
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
        /// 加载配置信息
        /// </summary>
        private void LoadConfigData()
        {

        }

        /// <summary>
        /// 用于设置虚拟相机的LookAt属性
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
                //Debug.Log("检测到房间" + hitInfo.transform.gameObject.name);
            }
            return roomTransform;
        }

        /// <summary>
        /// 用于设置虚拟相机的Follow属性
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public Transform VirtualCameraLookAt(Transform follow)
        {
            Transform characterTransform = follow.transform;
            return characterTransform;
        }

        /// <summary>
        /// 踏入某个房间时检测是否是第一次进入，若是则进行相应操作
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
        /// 第一次踏入战斗房
        /// </summary>
        private void OnEntryFightRoom(Vector3 spawnPosition)
        {
            GameObjectPoolManager.SpawnObject(m_enemys[Random.Range(0, m_enemys.Count)], spawnPosition, Quaternion.identity);
        }

        /// <summary>
        /// 第一次踏入奖励房
        /// </summary>
        private void OnEntryRewardRoom(Vector3 spawnPosition)
        {
            GameObjectPoolManager.SpawnObject(m_reward[Random.Range(0, m_reward.Count)], spawnPosition, Quaternion.identity);
        }

        /// <summary>
        /// 第一次踏入Boss房
        /// </summary>
        private void OnEntryEndRoom()
        {
            if (m_level % 3 == 0)
            {
                GameObjectPoolManager.SpawnObject(m_boss[Random.Range(0, m_boss.Count)], Vector3.zero,Quaternion.identity,GameObjectPoolManager.EPoolType.None,3f,3f,3f);
            }
        }

        /// <summary>
        /// 获取player的Transform组件
        /// </summary>
        /// <returns></returns>
        private Transform GetPlayerTransform()
        {
            if (m_player != null)
            {
                return m_player.transform;
            }
            Debug.LogWarning("敌人获取角色的Transform为空");
            return null;
        }


        private void GenerateMap()
        {
            m_level++;
            EventManager.TriggerEvent("GenerateMap");
        }
    }
}

