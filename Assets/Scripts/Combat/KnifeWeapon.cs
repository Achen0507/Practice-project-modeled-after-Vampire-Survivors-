using Survivor.Core;
using Survivor.Data;
using UnityEngine;

namespace Survivor.Combat
{
    /// <summary>
    /// 飞刀武器 - 自动瞄准最近敌人发射子弹
    /// </summary>
    public class KnifeWeapon : WeaponBase
    {
        [Header("飞刀专用")]
        [SerializeField] private GameObject knifePrefab;  // 飞刀子弹预制体

        protected override void Attack()
        {

            if (currentTarget == null) return;
            if (knifePrefab == null) return;

            if (playerTransform == null)
            {
                playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
                if (playerTransform == null)
                {
                    Debug.LogError("找不到 Player！");
                    return;
                }
            }

            // 获取当前等级的射击模式
            ShotPattern pattern = GetCurrentShotPattern();

            int weaponLevelCount = pattern.projectileCount;

            // 全局加成（复制器等）默认是 1，加成后增加
            int globalBonus = GetProjectileCount() - 1;
            int finalCount = weaponLevelCount + globalBonus;

            // 计算方向
            Vector2 baseDirection = (currentTarget.Transform.position - playerTransform.position).normalized;

            // 根据散射角度生成多个子弹
            float startAngle = -pattern.spreadAngle / 2f;
            float angleStep = finalCount > 1 ? pattern.spreadAngle / (finalCount - 1) : 0;

            // 如果 spreadAngle 为 0，自动给一个基础偏移
            if (pattern.spreadAngle == 0 && finalCount > 1)
            {
                startAngle = -15f;
                angleStep = 30f / (finalCount - 1);
            }

            for (int i = 0; i < finalCount; i++)
                {
                // 计算每个子弹的方向
                Vector2 finalDir = baseDirection;
                if (finalCount > 1)
                {
                    float angle = startAngle + angleStep * i;
                    finalDir = Quaternion.Euler(0, 0, angle) * baseDirection;
                }      

                Vector3 spawnPos = playerTransform.position;  // 当前玩家位置

                GameObject knife = ObjectPool.Instance.Get("Knife", spawnPos, Quaternion.identity);

                if (knife == null) continue;

                // 设置飞刀的方向和伤害
                Projectile projectile = knife.GetComponent<Projectile>();
                if (projectile != null)
                {
                    float finalDamage = GetCurrentDamage() * pattern.damageMultiplier;
                    float finalSpeed = pattern.projectileSpeed > 0 ? pattern.projectileSpeed : GetProjectileSpeed();
                    projectile.Initialize(finalDir, finalDamage, GetCurrentRange(), spawnPos);
                    projectile.SetSpeed(finalSpeed);
                    projectile.SetParentWeapon(this);
                }
            }

            AudioManager.Instance?.PlayKnifeAttack();
        }

        private ShotPattern GetCurrentShotPattern() {
            if (weaponData != null && weaponData.shotPatterns != null && weaponData.shotPatterns.Length > 0) {
                int level = GetCurrentLevel();
                int index = Mathf.Min(level - 1, weaponData.shotPatterns.Length - 1);
                return weaponData.shotPatterns[index];
            }

            // 默认模式
            return new ShotPattern { projectileCount = 1 };
        }

        public void SetKnifePrefab(GameObject prefab)
        {
            knifePrefab = prefab;
        }
    }
}
