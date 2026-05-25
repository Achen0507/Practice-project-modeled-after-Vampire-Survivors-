using Survivor.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor.Enemy
{
    /// <summary>
    /// 敌人管理器 - 维护所有存活敌人的列表
    /// </summary>
    
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance { get; private set; }

        private List<IDamageable> enemies = new List<IDamageable>();

        public IReadOnlyList<IDamageable> AllEnemies => enemies;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            GameEvents.OnEnemyDeath += OnEnemyDeath;
        }

        private void OnDestroy()
        {
            GameEvents.OnEnemyDeath -= OnEnemyDeath;
        }

        /// <summary>
        /// 注册新敌人
        /// </summary>
        public void RegisterEnemy(IDamageable enemy)
        {
            if (!enemies.Contains(enemy))
            {
                enemies.Add(enemy);
            }
        }

        /// <summary>
        /// 敌人死亡时移除
        /// </summary>
        private void OnEnemyDeath(GameObject enemyObj, Vector3 position)
        {
            var damageable = enemyObj.GetComponent<IDamageable>();
            if (damageable != null && enemies.Contains(damageable))
            {
                enemies.Remove(damageable);
            }
        }

        /// <summary>
        /// 获取离指定点最近的敌人
        /// </summary>
        public IDamageable GetNearestEnemy(Vector3 position)
        {
            IDamageable nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (var enemy in enemies)
            {
                if (enemy == null || !enemy.IsAlive) continue;

                float distance = Vector2.Distance(position, enemy.Transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = enemy;
                }
            }
            return nearest;
        }
    }
}