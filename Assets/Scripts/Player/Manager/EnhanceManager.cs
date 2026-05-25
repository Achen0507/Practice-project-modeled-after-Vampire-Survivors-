using Survivor.Core;
using Survivor.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor.MainMenu
{
    public class EnhanceManager : MonoBehaviour
    {
        public static EnhanceManager Instance { get; private set; }

        private List<UpgradeData> upgrades;
        private Dictionary<UpgradeData, int> levels;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadAllUpgrades();

            LoadProgress();
        }

        private void LoadAllUpgrades()
        {
            UpgradeData[] loaded = Resources.LoadAll<UpgradeData>("Data/EnhanceData");
            System.Array.Sort(loaded, (a, b) => a.name.CompareTo(b.name));
            upgrades = new List<UpgradeData>(loaded);

            if (upgrades.Count == 0)
            {
                Debug.LogError("没有找到升级数据！请把 UpgradeData 文件放到 Assets/Resources/Upgrades/ 文件夹");
            }
        }

        public int GetLevel(UpgradeData upgrade)
        {
            return levels.ContainsKey(upgrade) ? levels[upgrade] : 0;
        }

        public int GetNextLevelCost(UpgradeData upgrade)
        {
            int level = GetLevel(upgrade);
            if (level >= upgrade.maxLevel) return -1;
            return upgrade.costPerLevel[level];
        }

        public bool CanUpgrade(UpgradeData upgrade)
        {
            int level = GetLevel(upgrade);
            if (level >= upgrade.maxLevel) return false;
            int cost = upgrade.costPerLevel[level];
            return GoldManager.Instance != null && GoldManager.Instance.GetGold() >= cost;
        }

        public float GetTotalBonus(StatType type)
        {
            float total = 0;
            foreach (var upgrade in upgrades)
            {
                if (upgrade.statType == type) 
                {
                    total += upgrade.baseValue * GetLevel(upgrade);
                }
            }
            return total;
        }

        public bool IsPercentage(StatType type)
        {
            foreach (var upgrade in upgrades)
            {
                if (upgrade.statType == type && upgrade.isPercentage)
                    return true;
            }
            return false;
        }

        public void Upgrade(UpgradeData upgrade)
        {
            if (!CanUpgrade(upgrade)) return;

            int level = GetLevel(upgrade);
            if (level >= upgrade.maxLevel) return;

            int cost = upgrade.costPerLevel[level];
            if (GoldManager.Instance != null)
                GoldManager.Instance.SpendGold(cost);

            levels[upgrade] = level + 1;

            PlayerAttributes.Instance.ApplyStatModifier(upgrade.statType, upgrade.baseValue, upgrade.isPercentage);

            SaveProgress();

            GameEvents.PlayerStatsChanged();
            EnhanceUI.Instance?.Refresh();
        }

        public List<UpgradeData> GetAllUpgrades()
        {
            return upgrades;
        }

        public float GetCurrentValue(UpgradeData upgrade)
        {
            return upgrade.baseValue * GetLevel(upgrade);
        }

        public float GetNextValue(UpgradeData upgrade)
        {
            return upgrade.baseValue * (GetLevel(upgrade) + 1);
        }

        private void LoadProgress()
        {
            levels = new Dictionary<UpgradeData, int>();
            if (upgrades == null) return;

            foreach (var up in upgrades)
            {
                int value = PlayerPrefs.GetInt($"Enhance_{up.name}", 0);
                levels[up] = value;
            }
        }

        private void SaveProgress()
        {
            foreach (var kvp in levels)
            {
                PlayerPrefs.SetInt($"Enhance_{kvp.Key.name}", kvp.Value);
            }
            PlayerPrefs.Save();
        }

        public void ApplyToAttributes()
        {
            foreach (var upgrade in upgrades)
            {
                int level = GetLevel(upgrade);
                if (level > 0)
                {
                    float totalValue = upgrade.baseValue * level;
                    PlayerAttributes.Instance.ApplyStatModifier(upgrade.statType, totalValue, upgrade.isPercentage);
                }
            }
        }
    }
}