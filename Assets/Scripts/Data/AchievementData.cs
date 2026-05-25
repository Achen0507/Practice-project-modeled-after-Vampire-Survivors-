using UnityEngine;

namespace Survivor.Data
{
    /// <summary>
    /// 管理成就的data
    /// </summary>
    
    [CreateAssetMenu(fileName = "New Achievement", menuName = "Survivor/Achievement")]
    public class AchievementData : ScriptableObject
    {
        public string nameKey;      // 本地化 Key -汉化用
        public string descKey;      // 本地化 Key -汉化用

        public string achievementName;
        public string description;
        public Sprite icon;
        public Sprite completedIcon;      // 完成时显示的图标
        public Sprite incompleteIcon;     // 未完成时显示图标
        public int targetValue;
        public AchievementType type;
        public string enemyName;
    }

    public enum AchievementType
    {
        TotalKills,
        TotalGames,
        MaxLevel,
        WeaponsUnlocked,
        PlayTime
    }
}
