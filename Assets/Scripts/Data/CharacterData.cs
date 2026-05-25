using UnityEngine;

namespace Survivor.Data
{
    /// <summary>
    /// 管理不同角色的data
    /// </summary>
    
    [CreateAssetMenu(fileName = "New Character", menuName = "Survivor/Character")]
    public class CharacterData : ScriptableObject
    {
        [Header("角色基础属性（选人界面显示用）")]
        public CharacterBaseStats baseStats;

        public string nameKey;      // 本地化 Key
        public string descKey;      // 本地化 Key

        public string characterName;
        public string description;
        public Sprite Weaponicon;
        public Sprite Charactericon;
        public string starterWeaponName;
        public GameObject starterWeaponPrefab;

        public StatModifier[] statModifiers;
        public bool TryGetStatModifier(StatType type, out StatModifier result)
        {
            foreach (var mod in statModifiers)
            {
                if (mod.statType == type)
                {
                    result = mod;
                    return true;
                }
            }
            result = default;
            return false;
        }
        public bool HasStatModifier(StatType type)
        {
            foreach (var mod in statModifiers)
            {
                if (mod.statType == type)
                    return true;
            }
            return false;
        }
    } 
}
