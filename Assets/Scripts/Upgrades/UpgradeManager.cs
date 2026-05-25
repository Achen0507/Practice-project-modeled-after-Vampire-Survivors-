using Survivor.Combat;
using Survivor.Core;
using Survivor.Data;
using Survivor.Passives;
using Survivor.Player;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor.Upgrades
{
    /// <summary>
    /// 3选1管理
    /// </summary>
    public class UpgradeManager : MonoBehaviour
    {
        public static UpgradeManager Instance { get; private set; }
        private bool isWaitingForChoice = false;
        private int pendingLevelUps = 0; 

        [Header("升级池")]
        private List<UpgradeData> availableUpgrades;

        [Header("UI 面板")]
        [SerializeField] private UpgradePanelUI upgradePanelUI;

        private List<UpgradeData> currentOptions = new List<UpgradeData>();
        public List<UpgradeData> obtainedUpgrades = new List<UpgradeData>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            LoadAllUpgrades();
        }

        private void LoadAllUpgrades()
        {
            UpgradeData[] loaded = Resources.LoadAll<UpgradeData>("Data/Upgrade");
            availableUpgrades = new List<UpgradeData>(loaded);

            if (availableUpgrades.Count == 0)
            {
                Debug.LogError("没有找到升级数据！请把 UpgradeData 文件放到 Assets/Resources/Upgrades/ 文件夹");
            }
        }

        private void OnEnable()
        {
            GameEvents.OnPlayerLevelUp += OnLevelUp;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerLevelUp -= OnLevelUp;
        }

        public List<UpgradeData> GetCurrentOptions()
        {
            return currentOptions;
        }

        private void OnLevelUp(int newLevel)
        {
            pendingLevelUps++;

            if (!isWaitingForChoice)
            {
                ShowNextUpgrade();
            }
        }

        private void ShowNextUpgrade()
        {
            if (pendingLevelUps <= 0) return;

            pendingLevelUps--;
            isWaitingForChoice = true;

            currentOptions.Clear();
            int optionCount = PlayerAttributes.Instance?.GetUpgradeOptionCount() ?? 3;
            currentOptions.AddRange(GetRandomUpgrades(optionCount));

            if (upgradePanelUI != null)
            {
                upgradePanelUI.ShowUpgradePanel(currentOptions);
            }
        }

        private List<UpgradeData> GetRandomUpgrades(int count)
        {
            if (availableUpgrades.Count == 0)
            {
                Debug.LogError("没有配置任何升级项！");
                return new List<UpgradeData>();
            }

            List<UpgradeData> validOptions = new List<UpgradeData>();
            foreach (var upgrade in availableUpgrades)
            {
                if (IsUpgradeAvailable(upgrade))
                {
                    validOptions.Add(upgrade);
                }
            }

            // 随机抽取
            List<UpgradeData> results = new List<UpgradeData>();
            for (int i = 0; i < count && validOptions.Count > 0; i++)
            {
                int index = Random.Range(0, validOptions.Count);
                results.Add(validOptions[index]);
                validOptions.RemoveAt(index);
            }

            return results;
        }

        private bool IsUpgradeAvailable(UpgradeData upgrade)
        {
            // 武器升级
            if (upgrade.upgradeType == UpgradeType.WeaponUpgrade && !string.IsNullOrEmpty(upgrade.relatedWeaponName))
            {
                WeaponData weaponData = Resources.Load<WeaponData>($"Data/WeaponData/{upgrade.relatedWeaponName}");
                WeaponBase weapon = FindWeaponByData(weaponData);
                if (weapon != null)
                {
                    // 已有武器：检查是否满级
                    return weapon.GetCurrentLevel() < weapon.GetWeaponData().maxLevel;
                }
                else
                {
                    // 新武器：检查武器槽是否已满
                    return WeaponManager.Instance != null && WeaponManager.Instance.CanAddNewWeapon();
                }
            }

            // 被动技能
            if (upgrade.upgradeType == UpgradeType.PassiveSkill && !string.IsNullOrEmpty(upgrade.relatedPassiveName))
            {
                PassiveData passiveData = Resources.Load<PassiveData>($"Data/PassiveData/{upgrade.relatedPassiveName}");
                int currentLevel = PassiveManager.Instance?.GetPassiveLevel(passiveData)??0;

                if (currentLevel == 0)
                {
                    // 新被动：检查是否已满 6 个
                    if (PassiveManager.Instance?.Passives.Count >= 6)
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    // 已有被动：检查是否满级
                    return currentLevel < passiveData.maxLevel;
                }
            }
            return true;
        }

        /// <summary>
        /// 玩家选择了某个升级
        /// </summary>
        public void SelectUpgrade(UpgradeData upgrade)
        {
            ApplyUpgrade(upgrade);
            GameEvents.UpgradeSelected(upgrade);

            isWaitingForChoice = false;

            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            playerStats?.PlayLevelUpEffect();
        }

        private void ApplyUpgrade(UpgradeData upgrade)
        {
            switch (upgrade.upgradeType)
            {
                case UpgradeType.WeaponUpgrade:
                    WeaponData weaponData = Resources.Load<WeaponData>($"Data/WeaponData/{upgrade.relatedWeaponName}");
                    WeaponBase weapon = FindWeaponByData(weaponData);
                    if (weapon != null)
                    {
                        weapon.Upgrade();
                    }
                    else{
                        // 新武器：添加到武器管理器
                        AddNewWeapon(weaponData);
                    }
                    break;
                case UpgradeType.PassiveStat:
                    obtainedUpgrades.Add(upgrade);
                    if (PlayerAttributes.Instance != null)
                    {
                        foreach (var modifier in upgrade.statModifiers)
                        {
                            PlayerAttributes.Instance.ApplyStatModifier(modifier.statType, modifier.value, modifier.isPercentage);
                        }
                    }
                    break;
                case UpgradeType.PassiveSkill:
                    if (!string.IsNullOrEmpty(upgrade.relatedPassiveName) && PassiveManager.Instance != null)
                    {
                        PassiveData passiveData = Resources.Load<PassiveData>($"Data/PassiveData/{upgrade.relatedPassiveName}");
                        if (passiveData != null)
                        {
                            PassiveManager.Instance.AddPassive(passiveData);
                        }
                    }
                    break;
            }
        }

        private WeaponBase FindWeaponByData(WeaponData data)
        {
            if (WeaponManager.Instance != null)
            {
                foreach (var weapon in WeaponManager.Instance.Weapons)
                {
                    if (weapon.GetWeaponData() != null &&
                        weapon.GetWeaponData().weaponName == data.weaponName)
                        return weapon;
                }
            }
            return null;
        }

        private void AddNewWeapon(WeaponData data) {
            if (!WeaponManager.Instance.CanAddNewWeapon()) return;

            GameObject weaponObj = new GameObject(data.weaponName);
            weaponObj.transform.SetParent(WeaponManager.Instance.transform);
            weaponObj.transform.localPosition = Vector3.zero;  

            WeaponBase weapon = null;

            switch (data.weaponName)
            {
                case "爱之箭":
                    weapon = weaponObj.AddComponent<KnifeWeapon>();
                    var knife = weapon as KnifeWeapon;
                    if (knife != null && data.projectilePrefab != null)
                    {
                        knife.SetKnifePrefab(data.projectilePrefab);
                    }
                    break;
                case "守护光环":
                    weapon = weaponObj.AddComponent<OrbitWeapon>();
                    var orbit = weapon as OrbitWeapon;
                    if (orbit != null && data.projectilePrefab != null)
                    {
                        orbit.SetOrbitPrefab(data.projectilePrefab);
                    }
                    break;
                case "神圣激光":
                    weapon = weaponObj.AddComponent<HeavenStrike>();
                    var heavenStrike = weapon as HeavenStrike;
                    if (heavenStrike != null && data.projectilePrefab != null)
                    {
                        heavenStrike.SetWarning(data.extraPrefab);
                        heavenStrike.SetHeavenStrike(data.projectilePrefab);
                    }
                    break;
                case "剑气":
                    weapon = weaponObj.AddComponent<WhipWeapon>();
                    var whip = weapon as WhipWeapon;
                    if (whip != null && data.projectilePrefab != null)
                    {
                        whip.SetWhip(data.projectilePrefab);
                    }
                    break;
                case "火球":
                    weapon = weaponObj.AddComponent<FireWand>();
                    var fireWand = weapon as FireWand;
                    if (fireWand != null && data.projectilePrefab != null)
                    {
                        fireWand.SetFireWand(data.projectilePrefab);
                    }
                    break;
                default:
                    Debug.LogError($"未知武器: {data.weaponName}");
                    Destroy(weaponObj);
                    return;
            }
            weapon.SetWeaponData(data);
            weapon.RecordFirstGetTime(Time.timeSinceLevelLoad);
            WeaponManager.Instance.AddWeapon(weapon);
        }

        public void OnPanelClosed()
        {
            isWaitingForChoice = false;

            // 检查是否还有待处理的升级
            if (pendingLevelUps > 0)
            {
                ShowNextUpgrade();
            }
        }
    }
}
