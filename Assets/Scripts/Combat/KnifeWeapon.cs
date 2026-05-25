using Survivor.Core;
using Survivor.Data;
using UnityEngine;

namespace Survivor.Combat
{
    /// <summary>
    /// 飞刀武器 - 效果更像箭
    /// </summary>
    
    public class KnifeWeapon : WeaponBase
    {
        [Header("飞刀专用")]
        [SerializeField] private GameObject knifePrefab;  

        protected override void Attack()
        {

            if (currentTarget == null) return;
            if (knifePrefab == null) return;

            if (playerTransform == null)
            {
                playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
                if (playerTransform == null) return;
            }

            // 获取当前等级的技能效果
            ShotPattern pattern = GetCurrentShotPattern();

            int weaponLevelCount = pattern.projectileCount;

            // 全局加成（被动技能复制器等）
            int globalBonus = GetProjectileCount() - 1;
            int finalCount = weaponLevelCount + globalBonus;

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
            return new ShotPattern { projectileCount = 1 };
        }

        public void SetKnifePrefab(GameObject prefab)
        {
            knifePrefab = prefab;
        }
    }
}
