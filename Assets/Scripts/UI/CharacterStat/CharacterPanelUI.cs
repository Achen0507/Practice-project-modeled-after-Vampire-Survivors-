using Survivor.Core;
using Survivor.Data;
using Survivor.MainMenu;
using Survivor.Passives;
using Survivor.Player;
using Survivor.Upgrades;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Survivor.UI
{
    public class CharacterPanelUI : MonoBehaviour
    {
        [Header("面板")]
        [SerializeField] private GameObject panelRoot;

        [Header("属性文本")]
        [SerializeField] private Text healthText;
        [SerializeField] private Text regenText;
        [SerializeField] private Text armorText;
        [SerializeField] private Text speedText;
        [SerializeField] private Text powerText;
        [SerializeField] private Text projectileSpeedText;
        [SerializeField] private Text durationText;
        [SerializeField] private Text rangeText;
        [SerializeField] private Text cooldownText;
        [SerializeField] private Text projectileCountText;
        [SerializeField] private Text reviveText;
        [SerializeField] private Text luckText;
        [SerializeField] private Text growthText;
        [SerializeField] private Text greedText;

        [Header("列表容器")]
        [SerializeField] private Transform weaponContainer;
        [SerializeField] private Transform passiveContainer;
        [SerializeField] private GameObject listItemPrefab;

        private PlayerStats playerStats;
        private PlayerAttributes attributes;
        private WeaponManager weaponManager;
        private CharacterData currentCharacter;

        private GameObject damageTextPrefab;
        private Transform canvasTransform;

        private void Start()
        {
            panelRoot.SetActive(false);
            playerStats = FindObjectOfType<PlayerStats>();
            attributes = PlayerAttributes.Instance;
            weaponManager = WeaponManager.Instance;

            currentCharacter = GameManager.Instance?.SelectedCharacter;
        }

        private void OnEnable()
        {
            GameEvents.OnPlayerStatsChanged += RefreshPanel;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerStatsChanged -= RefreshPanel;
        }

        private void Update()
        {
            if (PauseMenu.Instance != null && PauseMenu.Instance.IsPaused) return;

            if (Input.GetKeyDown(KeyCode.Tab)) {
                bool isOpen = panelRoot.activeSelf;
                if (!isOpen)
                {
                    RefreshPanel();
                    panelRoot.SetActive(true);
                    TimeManager.Instance.AddPause();
                    AudioManager.Instance?.PauseMusic();
                }
                else {
                    panelRoot.SetActive(false);
                    TimeManager.Instance.RemovePause();
                    AudioManager.Instance?.ResumeMusic();
                }
            }
        }

        private void RefreshPanel() {
            if (playerStats == null || attributes == null) return;

            healthText.text = $"{attributes.maxHealth:F0}";
            regenText.text = $"{attributes.healthRegen:F1}/S";
            armorText.text = $"{attributes.armor:F0}%";
            speedText.text = $"{attributes.moveSpeed:F1}";
            powerText.text = $"{(attributes.power):F0}%";
            projectileSpeedText.text = $"{attributes.projectileSpeed:F0}";
            durationText.text = $"{attributes.duration:F0}S";
            rangeText.text = $"{attributes.attackRange:F0}";
            cooldownText.text = $"{(attributes.cooldownReduction):F0}%";
            projectileCountText.text = $"{attributes.projectileCount}";
            reviveText.text = $"{attributes.reviveChance}";
            luckText.text = $"{(attributes.luck ):F0}%";
            growthText.text = $"{(attributes.growth):F0}%";
            greedText.text = $"{(attributes.greed ):F0}%";

            //// 刷新武器列表
            RefreshWeaponList();

            // 刷新被动列表
            RefreshPassiveList();
        }

        private void RefreshWeaponList()
        {
            foreach (Transform child in weaponContainer)
            {
                Destroy(child.gameObject);
            }

            if (weaponManager == null) return;
            WeaponManager.Instance.RefreshWeaponList();

            foreach (var weapon in weaponManager.Weapons) {
                GameObject item = Instantiate(listItemPrefab, weaponContainer);
                WeaponListItemUI itemUI = item.GetComponent<WeaponListItemUI>();
                if (itemUI != null) {
                    Sprite icon = weaponManager.GetWeaponIcon(weapon);
                    int level = weapon.GetCurrentLevel();
                    itemUI.Initialize(icon,level);
                }
            }
        }

        private void RefreshPassiveList()
        {
            foreach (Transform child in passiveContainer)
            {
                Destroy(child.gameObject);
            }

            // 从 PassiveManager 获取被动列表
            if (PassiveManager.Instance == null) return;

            foreach(var passive in PassiveManager.Instance.Passives) {
                GameObject item = Instantiate(listItemPrefab, passiveContainer);
                WeaponListItemUI itemUI = item.GetComponent<WeaponListItemUI>();
                if (itemUI != null) {
                    Sprite icon = passive.data.icon;
                    int level = passive.level;
                    itemUI.Initialize(icon, level);
                }
            }
        }

        public void SetDamageTextPrefab(GameObject prefab, Transform canvas)
        {
            damageTextPrefab = prefab;
            canvasTransform = canvas;
        }

        public GameObject GetDamageTextPrefab() => damageTextPrefab;
        public Transform GetCanvasTransform() => canvasTransform;

    }
}
