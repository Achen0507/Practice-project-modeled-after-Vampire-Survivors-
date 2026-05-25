using System;

/// <summary>
/// 玩家的基础设定，用于重置
/// </summary>

[Serializable]
public class CharacterBaseStats
{
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
    public float luck = 0f;
    public float growth = 0f;
    public float greed = 0f;
}