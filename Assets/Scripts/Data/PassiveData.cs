using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor.Data
{
    [CreateAssetMenu(fileName = "New Passive", menuName = "Survivor/Passive Data")]
    public class PassiveData : ScriptableObject
    {
        [Header("基础信息")]
        public string nameKey;      // 本地化 Key
        public string descKey;      // 本地化 Key

        public string passiveName;
        public string description;
        public Sprite icon;

        [Header("属性")]
        public StatType statType;           // 影响哪个属性
        public float baseValue;              // 第1级的值
        public float growthPerLevel;         // 每级增长
        public bool isPercentage;            // 是否百分比

        [Header("限制")]
        public int maxLevel = 100;

        /// <summary>
        /// 获取指定等级的数值
        /// </summary>
        public float GetValueAtLevel(int level) {
            if (level < 0) return 0;
            return baseValue + (level - 1) * growthPerLevel;
        }
    }
}
