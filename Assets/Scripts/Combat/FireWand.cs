using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Survivor.Combat
{
    public class FireWand : WeaponBase
    {
        [Header("火焰法杖")]
        [SerializeField] private GameObject fireballPrefab;
        [SerializeField] private float fireballSpeed = 10f;
        [SerializeField] private float burstInterval = 0.08f;  // 连发间隔（秒）

        protected override void Attack()
        {
            BurstFireAsync();
            AudioManager.Instance?.PlayFireWandAttack();
        }

        private async void BurstFireAsync()
        {
            int count = GetFireballCount();

            for (int i = 0; i < count; i++)
            {
                Transform target = GetNearestEnemy();
                if (target != null)
                {
                    // 基础方向（指向最近敌人）
                    Vector2 baseDirection = (target.position - playerTransform.position).normalized;

                    // 关键：在这个方向上加上一个随机旋转（-45° 到 45°）
                    float randomAngle = Random.Range(-45f, 45f);
                    Vector2 finalDirection = Quaternion.Euler(0, 0, randomAngle) * baseDirection;

                    // 随机位置偏移
                    float offsetX = Random.Range(-0.3f, 0.3f);
                    Vector3 spawnPos = playerTransform.position + new Vector3(offsetX, 0, 0);

                    ShootFireball(finalDirection, spawnPos);
                }

                if (i < count - 1)
                    await Task.Delay((int)(burstInterval * 1000));
            }
        }

        private Transform GetNearestEnemy()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            Transform nearest = null;
            float minDist = float.MaxValue;

            foreach (var enemy in enemies)
            {
                float dist = Vector2.Distance(playerTransform.position, enemy.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = enemy.transform;
                }
            }
            return nearest;
        }

        private void ShootFireball(Vector2 direction, Vector3 spawnPos)
        {
            if (fireballPrefab == null) return;

            GameObject fireball = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

            Projectile proj = fireball.GetComponent<Projectile>();
            if (proj == null)
                proj = fireball.AddComponent<Projectile>();

            proj.Initialize(direction, GetCurrentDamage(), GetCurrentRange(), spawnPos);
            proj.SetSpeed(fireballSpeed);
            proj.SetLifeTime(1.5f);
            proj.SetParentWeapon(this);
        }

        private int GetFireballCount()
        {
            int weaponLevelCount = 1;
            if (weaponData?.shotPatterns != null && weaponData.shotPatterns.Length > 0)
            {
                int idx = Mathf.Min(GetCurrentLevel() - 1, weaponData.shotPatterns.Length - 1);
                weaponLevelCount = weaponData.shotPatterns[idx].projectileCount;
            }
            // 全局加成（复制器等）
            int globalBonus = GetProjectileCount() - 1;
            return Mathf.Max(1, weaponLevelCount + globalBonus);
        }

        public void SetFireWand(GameObject prefab)
        {
            fireballPrefab = prefab;
        }
    }
}