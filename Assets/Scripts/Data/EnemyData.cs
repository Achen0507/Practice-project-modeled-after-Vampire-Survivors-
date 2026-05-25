using UnityEngine;

namespace Survivor.Data
{
    /// <summary>
    /// 敌人配置数据
    /// </summary>
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Survivor/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("基础信息")]
        public string nameKey;      // 本地化 Key
        public string descKey;      // 本地化 Key

        public string enemyName;
        public string description;

        [Header("战斗属性")]
        public float baseHealth = 30f;
        public float baseSpeed = 2f;
        public float baseDamage = 10f;          // 碰撞伤害

        [Header("击杀奖励")]
        public int expValue = 5;                 // 死亡掉落的经验值
        public int goldValue = 0;                // 金币（后续扩展）

        [Header("视觉")]
        public RuntimeAnimatorController animatorController;
        public Sprite sprite;

        [Header("生成权重")]
        [Range(0, 100)]
        public int spawnWeight = 100;             // 越高越容易出现

        [Header("难度")]
        [Range(0, 2)]
        public float difficultyMultiplier = 1f;  // 难度系数，越高越容易在后期出现

        [Header("解锁时间")]
        public float unlockTime = 0f;  // 出现时间（秒），0 表示初始可用
    }
}
