using Survivor.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Survivor.MainMenu
{
    /// <summary>
    /// 武器收藏页面
    /// </summary>
    public class CollectionUI : MonoBehaviour
    {
        [Header("主菜单按钮")]
        [SerializeField] private Button quitButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button optionButton;
        [SerializeField] private Button enhanceButton;
        [SerializeField] private Button recordButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button collectionButton;
        [SerializeField] private Button backButton;

        [Header("收藏面板")]
        [SerializeField] private GameObject collectionPanel;

        [Header("上侧列表")]
        [SerializeField] private Transform listContainer;
        [SerializeField] private GameObject listItemPrefab;
        [SerializeField] private GameObject emptyItemPrefab;

        [Header("下侧详情")]
        [SerializeField] private Image detailIcon;
        [SerializeField] private Text detailName;
        [SerializeField] private Text detailDesc;
        [SerializeField] private Text detailAbout;

        [Header("空状态")]
        [SerializeField] private Transform detailContainer;      

        [Header("标题")]
        [SerializeField] private Text collectionCountText;

        private List<CollectionItem> allItems;

        private void Start()
        {
            if (CollectionManager.Instance != null)
            {
                var items = CollectionManager.Instance.GetAllItems();
            }
            backButton.onClick.AddListener(ClosePanel);
            collectionPanel.SetActive(false);
        }

        public void OpenPanel()
        {
            quitButton.gameObject.SetActive(false);
            startButton.gameObject.SetActive(false);
            optionButton.gameObject.SetActive(false);
            enhanceButton.gameObject.SetActive(false);
            recordButton.gameObject.SetActive(false);
            creditsButton.gameObject.SetActive(false);
            collectionButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(true);

            if (collectionPanel != null) collectionPanel.SetActive(true);

            RefreshList();
            UpdateCollectionCount();
        }

        public void ClosePanel()
        {
            collectionPanel.SetActive(false);

            quitButton.gameObject.SetActive(true);
            startButton.gameObject.SetActive(true);
            optionButton.gameObject.SetActive(true);
            enhanceButton.gameObject.SetActive(true);
            recordButton.gameObject.SetActive(true);
            creditsButton.gameObject.SetActive(true);
            collectionButton.gameObject.SetActive(true);
        }

        private void UpdateCollectionCount() {
            int unlockedCount = CollectionManager.Instance.GetUnlockedCount();
            int totalCount = CollectionManager.Instance.GetAllItems().Count;

            if (collectionCountText != null)
                collectionCountText.text = $"{unlockedCount} / {totalCount}";
        }

        private void RefreshList()
        {
            foreach (Transform child in listContainer)
                Destroy(child.gameObject);

            allItems = CollectionManager.Instance.GetAllItems();

            if (allItems == null || allItems.Count == 0)
            {
                ShowEmptyList();
                ShowEmptyDetail();
                return;
            }

            foreach (var item in allItems)
            {
                bool isUnlocked = CollectionManager.Instance.IsUnlocked(item);

                // 根据解锁状态选择预制体
                GameObject prefab = isUnlocked ? listItemPrefab : emptyItemPrefab;
                GameObject go = Instantiate(prefab, listContainer);

                Image icon = go.transform.Find("itemIcon")?.GetComponent<Image>();
                if (icon != null && isUnlocked)
                {
                    icon.sprite = item.icon;
                }

                Button btn = go.GetComponent<Button>();
                btn.onClick.AddListener(() => ShowDetail(item));
            }
            // 默认显示第一个藏品
            if (allItems.Count > 0)
                ShowDetail(allItems[0]);

            UpdateCollectionCount();
        }

        private void ShowEmptyList()
        {
            GameObject emptyGO = Instantiate(emptyItemPrefab, listContainer);
        }

        private void ShowEmptyDetail()
        {
            // 下半部分显示空提示
            detailIcon.sprite = null;
            detailIcon.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            detailName.text = "???";
            detailDesc.text = "在游戏中获得武器来解锁收藏";
            detailAbout.text = "";
        }

        private void ShowDetail(CollectionItem item)
        {
            // 正常显示藏品详情
            bool isUnlocked = CollectionManager.Instance.IsUnlocked(item);

            if (isUnlocked)
            {
                detailIcon.sprite = item.icon;
                detailIcon.color = Color.white;

                detailName.text = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", item.nameKey);
                detailDesc.text = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", item.descKey);
                detailAbout.text = LocalizationSettings.StringDatabase.GetLocalizedString("UIText", item.aboutKey);
            }
            else
            {
                // 未解锁显示问号
                detailIcon.sprite = null;
                detailIcon.color = new Color(0.2f, 0.2f, 0.2f, 1f);
                detailName.text = "???";
                detailDesc.text = item.unlockConditionHint;
                detailAbout.text = "尚未解锁";
            }
            detailIcon.gameObject.SetActive(true);
            detailName.gameObject.SetActive(true);
            detailDesc.gameObject.SetActive(true);
            detailAbout.gameObject.SetActive(true);
        }
    }
}
