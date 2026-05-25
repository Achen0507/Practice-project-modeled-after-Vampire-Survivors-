using Survivor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Survivor.MainMenu
{
    public class AchievementUI : MonoBehaviour
    {
        [Header("面板")]
        [SerializeField] private GameObject recordPanel;
        [SerializeField] private Button backButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button optionButton;
        [SerializeField] private Button enhanceButton;
        [SerializeField] private Button recordButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button collectionButton;

        [Header("列表")]
        [SerializeField] private Transform achievementContainer;
        [SerializeField] private GameObject achievementItemPrefab;

        [Header("标题")]
        [SerializeField] private Text titleText;  // 显示 "成就 5/20"

        private void Start()
        {
            backButton.onClick.AddListener(ClosePanel);
            recordPanel.SetActive(false);
        }

        private void OnEnable()
        {
            RefreshList();
        }

        public void OpenPanel() {
            quitButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(false);
            optionButton.gameObject.SetActive(false);
            enhanceButton.gameObject.SetActive(false);
            recordButton.gameObject.SetActive(false);
            creditsButton.gameObject.SetActive(false);
            collectionButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(true);

            recordPanel.SetActive(true);
            RefreshList();
            UpdateTitle();
        }

        public void ClosePanel() {
            recordPanel.SetActive(false);

            quitButton.gameObject.SetActive(true);
            startButton.gameObject.SetActive(true);
            optionButton.gameObject.SetActive(true);
            enhanceButton.gameObject.SetActive(true);
            recordButton.gameObject.SetActive(true);
            creditsButton.gameObject.SetActive(true);
            collectionButton.gameObject.SetActive(true);
        }

        private void UpdateTitle()
        {
            int completedCount = 0;
            foreach (var ach in AchievementManager.Instance.GetAllAchievements())
            {
                if (AchievementManager.Instance.IsCompleted(ach))
                    completedCount++;
            }
            int totalCount = AchievementManager.Instance.GetAllAchievements().Count;

            if (titleText != null)
                titleText.text = $" {completedCount} / {totalCount}";
        }

        public void RefreshList() {
            foreach (Transform child in achievementContainer) 
            {
                Destroy(child.gameObject);
            }

            var achievements = AchievementManager.Instance.GetAllAchievements();

            foreach (var data in achievements)
            {
                GameObject go = Instantiate(achievementItemPrefab, achievementContainer);
                AchievementItem item = go.GetComponent<AchievementItem>();
                bool isCompleted = AchievementManager.Instance.IsCompleted(data);
                int current = AchievementManager.Instance.GetProgress(data);

                string localizedDesc = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", data.descKey);
                item.SetData(data, localizedDesc, isCompleted);

                // 设置进度条
                Slider progressBar = go.GetComponentInChildren<Slider>();
                if (progressBar != null)
                {
                    progressBar.value = (float)current / data.targetValue;
                }
            }
            UpdateTitle();
        }
    } 
}
