using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor.Data
{
    [CreateAssetMenu(fileName = "New Achievement", menuName = "Survivor/Achievement")]
    public class AchievementData : ScriptableObject
    {
        public string nameKey;      // 本地化 Key
        public string descKey;      // 本地化 Key

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
