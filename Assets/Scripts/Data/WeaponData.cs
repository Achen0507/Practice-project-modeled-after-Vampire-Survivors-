using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor.Data
{
    /// <summary>
    /// 武器配置数据
    /// </summary>
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Survivor/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        public string nameKey;        // 本地化 Key，如 "Weapon_Knife_Name"
        public string descKey;        // 本地化 Key，如 "Weapon_Knife_Desc"

        [Header("基础信息")]
        public string weaponName = "新武器";
        public string description = "武器描述";
        public Sprite icon;

        [Header("战斗属性")]
        public float baseDamage = 10f;
        public float baseCooldown = 1f;      // 攻击间隔（秒）
        public float baseRange = 5f;          // 攻击范围
        public float baseProjectileSpeed = 5f; // 弹道速度（如果适用）

        [Header("成长数据")]
        public int maxLevel = 8;               // 最高等级

        [Header("视觉")]
        public GameObject projectilePrefab;    // 子弹/攻击特效预制体
        public GameObject extraPrefab;    //额外需要的特效预制体
        public GameObject weaponVisual;        // 武器视觉表现（可选）

        [Header("射击模式")]
        public ShotPattern[] shotPatterns;   // 每个等级的射击模式
    }

    [System.Serializable]
    public struct ShotPattern
    {
        public int projectileCount;      // 子弹数量
        public float spreadAngle;        // 散射角度（度）
        public float projectileSpeed;    // 子弹速度（0=使用默认）
        public float damageMultiplier;   // 伤害倍率
        public float width;              // 攻击宽度 ← 新增
    }
}
