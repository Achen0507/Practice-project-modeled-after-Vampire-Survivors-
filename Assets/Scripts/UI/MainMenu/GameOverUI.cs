using Survivor.Combat;
using Survivor.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Survivor.UI
{
    /// <summary>
    /// 结算面板
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        public static GameOverUI Instance { get; private set; }

        [Header("面板")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Text titleText;

        [Header("左侧固定数据")]
        [SerializeField] private Text timeText;
        [SerializeField] private Text goldText;
        [SerializeField] private Text killText;
        [SerializeField] private Text levelText;

        [Header("右侧道具列表")]
        [SerializeField] private Transform collectionContainer;
        [SerializeField] private GameObject collectionItemPrefab;

        [Header("左侧武器")]
        [SerializeField] private Transform weaponContainer;
        [SerializeField] private GameObject weaponItemPrefab;

        [Header("按钮")]
        [SerializeField] private Button achievementButton;
        [SerializeField] private Button menuButton;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            gameOverPanel.SetActive(false);
            if (achievementButton != null)
            {
                achievementButton.onClick.AddListener(() => {
                    AudioManager.Instance?.PlayUIClick();
                    GoToAchievement();
                });
            }

            if (menuButton != null)
            {
                menuButton.onClick.AddListener(() => {
                    AudioManager.Instance?.PlayUIClick();
                    BackToMenu();
                });
            }
        }

        public void Show(bool isVictory, GameStats stats, List<WeaponBase> weapons, List<CollectionRecord> collections) {
            AudioManager.Instance?.StopMusic();

            string titleKey = isVictory ? "UI_Victory" : "UI_GameOver";
            titleText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", titleKey);

            if (isVictory)
                AudioManager.Instance?.PlayVictoryMusic();

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                enemy.SetActive(false);
            }

            Time.timeScale = 0f;

            timeText.text = FormatTime(stats.playTime);
            goldText.text = $"{stats.goldEarned}";
            killText.text = $"{stats.kills}";
            levelText.text = $"{stats.level}";

            foreach (Transform child in weaponContainer) Destroy(child.gameObject);
            foreach (Transform child in collectionContainer) Destroy(child.gameObject);

            // 左侧武器列表（按伤害从大到小排序）
            List<WeaponBase> sortedWeapons = new List<WeaponBase>(weapons);
            sortedWeapons.Sort((a, b) => b.GetTotalDamage().CompareTo(a.GetTotalDamage()));
            foreach (var weapon in sortedWeapons)
            {
                AddWeaponItem(weapon);
            }

            // 右侧道具列表（被动+强化）
            foreach (var item in collections)
            {
                AddCollectionItem(item);
            }

            gameOverPanel.SetActive(true);
        }

        private void AddWeaponItem(WeaponBase weapon)
        {
            GameObject go = Instantiate(weaponItemPrefab, weaponContainer);
            WeaponData data = weapon.GetWeaponData();

            Text nameText = go.transform.Find("Name")?.GetComponent<Text>();
            Text levelText = go.transform.Find("Level")?.GetComponent<Text>();
            Text damageText = go.transform.Find("Damage")?.GetComponent<Text>();
            Text timeText = go.transform.Find("Time")?.GetComponent<Text>();

            string localizedName = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", data.nameKey);
            if (nameText != null) nameText.text = localizedName;
            if (levelText != null) levelText.text = $"{weapon.GetCurrentLevel()}";
            if (damageText != null) damageText.text = FormatDamage(weapon.GetTotalDamage());
            if (timeText != null) timeText.text = weapon.GetFirstGetTimeString();
        }

        private void AddCollectionItem(CollectionRecord item)
        {
            GameObject go = Instantiate(collectionItemPrefab, collectionContainer);

            Image icon = go.transform.Find("Icon")?.GetComponent<Image>();
            Text levelText = go.transform.Find("Level")?.GetComponent<Text>();

            if (icon != null) icon.sprite = item.icon;
            if (levelText != null) levelText.text = $"{item.level}";
        }

        private string FormatDamage(float damage)
        {
            if (damage >= 1000000)
                return (damage / 1000000f).ToString("F1") + "M";
            if (damage >= 1000)
                return (damage / 1000f).ToString("F1") + "K";
            return damage.ToString("F0");
        }

        private string FormatTime(float seconds)
        {
            int minutes = Mathf.FloorToInt(seconds / 60f);
            int secs = Mathf.FloorToInt(seconds % 60f);
            return $"{minutes:00}:{secs:00}";
        }

        private void BackToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        private void GoToAchievement()
        {
            Time.timeScale = 1f;
            PlayerPrefs.SetInt("OpenAchievement", 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("MainMenu");
        }
    }

    // 数据类
    [System.Serializable]
    public class GameStats
    {
        public float playTime;
        public int goldEarned;
        public int kills;
        public int level;
    }

    [System.Serializable]
    public class CollectionRecord
    {
        public Sprite icon;
        public int level;
    }
}
