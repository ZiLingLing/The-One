using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace Roguelike
{
    public class MapGenerate : MonoBehaviour
    {

        #region 地图组成
        /// <summary>
        /// 存放所有已生成房间的列表
        /// </summary>
        private List<GameObject> m_roomList;

        /// <summary>
        /// 存放所有已生成房间的墙壁
        /// </summary>
        private List<GameObject> m_wallList;

        private enum ERoomType
        {
            Entry,
            End,
            Fight,
            Reward,
        }

        /// <summary>
        /// 战斗房间列表
        /// </summary>
        public List<GameObject> m_fightRoomList;

        /// <summary>
        /// Boss房列表
        /// </summary>
        public List<GameObject> m_endRoomList;

        /// <summary>
        /// 奖励房列表
        /// </summary>
        public List<GameObject> m_rewardRoomList;

        /// <summary>
        /// 入口房间列表
        /// </summary>
        public List<GameObject> m_entryRoomList;

        /// <summary>
        /// 有门框的墙，用于房间间的连接
        /// </summary>
        public GameObject m_wallWithDoor;

        /// <summary>
        /// 没有门框的墙
        /// </summary>
        public GameObject m_wallWithoutDoor;

        #endregion

        /// <summary>
        /// 房间数量及其范围
        /// </summary>
        public int m_minRoomCount = 7;
        public int m_maxRoomCount = 10;
        private int m_roomCount;
        private int m_currentRoomCount = 0;
        private int m_entryRoomCount = 1;
        private int m_endRoomCount = 1;
        [SerializeField,Tooltip("战斗房数量")]
        private int m_fightRoomCount;
        [SerializeField,Tooltip("奖励房数量")]
        private int m_rewardRoomCount = 1;

        /// <summary>
        /// 初始房间位置
        /// </summary>
        private Vector3 m_spawnPoint=Vector3.zero;

        /// <summary>
        /// 横纵方向的响铃房间偏移量
        /// </summary>
        public float m_verticalOffset;
        public float m_horizontalOffset;

        /// <summary>
        /// 记录房间信息的字典
        /// </summary>
        private Dictionary<Vector3, ERoomType> m_roomDic = new Dictionary<Vector3, ERoomType>();

        /// <summary>
        /// 记录墙位置信息的字典,key为墙壁位置,value为是否带门
        /// </summary>
        private Dictionary<Vector3, bool> m_wallDic = new Dictionary<Vector3, bool>();

        /// <summary>
        /// 记录墙壁是否旋转的字典,key为墙壁位置,value为是否旋转90度
        /// </summary>
        private Dictionary<Vector3, bool> m_wallRotate = new Dictionary<Vector3, bool>();

        /// <summary>
        /// 房间生成方向
        /// </summary>
        private enum EGenerateDirection
        {
            Front=0,
            Right,
            Back,
            Left,
        }
        private int m_bannedGenerateDirection;

        #region 生命周期函数

        private void OnEnable()
        {
            EventManager.AddEventListener("GenerateMap", GenerateMap);
            EventManager.AddEventListener<Vector3>("GetEntryPoint", GetEntryRoomPosition);
            EventManager.AddEventListener<List<GameObject>>("GetRoomList", GetRoomList);
        }

        private void OnDisable()
        {
            EventManager.RemoveEventListener("GenerateMap", GenerateMap);
            EventManager.RemoveEventListener<Vector3>("GetEntryPoint", GetEntryRoomPosition);
            EventManager.RemoveEventListener<List<GameObject>>("GetRoomList", GetRoomList);
        }

        #endregion

        /// <summary>
        /// 单例实现
        /// </summary>
        //private static MapGenerate s_instance;

        //public static MapGenerate Instance
        //{
        //    get
        //    {
        //        s_instance = FindObjectOfType<MapGenerate>();
        //        if (s_instance == null)
        //        {
        //            s_instance = new MapGenerate();
        //        }
        //        return s_instance;
        //    }
        //}

        //private MapGenerate()
        //{

        //}

        /// <summary>
        /// 生成地图
        /// </summary>
        public void GenerateMap()
        {
            Init();
            //生成第一个房间，第一个房间必定为战斗房
            //战斗房间连接起始房
            //记录从第一个战斗房到出生房的方向
            //这个方向不会成为后续房间的生成方向
            GameObject mapRoot = GameObject.Find("MapRoot");
            if (mapRoot==null)
            {
                mapRoot = new GameObject("MapRoot");
                //Instantiate(mapRoot, m_spawnPoint, Quaternion.identity);
            }
            //生成第一个房间以及与它相连的起始房间，获得被禁止生成房间的方向
            m_roomDic.Add(m_spawnPoint, ERoomType.Fight);
            m_bannedGenerateDirection = UnityEngine.Random.Range(0, Enum.GetValues(typeof(EGenerateDirection)).Length);
            Vector3 entryPoint = RoomPointAfterOffset(m_spawnPoint, m_bannedGenerateDirection);
            Debug.Log(entryPoint);
            m_roomDic.Add(entryPoint, ERoomType.Entry);
            m_currentRoomCount += 2;

            //生成房间
            DefineRoomInfo(m_spawnPoint);
            GenerateRoom(m_roomDic);

            //生成墙壁
            DefineWallInfo();
            GenerateWall(m_wallDic, m_wallRotate);

            //碰上极端情况，比如第一个房间剩下的三个方向都没有生成房间，这类没有生成足够房间的情况，就重新生成
            if (m_currentRoomCount != m_roomCount)
            {
                GenerateMap();
            }

        }

        /// <summary>
        /// 初始化地图的各项数据
        /// </summary>
        private void Init()
        {
            if(m_roomList == null || m_wallList == null)
            {
                m_roomList = new List<GameObject>();
                m_wallList = new List<GameObject>();
            }
            else
            {
                m_roomDic.Clear();
                m_wallDic.Clear();
                m_wallRotate.Clear();
                for (int i = m_roomList.Count - 1; i >= 0; i--)
                {
                    Destroy(m_roomList[i]);
                    m_roomList.RemoveAt(i);
                }
                for (int i = m_wallList.Count - 1; i >= 0; i--)
                {
                    Destroy(m_wallList[i]);
                    m_wallList.RemoveAt(i);
                }
            }
            m_roomCount = Random.Range(m_minRoomCount, m_maxRoomCount);
            m_currentRoomCount = 0;
            //m_fightRoomCount = m_roomCount - m_rewardRoomCount - m_endRoomCount - m_entryRoomCount;
        }

        /// <summary>
        /// 依照记录房间信息的字典生成房间
        /// </summary>
        /// <param name="roomPoint"></param>
        private void GenerateRoom(Dictionary<Vector3,ERoomType> dic)
        {
            Debug.Log(dic.Count+"--"+m_roomCount);
            foreach(var roomInfo in dic)
            {
                GameObject room = null;
                switch (roomInfo.Value)
                {
                    case ERoomType.Fight:
                        room = Instantiate(m_fightRoomList[UnityEngine.Random.Range(0, m_fightRoomList.Count)], roomInfo.Key, Quaternion.identity, GameObject.Find("MapRoot").transform);
                        break;
                    case ERoomType.Entry:
                        room = Instantiate(m_entryRoomList[UnityEngine.Random.Range(0, m_entryRoomList.Count)], roomInfo.Key, Quaternion.identity, GameObject.Find("MapRoot").transform);
                        break;
                    case ERoomType.End:
                        room = Instantiate(m_endRoomList[UnityEngine.Random.Range(0, m_endRoomList.Count)], roomInfo.Key, Quaternion.identity, GameObject.Find("MapRoot").transform);
                        break;
                    case ERoomType.Reward:
                        room = Instantiate(m_rewardRoomList[UnityEngine.Random.Range(0, m_rewardRoomList.Count)], roomInfo.Key, Quaternion.identity, GameObject.Find("MapRoot").transform);
                        break;
                    default:
                        Debug.LogWarning("存在不明类型的房间，位置在" + roomInfo.Key);
                        room = Instantiate(m_fightRoomList[UnityEngine.Random.Range(0, m_fightRoomList.Count)], roomInfo.Key, Quaternion.identity, GameObject.Find("MapRoot").transform);
                        break;
                }
                m_roomList.Add(room);

            }
        }

        /// <summary>
        /// 确定所有房间的位置
        /// </summary>
        /// <param name="startPoint"></param>
        private void DefineRoomInfo(Vector3 startPoint)
        {
            Queue<Vector3> points = new Queue<Vector3>();
            points.Enqueue(startPoint);
            int rewardRoomNumber = UnityEngine.Random.Range(3, m_roomCount - 1);
            while (points.Count > 0)
            {
                //键为生成的方向，值为生成概率
                Dictionary<int, float> orders = DefineOrder(m_bannedGenerateDirection);
                Vector3 oldPoint = points.Peek();
                foreach (var order in orders)
                {
                    if (m_currentRoomCount < m_roomCount)
                    {
                        //如果满足条件：该点被标记为待生成状态，并且该点的位置之前没有房间
                        if(TryAddRoom(order.Value) == true && m_roomDic.ContainsKey(RoomPointAfterOffset(oldPoint,order.Key))==false)
                        {
                            Vector3 newPoint = RoomPointAfterOffset(oldPoint, order.Key);
                            points.Enqueue(newPoint);
                            if(m_currentRoomCount == m_roomCount - 1)
                            {
                                m_roomDic.Add(newPoint, ERoomType.End);
                            }
                            else if(m_currentRoomCount == rewardRoomNumber - 1)
                            {
                                m_roomDic.Add(newPoint, ERoomType.Reward);
                            }
                            else
                            {
                                m_roomDic.Add(newPoint, ERoomType.Fight);
                            }      
                            m_currentRoomCount++;
                        }
                    }

                }
                points.Dequeue();
            }
        }

        /// <summary>
        /// 确定该房间的房间生成顺序以及生成概率
        /// </summary>
        /// <param name="bannedDirection"></param>
        /// <returns></returns>
        private Dictionary<int,float> DefineOrder(int bannedDirection)
        {
            Dictionary<int, float> order = new Dictionary<int, float>();
            List<int> directions = new List<int>(new int[] { 0, 1, 2, 3 });
            while (directions.Count > 0)
            {
                //每经过一次随机少一个方向
                int direction = directions[UnityEngine.Random.Range(0, directions.Count)];
                directions.Remove(direction);
                if (direction == bannedDirection)
                {
                    continue;
                }

                order.Add(direction,UnityEngine.Random.Range(0,1f));
            }
            return order;
        }

        /// <summary>
        /// 该位置是否要标记为待生成状态
        /// </summary>
        /// <returns></returns>
        private bool TryAddRoom(float probability)
        {
            return UnityEngine.Random.Range(0, 1f) >= probability ? true : false;
        }

        /// <summary>
        /// 从当前房间延展地图，获得下一个房间的位置
        /// </summary>
        /// <param name="oldPoint"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private Vector3 RoomPointAfterOffset(Vector3 oldPoint,int direction)
        {
            Vector3 newPoint=Vector3.zero;
            EGenerateDirection Edirection;
            switch (direction)
            {
                case 0:
                    Edirection = EGenerateDirection.Front;
                    break;
                case 1:
                    Edirection = EGenerateDirection.Right;
                    break;
                case 2:
                    Edirection = EGenerateDirection.Back;
                    break;
                case 3:
                    Edirection = EGenerateDirection.Left;
                    break;
                default:
                    Edirection = EGenerateDirection.Front;
                    Debug.LogError("进行地图延展时新房间的方向出错");
                    break;
            }
            switch (Edirection)
            {
                case EGenerateDirection.Front:
                        newPoint = new Vector3(oldPoint.x, oldPoint.y, oldPoint.z + m_verticalOffset);
                    break;
                case EGenerateDirection.Right:
                        newPoint = new Vector3(oldPoint.x + m_horizontalOffset, oldPoint.y, oldPoint.z);
                    break;
                case EGenerateDirection.Back:
                        newPoint = new Vector3(oldPoint.x, oldPoint.y, oldPoint.z - m_verticalOffset);
                    break;
                case EGenerateDirection.Left:
                        newPoint = new Vector3(oldPoint.x - m_horizontalOffset, oldPoint.y, oldPoint.z);
                    break;
                default:
                    Debug.LogWarning("有个房间对应了null");
                    break;
            }
            return newPoint;
        }

        /// <summary>
        /// 定义墙壁信息
        /// </summary>
        private void DefineWallInfo()
        {
            foreach(var roomInfo in m_roomDic)
            {
                for(int i = 0; i < 4; i++)
                {
                    Vector3 wallPoint = WallPointAfterOffset(roomInfo.Key, i);
                    if(m_wallDic.ContainsKey(wallPoint) == false)
                    {
                        if (JudgeWallType(roomInfo.Key, i))
                        {
                            m_wallDic.Add(wallPoint, true);
                        }
                        else
                        {
                            m_wallDic.Add(wallPoint, false);
                        }
                    }
                    if (m_wallRotate.ContainsKey(wallPoint) == false)
                    {
                        if(i==0 || i == 2)
                        {
                            m_wallRotate.Add(wallPoint, true);
                        }
                        else
                        {
                            m_wallRotate.Add(wallPoint, false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 生成墙壁物体
        /// </summary>
        /// <param name="dic"></param>
        private void GenerateWall(Dictionary<Vector3, bool> vectorDic,Dictionary<Vector3, bool> rotationDic)
        {
            GameObject wall = null;
            foreach(var wallInfo in vectorDic)
            {
                if (wallInfo.Value == true)
                {
                    wall = Instantiate(m_wallWithDoor, wallInfo.Key, Quaternion.identity, GameObject.Find("MapRoot").transform);
                }
                else
                {
                    wall = Instantiate(m_wallWithoutDoor, wallInfo.Key, Quaternion.identity, GameObject.Find("MapRoot").transform);
                }
                if (rotationDic[wallInfo.Key] == false)
                {
                    wall.transform.Rotate(new Vector3(0, 90f, 0));
                }
                m_wallList.Add(wall);
            }
            //foreach(var wallInfo in rotationDic)
            //{
            //    if (rotationDic[wallInfo.Key] == false)
            //    {
            //        wall.transform.Rotate(new Vector3(0, 90f, 0));
            //    }
            //}
        }

        /// <summary>
        /// 判断相邻位置上是否有房间
        /// </summary>
        /// <param name="roomPoint"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private bool JudgeWallType(Vector3 roomPoint,int direction)
        {
            if (m_roomDic.ContainsKey(RoomPointAfterOffset(roomPoint, direction)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 从当前房间延展地图，获得该方向上墙壁的位置
        /// </summary>
        /// <param name="oldPoint"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private Vector3 WallPointAfterOffset(Vector3 roomPoint, int direction)
        {
            Vector3 wallPoint = Vector3.zero;
            EGenerateDirection Edirection;
            switch (direction)
            {
                case 0:
                    Edirection = EGenerateDirection.Front;
                    break;
                case 1:
                    Edirection = EGenerateDirection.Right;
                    break;
                case 2:
                    Edirection = EGenerateDirection.Back;
                    break;
                case 3:
                    Edirection = EGenerateDirection.Left;
                    break;
                default:
                    Edirection = EGenerateDirection.Front;
                    Debug.LogError("进行地图延展时新房间的方向出错");
                    break;
            }
            switch (Edirection)
            {
                case EGenerateDirection.Front:
                    wallPoint = new Vector3(roomPoint.x, roomPoint.y, roomPoint.z + m_verticalOffset * 0.5f);
                    break;
                case EGenerateDirection.Right:
                    wallPoint = new Vector3(roomPoint.x + m_horizontalOffset * 0.5f, roomPoint.y, roomPoint.z);
                    break;
                case EGenerateDirection.Back:
                    wallPoint = new Vector3(roomPoint.x, roomPoint.y, roomPoint.z - m_verticalOffset * 0.5f);
                    break;
                case EGenerateDirection.Left:
                    wallPoint = new Vector3(roomPoint.x - m_horizontalOffset * 0.5f, roomPoint.y, roomPoint.z);
                    break;
                default:
                    Debug.LogWarning("有个房间对应了null");
                    break;
            }
            return wallPoint;
        }

        /// <summary>
        /// 获取EntryRoom的位置
        /// </summary>
        /// <returns></returns>
        private Vector3 GetEntryRoomPosition()
        {
            Vector3 entryPosition = Vector3.zero;
            foreach (var roomInfo in m_roomDic)
            {
                if (roomInfo.Value == ERoomType.Entry)
                {
                    entryPosition = roomInfo.Key;
                    break;
                }
            }
            return entryPosition;
        }

        /// <summary>
        /// 获取房间列表
        /// </summary>
        /// <returns></returns>
        private List<GameObject> GetRoomList()
        {
            if (m_roomList != null)
            {
                return m_roomList;
            }
            Debug.LogWarning("房间列表为空");
            return null;
        }
    }
}
