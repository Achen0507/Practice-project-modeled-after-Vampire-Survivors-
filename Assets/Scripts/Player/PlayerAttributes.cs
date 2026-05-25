using Survivor.Data;
using Survivor.Player;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    public static PlayerAttributes Instance { get; private set; }

    [Header("当前属性（游戏内会被升级修改）")]
    public float maxHealth = 100f;
    public float healthRegen = 0f;
    public float armor = 0f;
    public float moveSpeed = 5f;
    public float power = 0f;
    public float projectileSpeed = 10f;
    public float duration = 3f;
    public float attackRange = 5f;
    public float cooldownReduction = 0f;
    public int projectileCount = 1;
    public int reviveChance = 0;
    public float luck = 0f;
    public float growth = 0f;
    public float greed = 0f;

    // 原始值（用于重置）
    private float originalMaxHealth;
    private float originalHealthRegen;
    private float originalArmor;
    private float originalMoveSpeed;
    private float originalPower;
    private float originalProjectileSpeed;
    private float originalDuration;
    private float originalAttackRange;
    private float originalCooldownReduction;
    private int originalProjectileCount;
    private int originalReviveChance;
    private float originalLuck;
    private float originalGrowth;
    private float originalGreed;

    private float regenTimer;
    private PlayerStats playerStats;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);  
    }

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        regenTimer = 1f;
    }

    // 进入游戏时，从选中的角色初始化属性
    public void InitializeFromCharacter(CharacterData character)
    {
        if (character == null || character.baseStats == null) return;

        // 从角色基础属性赋值
        maxHealth = character.baseStats.maxHealth;
        healthRegen = character.baseStats.healthRegen;
        armor = character.baseStats.armor;
        moveSpeed = character.baseStats.moveSpeed;
        power = character.baseStats.power;
        projectileSpeed = character.baseStats.projectileSpeed;
        duration = character.baseStats.duration;
        attackRange = character.baseStats.attackRange;
        cooldownReduction = character.baseStats.cooldownReduction;
        projectileCount = character.baseStats.projectileCount;
        luck = character.baseStats.luck;
        growth = character.baseStats.growth;
        greed = character.baseStats.greed;

        // 保存原始值
        SaveOriginalValues();

        Debug.Log($"角色属性已初始化: 生命={maxHealth}, 移速={moveSpeed}");
    }

    public void SaveOriginalValues()
    {
        originalMaxHealth = maxHealth;
        originalHealthRegen = healthRegen;
        originalArmor = armor;
        originalMoveSpeed = moveSpeed;
        originalPower = power;
        originalProjectileSpeed = projectileSpeed;
        originalDuration = duration;
        originalAttackRange = attackRange;
        originalCooldownReduction = cooldownReduction;
        originalProjectileCount = projectileCount;
        originalReviveChance = reviveChance;
        originalLuck = luck;
        originalGrowth = growth;
        originalGreed = greed;
    }

    private void Update()
    {
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();
            if (playerStats == null) return;
        }

        if (healthRegen > 0 && playerStats.CurrentHealth < playerStats.MaxHealth)
        {
            regenTimer -= Time.deltaTime;
            if (regenTimer <= 0)
            {
                playerStats.Heal(healthRegen);
                regenTimer = 1f;
            }
        }
    }

    // 计算减伤后的伤害
    public float CalculateDamage(float incomingDamage) {
        return incomingDamage * (1 - armor / 100f);
    }

    // 应用升级效果
    public void ApplyStatModifier(StatType statType, float value, bool isPercentage) {
        switch (statType) {
            case StatType.MaxHealth:
    
                if (isPercentage)
                    maxHealth += maxHealth * value;
                else
                    maxHealth += value;
                break;
            case StatType.HealthRegen:
                if (isPercentage)
                    healthRegen += healthRegen * value;
                else
                    healthRegen += value;
                break;
            case StatType.Armor:
                if (isPercentage)
                    armor += value * 100f; 
                else
                    armor += value;
                break;
            case StatType.MoveSpeed:
                ApplyMoveSpeed(value, isPercentage);
                break;
            case StatType.DamageBonus:
                if (isPercentage)
                    power += value * 100f;  
                else
                    power += value;  
                break;
            case StatType.ProjectileSpeed:
                if (isPercentage)
                    projectileSpeed += projectileSpeed * value;
                else
                    projectileSpeed += value;
                break;
            case StatType.Duration:
                if (isPercentage)
                    duration += duration * value;
                else
                    duration += value;
                break;
            case StatType.AttackRange:
                if (isPercentage)
                    attackRange += attackRange * value;
                else
                    attackRange += value;
                break;
            case StatType.CooldownReduction:
                if (isPercentage)
                    cooldownReduction += value * 100f;  
                else
                    cooldownReduction += value;
                break;
            case StatType.ProjectileCount:
                if (isPercentage)
                    projectileCount += projectileCount * (int)value;
                else
                    projectileCount += (int)value;
                break;
            case StatType.Growth:
                if (isPercentage)
                    growth += value * 100f;  
                else
                    growth += value;
                break;
            case StatType.Greed:
                if (isPercentage)
                    greed += value * 100f;  
                else
                    greed += value;
                break;
            case StatType.Luck:
                if (isPercentage)
                    luck += value * 100f; 
                else
                    luck += value;
                break;
        }
    }
   
    private void ApplyMoveSpeed(float value, bool isPercentage)
    {
        if (isPercentage)
            moveSpeed += moveSpeed * value;
        else
            moveSpeed += value;

        PlayerController player = GetComponent<PlayerController>();
        if (player != null)
            player.SetMoveSpeed(moveSpeed);
    }

    // 获取经验加成
    public float GetExpBonus() {
        return 1 + (growth + greed) / 100f;
    }

    // 获取伤害倍率
    public float GetDamageMultiplier() {
        return 1 + power / 100f;  
    }

    // 获取冷却倍率
    public float GetCooldownMultiplier() {
        return 1 - Mathf.Min(0.8f, cooldownReduction / 100f);
    }

    // 获取升级选项数量（运气影响）
    public int GetUpgradeOptionCount()
    {
        if (Random.value < luck / 100f)
            return 4;
        return 3;
    }

    // 重置属性（退出游戏时调用）
    public void ResetToBase()
    {
        maxHealth = originalMaxHealth;
        healthRegen = originalHealthRegen;
        armor = originalArmor;
        moveSpeed = originalMoveSpeed;
        power = originalPower;
        projectileSpeed = originalProjectileSpeed;
        duration = originalDuration;
        attackRange = originalAttackRange;
        cooldownReduction = originalCooldownReduction;
        projectileCount = originalProjectileCount;
        reviveChance = originalReviveChance;
        luck = originalLuck;
        growth = originalGrowth;
        greed = originalGreed;

        Debug.Log("属性已重置为初始值");
    }
}