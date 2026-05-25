using Survivor.Core;
using Survivor.Player;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0);  // 血量条偏移
    private PlayerStats playerStats;
    private Transform playerTransform;

    private void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogError("FloatingHealthBar: 找不到 PlayerStats");
            return;
        }
        // 获取玩家位置（向上查找根物体）
        playerTransform = playerStats.transform;

        healthSlider.maxValue = playerStats.MaxHealth;
        healthSlider.value = playerStats.CurrentHealth;
    }

    private void LateUpdate()
    {
        if (playerTransform != null)
        {
            transform.position = playerTransform.position + offset;
        }
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerTakeDamage += OnTakeDamage;
        GameEvents.OnPlayerLevelUp += OnLevelUp;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerTakeDamage -= OnTakeDamage;
        GameEvents.OnPlayerLevelUp -= OnLevelUp;
    }

    private void OnTakeDamage(float damage, float currentHp, float maxHp)
    {
        healthSlider.maxValue = maxHp;
        healthSlider.value = currentHp;
    }

    private void OnLevelUp(int newLevel)
    {
        if (playerStats != null)
        {
            healthSlider.maxValue = playerStats.MaxHealth;
            healthSlider.value = playerStats.CurrentHealth;
        }
    }
}
