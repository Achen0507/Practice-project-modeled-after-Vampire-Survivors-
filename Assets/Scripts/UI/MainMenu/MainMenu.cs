using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Survivor.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        [Header("金币")]
        [SerializeField] private Text goldText;

        [Header("面板")]
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private GameObject collectionPanel;
        [SerializeField] private GameObject enhancePanel;
        [SerializeField] private GameObject recordPanel;
        [SerializeField] private GameObject creditsPanel;

        [Header("首页按钮容器")]
        [SerializeField] private Button QuitButton;
        [SerializeField] private Button StartButton;
        [SerializeField] private Button OptionButton;
        [SerializeField] private Button EnhanceButton;
        [SerializeField] private Button RecordButton;
        [SerializeField] private Button CreditsButton;
        [SerializeField] private Button CollectionButton;

        private void Start()
        {
            AudioManager.Instance?.PlayMainMenuMusic();
            UpdateGoldUI();

            if (PlayerPrefs.GetInt("OpenAchievement", 0) == 1)
            {
                PlayerPrefs.SetInt("OpenAchievement", 0);
                // 延迟一帧，确保面板都初始化好
                StartCoroutine(OpenRecordPanelDelayed());
            }
        }

        private IEnumerator OpenRecordPanelDelayed()
        {
            yield return null;
            if (recordPanel != null)
            {
                recordPanel.SetActive(true);
                // 同时关闭其他面板
                CloseAllPanels();
                recordPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("recordPanel 未绑定");
            }
        }

        public void UpdateGoldUI() {
            if (goldText != null && GoldManager.Instance != null)
            {
                goldText.text = $"{GoldManager.Instance.GetGold()}";
            }
        }

        public void StartGame() {
            SceneManager.LoadScene("GameScene");
        }

        public void OpenOptions() {
            CloseAllPanels();
            QuitButton.gameObject.SetActive(false);
            StartButton.gameObject.SetActive(false);
            OptionButton.gameObject.SetActive(false);
            EnhanceButton.gameObject.SetActive(false);
            RecordButton.gameObject.SetActive(false);
            CreditsButton.gameObject.SetActive(false);
            CollectionButton.gameObject.SetActive(false);
            if (optionsPanel != null) optionsPanel.SetActive(true);
        }

        public void OpenCollection()
        {
            CloseAllPanels();
            QuitButton.gameObject.SetActive(false);
            StartButton.gameObject.SetActive(false);
            OptionButton.gameObject.SetActive(false);
            EnhanceButton.gameObject.SetActive(false);
            RecordButton.gameObject.SetActive(false);
            CreditsButton.gameObject.SetActive(false);
            CollectionButton.gameObject.SetActive(false);
            if (collectionPanel != null) collectionPanel.SetActive(true);
        }

        public void OpenEnhance()
        {
            CloseAllPanels();
            QuitButton.gameObject.SetActive(false);
            StartButton.gameObject.SetActive(false);
            OptionButton.gameObject.SetActive(false);
            EnhanceButton.gameObject.SetActive(false);
            RecordButton.gameObject.SetActive(false);
            CreditsButton.gameObject.SetActive(false);
            CollectionButton.gameObject.SetActive(false);
            if (enhancePanel != null) enhancePanel.SetActive(true);
        }

        public void OpenRecord()
        {
            CloseAllPanels();
            QuitButton.gameObject.SetActive(false);
            StartButton.gameObject.SetActive(false);
            OptionButton.gameObject.SetActive(false);
            EnhanceButton.gameObject.SetActive(false);
            RecordButton.gameObject.SetActive(false);
            CreditsButton.gameObject.SetActive(false);
            CollectionButton.gameObject.SetActive(false);
            if (recordPanel != null) recordPanel.SetActive(true);
        }

        public void OpenCredits()
        {
            CloseAllPanels();
            QuitButton.gameObject.SetActive(false);
            StartButton.gameObject.SetActive(false);
            OptionButton.gameObject.SetActive(false);
            EnhanceButton.gameObject.SetActive(false);
            RecordButton.gameObject.SetActive(false);
            CreditsButton.gameObject.SetActive(false);
            CollectionButton.gameObject.SetActive(false);
            if (creditsPanel != null) creditsPanel.SetActive(true);
        }

        public void CloseAllPanels() {
            if (optionsPanel != null) optionsPanel.SetActive(false);
            if (collectionPanel != null) collectionPanel.SetActive(false);
            if (enhancePanel != null) enhancePanel.SetActive(false);
            if (recordPanel != null) recordPanel.SetActive(false);
            if (creditsPanel != null) creditsPanel.SetActive(false);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}
