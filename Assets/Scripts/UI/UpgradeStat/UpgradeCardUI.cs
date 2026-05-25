using Survivor.Data;
using Survivor.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    [Header("UI 组件")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text newFlag;

    private UpgradeData upgradeData;
    private Action<UpgradeData> onSelectCallback;

    public void Initialize(UpgradeData data, Action<UpgradeData> callback) {
        upgradeData = data;
        onSelectCallback = callback;

        string localizedName = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", data.nameKey);
        string localizedDesc = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", data.descKey);

        iconImage.sprite = data.icon;
        nameText.text = localizedName;
        descriptionText.text = localizedDesc;

        if (data.upgradeType == UpgradeType.WeaponUpgrade)
        {
            int currentLevel = GetWeaponCurrentLevel(data);
            if (currentLevel == 0)
            {
                // 新武器
                newFlag.text = "NEW!";
                newFlag.gameObject.SetActive(true);
                levelText.text = "";
            }
            else
            {
                // 已有武器升级
                levelText.text = $"LEVEL {currentLevel + 1}";
                newFlag.gameObject.SetActive(false);
            }
        }
        else {
            levelText.text = "";
            newFlag.gameObject.SetActive(false);
        }
    }

    private int GetWeaponCurrentLevel(UpgradeData data) {
        if (string.IsNullOrEmpty(data.relatedWeaponName)) return 0;

        if (WeaponManager.Instance != null) {
            foreach (var weapon in WeaponManager.Instance.Weapons)
            {
                if (weapon.GetWeaponData() != null &&
                weapon.GetWeaponData().weaponName == data.relatedWeaponName)
                {
                    return weapon.GetCurrentLevel();
                }
            }
        }
        return 0;
    }

    public void OnClick() {
        AudioManager.Instance?.PlayUpgradeSelect();
        onSelectCallback?.Invoke(upgradeData);
    }
}
