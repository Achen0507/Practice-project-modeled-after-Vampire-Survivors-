using UnityEngine;

namespace Survivor.Data
{
    /// <summary>
    /// 武器收藏系统data
    /// </summary>
    
    [CreateAssetMenu(fileName = "New CollectionItem", menuName = "Survivor/Collection Item")]
    public class CollectionItem : ScriptableObject
    {
        public string nameKey;      // 本地化 Key
        public string descKey;      // 本地化 Key
        public string aboutKey;      // 本地化 Key

        public string itemName;
        public string description;
        public string about;
        public Sprite icon;
        public bool isUnlock;
        public int collectionPointValue = 10; ///暂无用
        public string unlockConditionHint = "???";
        public ItemType itemType;
    }

    public enum ItemType {
        Weapon,
        Passive,
        Evolution,
        Secret
    }
}
