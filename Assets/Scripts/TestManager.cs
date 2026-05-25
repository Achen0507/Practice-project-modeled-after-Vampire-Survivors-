using UnityEngine;
using Survivor.Player;
using Survivor.Passives;
using Survivor.Upgrades;
using Survivor.Combat;

public class TestManager : MonoBehaviour
{
    private void Update()
    {
        // F1: 增加 100 经验
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PlayerStats stats = FindObjectOfType<PlayerStats>();
            if (stats != null) stats.GainExp(100);
            Debug.Log("【测试】增加 100 经验");
        }

        // F2: 增加 100 金币
        if (Input.GetKeyDown(KeyCode.F2))
        {
            GoldManager.Instance?.AddGold(100);
            Debug.Log("【测试】增加 100 金币");
        }

        // F4: 升级当前武器
        if (Input.GetKeyDown(KeyCode.F4))
        {
            var weapons = WeaponManager.Instance?.Weapons;
            if (weapons != null && weapons.Count > 0)
            {
                weapons[0].Upgrade();
                Debug.Log($"【测试】武器升级: {weapons[0].name}");
            }
        }
    }
}

