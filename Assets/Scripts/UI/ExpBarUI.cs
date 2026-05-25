using Survivor.Core;
using Survivor.Player;
using UnityEngine;
using UnityEngine.UI;

public class ExpBarUI : MonoBehaviour
{
    /// <summary>
    /// 最上面exp提示
    /// </summary>
    [SerializeField] private Slider expSlider;
    [SerializeField] private Text levelText;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("找不到 PlayerStats");
            return;
        }
        UpdateExpBar();
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerGainExp += OnExpChanged;
        GameEvents.OnPlayerLevelUp += OnLevelUp;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerGainExp -= OnExpChanged;
        GameEvents.OnPlayerLevelUp -= OnLevelUp;
    }

    private void OnExpChanged(int amount,int currentExp,int currentLevel) {
        UpdateExpBar();
    }

    private void OnLevelUp(int newLevel) {
        UpdateExpBar();
    }

    private void UpdateExpBar() {
        if (playerStats == null) return;

        int currentExp = playerStats.CurrentExp;
        int requiredExp = playerStats.ExpRequiredForNextLevel;

        expSlider.maxValue = requiredExp;
        expSlider.value = currentExp;

        if(levelText != null)
        {
            levelText.text = $"LEVEL: {playerStats.CurrentLevel}";
        }
    }
}
