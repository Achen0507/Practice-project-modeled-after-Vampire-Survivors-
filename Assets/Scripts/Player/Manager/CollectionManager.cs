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
            GameManager.Instance?.AddCollectionPoints(item.collectionPointValue);
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
            }
        }

        private void SaveUnlockStatus() {
            foreach (var item in allItems)
            {
                PlayerPrefs.SetInt($"Unlocked_{item.itemName}", unlockedStatus[item.itemName] ? 1 : 0);
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
