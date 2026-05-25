using Survivor.Combat;
using Survivor.Data;
using Survivor.Player;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance?.PlayGameMusic();

        // 获取选中的角色数据
        CharacterData selectedChar = GameManager.Instance.SelectedCharacter;
        var finalStats = GameManager.Instance.FinalCharacterStats;

        if (selectedChar == null || finalStats == null)
        {
            Debug.LogError("数据缺失");
            return;
        }

        GameObject damageTextPrefab = Resources.Load<GameObject>("DamageText");
        Transform canvasTransform = GameObject.Find("DamageTextCanvas")?.transform;
        WeaponManager.Instance.SetDamageTextPrefab(damageTextPrefab, canvasTransform);

        PlayerAttributes.Instance.maxHealth = finalStats[StatType.MaxHealth];
        PlayerAttributes.Instance.healthRegen = finalStats[StatType.HealthRegen];
        PlayerAttributes.Instance.armor = finalStats[StatType.Armor];
        PlayerAttributes.Instance.moveSpeed = finalStats[StatType.MoveSpeed];
        PlayerAttributes.Instance.power = finalStats[StatType.DamageBonus];
        PlayerAttributes.Instance.projectileSpeed = finalStats[StatType.ProjectileSpeed];
        PlayerAttributes.Instance.duration = finalStats[StatType.Duration];
        PlayerAttributes.Instance.attackRange = finalStats[StatType.AttackRange];
        PlayerAttributes.Instance.cooldownReduction = finalStats[StatType.CooldownReduction];
        PlayerAttributes.Instance.projectileCount = (int)finalStats[StatType.ProjectileCount];
        PlayerAttributes.Instance.luck = finalStats[StatType.Luck];
        PlayerAttributes.Instance.growth = finalStats[StatType.Growth];
        PlayerAttributes.Instance.greed = finalStats[StatType.Greed];

        // 保存原始值（用于每次重置）
        PlayerAttributes.Instance.SaveOriginalValues();

        //  添加角色初始武器
        if (!string.IsNullOrEmpty(selectedChar.starterWeaponName))
        {
            WeaponData weaponData = Resources.Load<WeaponData>($"Data/WeaponData/{selectedChar.starterWeaponName}");
            if (weaponData != null)
            {
                AddWeapon(weaponData);
            }
            else
            {
                Debug.LogError($"找不到初始武器数据: {selectedChar.starterWeaponName}");
            }
        }

        // 同步移速到 PlayerController,因为Attr和Controller不在同个物体上面
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.SetMoveSpeed(PlayerAttributes.Instance.moveSpeed);
        }

        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.RefreshHealthFromAttributes();
        }
    }

    private void AddWeapon(WeaponData data)
    {
        // 创建武器物体
        GameObject weaponObj = new GameObject(data.weaponName); 
        weaponObj.transform.SetParent(transform);  // 挂在 Player 下

        WeaponBase weapon = null;

        switch (data.weaponName)
        {
            case "爱之箭":
                weapon = weaponObj.AddComponent<KnifeWeapon>();
                var knife = weapon as KnifeWeapon;
                if (knife != null && data.projectilePrefab != null) {
                    knife.SetKnifePrefab(data.projectilePrefab);
                }
                break;
            case "守护光环":
                weapon = weaponObj.AddComponent<OrbitWeapon>();
                var orbit = weapon as OrbitWeapon;
                if (orbit != null && data.projectilePrefab != null)
                {
                    orbit.SetOrbitPrefab(data.projectilePrefab);
                }
                break;
            case "神圣激光":
                weapon = weaponObj.AddComponent<HeavenStrike>();
                var heavenStrike = weapon as HeavenStrike;
                if (heavenStrike != null && data.projectilePrefab != null)
                {
                    heavenStrike.SetWarning(data.extraPrefab);
                    heavenStrike.SetHeavenStrike(data.projectilePrefab);
                }
                break;
            case "剑气":
                weapon = weaponObj.AddComponent<WhipWeapon>();
                var whip = weapon as WhipWeapon;
                if (whip != null && data.projectilePrefab != null)
                {
                    whip.SetWhip(data.projectilePrefab);
                }
                break;
            case "火球":
                weapon = weaponObj.AddComponent<FireWand>();
                var fireWand = weapon as FireWand;
                if (fireWand != null && data.projectilePrefab != null)
                {
                    fireWand.SetFireWand(data.projectilePrefab);
                }
                break;
            default:
                Debug.LogError($"未知武器: {data.weaponName}");
                Destroy(weaponObj);
                return;
        }
        // 设置武器数据
        weapon.SetWeaponData(data);
        weapon.RecordFirstGetTime(Time.timeSinceLevelLoad);   //结算界面用
        WeaponManager.Instance.AddWeapon(weapon);
    }
}
