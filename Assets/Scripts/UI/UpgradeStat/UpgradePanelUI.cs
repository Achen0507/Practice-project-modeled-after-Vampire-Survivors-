using Survivor.Core;
using Survivor.Data;
using Survivor.Upgrades;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanelUI : MonoBehaviour
{
    public static UpgradePanelUI Instance { get; private set; }

    [Header("面板设置")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Transform cardContainer;      // 卡片父物体
    [SerializeField] private GameObject cardPrefab;        // 卡片预制体

    private List<UpgradeCardUI> currentCards = new List<UpgradeCardUI>();
    private List<UpgradeData> currentOptions;

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
       panelRoot.SetActive(false);
   }

    public void ShowUpgradePanel(List<UpgradeData> options) {
        currentOptions = options;

        foreach (var card in currentCards)
        {
            Destroy(card.gameObject);
        }
        currentCards.Clear();

        // 动态创建卡片
        foreach (var option in currentOptions)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardContainer);
            UpgradeCardUI card = cardObj.GetComponent<UpgradeCardUI>();
            card.Initialize(option, OnUpgradeSelected);
            currentCards.Add(card);
        }

        panelRoot.SetActive(true);
        AudioManager.Instance?.PlayUpgradeSelect();
        TimeManager.Instance.AddPause();
    }

    private void OnUpgradeSelected(UpgradeData upgrade) {
        if (UpgradeManager.Instance != null) {
            UpgradeManager.Instance.SelectUpgrade(upgrade);
        } 
        ClosePanel();
    }

    private void ClosePanel()
    {
        panelRoot.SetActive(false);
        TimeManager.Instance?.RemovePause();

        UpgradeManager.Instance?.OnPanelClosed();
    }
}
