using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor.Core
{
    /// <summary>
    /// 通用对象池 - 管理需要频繁创建销毁的对象
    /// 用法：ObjectPool.Instance.Get("Bullet", position, rotation);
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance { get; private set; }

        [System.Serializable]
        public class PoolEntry
        {
            public string key;              // 唯一标识，如 "Bullet", "Enemy_Slime"
            public GameObject prefab;       // 原始预制体
            public int prewarmAmount = 10;  // 预热数量
            public bool expandable = true;  // 不够用时是否自动扩容
        }

        [SerializeField] private List<PoolEntry> poolEntries = new List<PoolEntry>();

        private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();//keymap的各种prefab队列
        private Dictionary<string, GameObject> prefabMap = new Dictionary<string, GameObject>();//keyMap预制体
        private Dictionary<GameObject, string> keyMap = new Dictionary<GameObject, string>(); // 用于归还时找到对应池子

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var entry in poolEntries)
            {
                prefabMap[entry.key] = entry.prefab;
                var queue = new Queue<GameObject>();// 创建一个空的队列，专门放 GameObject

                for (int i = 0; i < entry.prewarmAmount; i++)
                {
                    var obj = CreateNew(entry.key);
                    ReturnToPool(obj);
                }
                pools[entry.key] = queue;  // 用来存放空闲子弹的容器，把这个队列存进字典，用 key（如 "Bullet"）做钥匙
            }
        }

        private GameObject CreateNew(string key)
        {
            if (!prefabMap.TryGetValue(key, out var prefab))
            {
                Debug.LogError($"对象池中没有找到 key: {key}");
                return null;
            }

            var obj = Instantiate(prefab);
            obj.SetActive(false);
            keyMap[obj] = key;

            // 添加自动归还组件（子弹等自带生命周期的东西用）
            var returnable = obj.GetComponent<PoolReturnable>();
            if (returnable == null) returnable = obj.AddComponent<PoolReturnable>();
            returnable.SetKey(key);

            return obj;
        }

        /// <summary>
        /// 从池中获取对象
        /// </summary>
        public GameObject Get(string key, Vector3 position, Quaternion rotation)
        {
            if (!pools.TryGetValue(key, out var queue))
            {
                Debug.LogError($"对象池中没有 key: {key}");
                return null;
            }

            GameObject obj;
            
            if (queue.Count > 0)
            {
                obj = queue.Dequeue();// 从队列头部取出一个
            }
            else
            {
                // 池子空了，看是否允许扩容
                var entry = poolEntries.Find(e => e.key == key);
                if (entry != null && entry.expandable)
                {
                    obj = CreateNew(key);
                }
                else
                {
                    Debug.LogWarning($"对象池 [{key}] 已空且不允许扩容");
                    return null;
                }
            }
            
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            
            // 重置对象状态（如果有的话）
            var resettable = obj.GetComponent<IPoolResettable>();
            resettable?.OnGetFromPool();// 重置对象状态
            
            return obj;
        }

        /// <summary>
        /// 归还对象到池中
        /// </summary>
        public void ReturnToPool(GameObject obj)
        {
            if (!keyMap.TryGetValue(obj, out var key))
            {
                Destroy(obj);
                return;
            }

            obj.SetActive(false);

            if (pools.TryGetValue(key, out var queue))
            {
                queue.Enqueue(obj);
            }
            else
            {
                Destroy(obj);
            }
        }
    }

    /// <summary>
    /// 挂在需要自动归还的对象上（如子弹飞出边界后自动归还）
    /// </summary>
    public class PoolReturnable : MonoBehaviour
    {
        private string poolKey;

        public void SetKey(string key) => poolKey = key;

        public void ReturnToPool()
        {
            if (ObjectPool.Instance != null && !string.IsNullOrEmpty(poolKey))
                ObjectPool.Instance.ReturnToPool(gameObject);
            else
                Destroy(gameObject);
        }
    }

    /// <summary>
    /// 可选接口：对象从池中取出时重置状态
    /// </summary>
    public interface IPoolResettable
    {
        void OnGetFromPool();
    }
}
