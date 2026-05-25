using Survivor.Data;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Survivor.MainMenu
{
    /// <summary>
    /// 强化页面
    /// </summary>
    public class EnhanceUI : MonoBehaviour
    {
        public static EnhanceUI Instance { get; private set; }

        [Header("面板")]
        [SerializeField] private GameObject enhancePanel;
        [SerializeField] private Button backButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button optionButton;
        [SerializeField] private Button enhanceButton;
        [SerializeField] private Button recordButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button collectionButton;

        [Header("列表")]
        [SerializeField] private Transform gridContainer;
        [SerializeField] private GameObject gridItemPrefab;

        [Header("详情")]
        [SerializeField] private Image detailIcon;
        [SerializeField] private Text detailName;
        [SerializeField] private Text detailDesc;
        [SerializeField] private Text detailCost;
        [SerializeField] private Button buyButton;

        private UpgradeData currentSelected;

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
            backButton.onClick.AddListener(ClosePanel);
            buyButton.onClick.AddListener(OnBuyClick);
            enhancePanel.SetActive(false);
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

            enhancePanel.SetActive(true);
            RefreshGrid();
        }

        public void ClosePanel() {
            enhancePanel.SetActive(false);

            quitButton.gameObject.SetActive(true);
            startButton.gameObject.SetActive(true);
            optionButton.gameObject.SetActive(true);
            enhanceButton.gameObject.SetActive(true);
            recordButton.gameObject.SetActive(true);
            creditsButton.gameObject.SetActive(true);
            collectionButton.gameObject.SetActive(true);
        }

        public void Refresh() {
            RefreshGrid();
            if (currentSelected != null) ShowDetail(currentSelected);
        }

        private void RefreshGrid() {
            foreach (Transform child in gridContainer)
                Destroy(child.gameObject);

            var allUpgrades = EnhanceManager.Instance.GetAllUpgrades();
            if (allUpgrades == null || allUpgrades.Count == 0) return;

            foreach (var upgrade in allUpgrades) {
                GameObject go = Instantiate(gridItemPrefab, gridContainer);
                EnhanceItem item = go.GetComponent<EnhanceItem>();
                item.SetData(upgrade, EnhanceManager.Instance.GetLevel(upgrade));

                Button btn = go.GetComponent<Button>();
                btn.onClick.AddListener(() => ShowDetail(upgrade));
            }
            // 默认选中第一个
            if (allUpgrades.Count > 0)
            {
                ShowDetail(allUpgrades[0]);
            }
        }

        private void ShowDetail(UpgradeData upgrade) {
            currentSelected = upgrade;

            int level = EnhanceManager.Instance.GetLevel(upgrade);
            bool isMaxLevel = level >= upgrade.maxLevel;
            int cost = EnhanceManager.Instance.GetNextLevelCost(upgrade);
            bool canBuy = EnhanceManager.Instance.CanUpgrade(upgrade);

            detailIcon.sprite = upgrade.icon;
            detailName.text = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", upgrade.nameKey);
            detailDesc.text = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", upgrade.descKey);

            if (!isMaxLevel)
            {
                detailCost.text = $"{cost}";
                buyButton.interactable = canBuy;
            }
            else {
                detailCost.text = "已满级";
                buyButton.interactable = false;
            }
        }

        private void OnBuyClick() {
            if (currentSelected != null) {
                EnhanceManager.Instance.Upgrade(currentSelected);
                Refresh();
            }
        }
    }
}
