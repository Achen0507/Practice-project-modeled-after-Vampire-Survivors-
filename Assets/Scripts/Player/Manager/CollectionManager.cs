using Survivor.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor.MainMenu
{
    public class CollectionManager : MonoBehaviour
    {
        public static CollectionManager Instance { get; private set; }
        private List<CollectionItem> allItems;


        private Dictionary<string, bool> unlockedStatus;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadAllCollectionItems();
        }

        private void Start()
        {
            LoadUnlockStatus();
            foreach (var item in allItems)
            {
                Debug.Log($"藏品: {item.itemName}, 已解锁: {IsUnlocked(item)}");
            }
        }

        private void LoadAllCollectionItems()
        {
            CollectionItem[] loaded = Resources.LoadAll<CollectionItem>("Data/CollectionData");
            System.Array.Sort(loaded, (a, b) => a.name.CompareTo(b.name));  
            allItems = new List<CollectionItem>(loaded);

            if (allItems.Count == 0)
            {
                Debug.LogError("没有找到收藏品数据！请把 CollectionItem 文件放到 Assets/Resources/Collections/ 文件夹");
            }
        }

        public void UnlockItem(CollectionItem item) {
            if (item == null) return;

            Debug.Log($"=== 解锁尝试 ===");
            Debug.Log($"item.itemName: '{item.itemName}'");
            Debug.Log($"unlockedStatus 中是否存在: {unlockedStatus.ContainsKey(item.itemName)}");
            if (unlockedStatus.ContainsKey(item.itemName))
            {
                Debug.Log($"当前状态: {unlockedStatus[item.itemName]}");
            }

            if (unlockedStatus.ContainsKey(item.itemName) && unlockedStatus[item.itemName])
            {
                Debug.Log($"收藏品 {item.itemName} 已解锁，跳过执行！");
                return;
            }

            unlockedStatus[item.itemName] = true;
            item.isUnlock = true;

            // 增加收藏点数
            GameManager.Instance?.AddCollectionPoints(item.collectionPointValue);

            // 触发 UI 提示
            Debug.Log($"收藏品已解锁: {item.itemName}");
            SaveUnlockStatus();
        }

        public bool IsUnlocked(CollectionItem item)
        {
            if (item == null) return false;
            return unlockedStatus.ContainsKey(item.itemName) && unlockedStatus[item.itemName];
        }

        public int GetUnlockedCount() {
            int count = 0;
            foreach (var item in allItems)
            {
                if (IsUnlocked(item)) count++;
            }
            return count;
        }

        private void LoadUnlockStatus() {
            unlockedStatus = new Dictionary<string, bool>();
            foreach (var item in allItems)
            {
                int savedValue = PlayerPrefs.GetInt($"Unlocked_{item.itemName}", 0);
                bool status = savedValue == 1;
                unlockedStatus[item.itemName] = status;
                Debug.Log($"加载收藏品 {item.itemName}: savedValue={savedValue}, status={status}");
            }
        }

        private void SaveUnlockStatus() {
            foreach (var item in allItems)
            {
                PlayerPrefs.SetInt($"Unlocked_{item.itemName}", unlockedStatus[item.itemName] ? 1 : 0);
                Debug.Log($"解锁收藏: {item.itemName}");
            }
            ;
            PlayerPrefs.Save();
        }

        public List<CollectionItem> GetAllItems()
        {
            return allItems;
        }
    }
}
