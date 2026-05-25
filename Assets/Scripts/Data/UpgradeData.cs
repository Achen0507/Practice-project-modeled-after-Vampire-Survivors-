using UnityEngine;

namespace Survivor.Data
{
    /// <summary>
    /// 升级配置数据（武器升级和被动技能共用）
    /// </summary>
    
    [CreateAssetMenu(fileName = "New Upgrade", menuName = "Survivor/Upgrade Data")]
    public class UpgradeData : ScriptableObject
    {
        [Header("基础信息")]
        public string nameKey;      // 本地化 Key
        public string descKey;      // 本地化 Key

        public string upgradeName = "新升级";
        public string description;
        public Sprite icon;
        public int maxLevel;
        public int[] costPerLevel; //每级花费
        public StatType statType;      // 对应 switch 里的类型
        public bool isPercentage;      
        public float baseValue;        // 每级增加的值

        [Header("升级类型")]
        public UpgradeType upgradeType;

        [Header("武器关联")]
        public string relatedWeaponName;

        [Header("被动关联")]
        public string relatedPassiveName;

        [Header("属性加成")]
        public StatModifier[] statModifiers;      // 对角色属性的修改

        [Header("稀有度")]
        [Range(0, 100)]
        public int baseWeight = 50;               // 出现的基础权重

        [Header("前置条件")]
        public string[] prerequisiteNames;       // 需要先拥有的升级
        public int requiredLevel = 0;              // 需要玩家等级
    }

    public enum UpgradeType {
        WeaponUpgrade,      
        PassiveStat,         // 局外购买的永久被动
        PassiveSkill      // 游戏内的被动
    }

    [System.Serializable]
    public struct StatModifier {
        public StatType statType;
        public float value;          
        public bool isPercentage;    // true=百分比, false=固定值
    }

    public enum StatType {
        MaxHealth,
        HealthRegen,
        Armor,
        MoveSpeed,
        DamageBonus,
        ProjectileSpeed,
        Duration,
        AttackRange,
        CooldownReduction,
        ProjectileCount,
        Growth,
        Greed,
        Luck
    }
}
