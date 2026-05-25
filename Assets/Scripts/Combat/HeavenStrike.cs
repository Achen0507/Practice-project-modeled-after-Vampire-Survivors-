using DG.Tweening;
using Survivor.Core;
using Survivor.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor.Combat
{
    public class HeavenStrike : WeaponBase
    {
        [Header("圣光射线")]
        [SerializeField] private GameObject warningPrefab;  // 预警特效
        [SerializeField] private GameObject laserPrefab;    // 激光特效
        [SerializeField] private float warningDelay = 1.8f; // 预警时间
        [SerializeField] private float radius = 1.2f;       // 攻击半径


        protected override void Attack()
        {
            if (currentTarget == null) return;

            int laserCount = GetLaserCount();
            float radius = GetCurrentRadius();

            // 获取范围内的敌人
            List<Transform> enemies = GetNearbyEnemies(radius * 4f);

            if (laserCount == 1 || enemies.Count <= laserCount)
            {
                // 数量不足，在目标位置生成激光
                SpawnLaser(currentTarget.Transform.position, radius);
            }
            else
            {
                // 随机选择多个敌人
                List<Transform> targets = new List<Transform>(enemies);
                for (int i = 0; i < laserCount && targets.Count > 0; i++)
                {
                    int randomIndex = Random.Range(0, targets.Count);
                    SpawnLaser(targets[randomIndex].position, radius);
                    targets.RemoveAt(randomIndex);
                }
            }
            AudioManager.Instance?.PlayHeavenStrikeAttack();
        }

        private void SpawnLaser(Vector3 position, float radius) {
            // 生成预警
            // 直接生成在世界坐标
            GameObject warning = Instantiate(warningPrefab, position, Quaternion.identity);
            Vector3 originalScale = warning.transform.localScale;
            warning.transform.localScale = originalScale * 0.5f;

            Sequence seq = DOTween.Sequence();
            seq.Append(warning.transform.DOScale(originalScale, warningDelay)); // 宽度从 0.5 → 2
            seq.OnComplete(() => {
                Destroy(warning);
                DealDamage(position, radius);
            });
        }
        private void DealDamage(Vector3 position, float radius)
        {
            GameObject laser = Instantiate(laserPrefab, position, Quaternion.identity);

            // 检测范围内敌人
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(position, radius);
            foreach (var enemyCollider in hitEnemies)
            {
                IDamageable enemy = enemyCollider.GetComponent<IDamageable>();
                if (enemy != null && enemy.IsAlive)
                {
                    // 随机伤害（0.8~1.2 倍）
                    float damage = GetCurrentDamage() * Random.Range(0.8f, 1.2f);
                    enemy.TakeDamage(damage);
                    AddDamage(damage);

                    ShowDamageNumber(enemyCollider.transform.position, Mathf.RoundToInt(damage));
                }
            }

            Destroy(laser, 1f);
        }

        private List<Transform> GetNearbyEnemies(float range)
        {
            List<Transform> enemies = new List<Transform>();
            GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (var enemyObj in enemyObjects)
            {
                float distance = Vector2.Distance(playerTransform.position, enemyObj.transform.position);
                if (distance <= range)
                {
                    enemies.Add(enemyObj.transform);
                }
            }

            return enemies;
        }

        private int GetLaserCount()
        {
            // 武器等级决定的激光数量
            int weaponLevelCount = 1;
            if (weaponData?.shotPatterns != null && weaponData.shotPatterns.Length > 0)
            {
                int level = GetCurrentLevel();
                int idx = Mathf.Min(level - 1, weaponData.shotPatterns.Length - 1);
                weaponLevelCount = weaponData.shotPatterns[idx].projectileCount;
            }

            // 全局加成（复制器等）
            int globalBonus = GetProjectileCount() - 1;
            return Mathf.Max(1, weaponLevelCount + globalBonus);
        }

        private float GetCurrentRadius()
        {
            return radius;
        }

        public void SetHeavenStrike(GameObject prefab)
        {
            laserPrefab = prefab;
        }

        public void SetWarning(GameObject prefab)
        {
            warningPrefab = prefab;
        }
    }
}
