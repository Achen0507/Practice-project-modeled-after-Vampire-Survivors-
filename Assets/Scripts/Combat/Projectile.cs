using Survivor.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Survivor.Combat
{
    /// <summary>
    /// 子弹/投射物 - 飞行、碰撞、伤害
    /// </summary>
    public class Projectile : MonoBehaviour,IPoolResettable
    {
        [SerializeField] private float baseSpeed = 10f;
        [SerializeField] private float baseLifeTime = 3f;

        public GameObject hitEffectPrefab;  // 命中特效预制体 

        private Vector2 direction;
        private float damage;
        private float range;
        private Vector3 startPosition;
        private float currentSpeed;
        private float currentLifeTime;
        private CancellationTokenSource cts;

        private WeaponBase parentWeapon;

        public float CurrentSpeed => currentSpeed;

        public void Initialize(Vector2 dir, float dmg, float rng, Vector3 pos)
        {
            direction = dir.normalized;
            damage = dmg;
            range = rng;
            startPosition = pos;
            transform.position = pos;

            var attr = PlayerAttributes.Instance;
            currentSpeed = attr?.projectileSpeed ?? baseSpeed;
            currentLifeTime = attr?.duration ?? baseLifeTime;

            gameObject.SetActive(true);

            // 取消旧的移动任务
            cts?.Cancel();
            cts = new CancellationTokenSource();

            // 启动异步移动
            MoveAsync(cts.Token);
        }

        public void SetParentWeapon(WeaponBase weapon)
        {
            parentWeapon = weapon;
        }

        private async void MoveAsync(CancellationToken token)
        {
            try
            {
                float startTime = Time.time;

                while (Time.time - startTime < currentLifeTime)
                {
                    if (token.IsCancellationRequested) break;
                    if (this == null) break;

                    transform.Translate(direction * currentSpeed * Time.deltaTime, Space.World);

                    if (this == null) break;
                    
                    if (Vector2.Distance(startPosition, transform.position) >= range) break;
                    
                    await Task.Yield();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"MoveAsync 异常: {e.Message}");
            }

            if (this != null && !token.IsCancellationRequested)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (PlayerPrefs.GetInt("DamageNumber", 1) == 0) return;
            if (collision.CompareTag("Player")) return;

            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                parentWeapon?.AddDamage(damage);

                if (parentWeapon != null)
                {
                    parentWeapon.ShowDamageNumber(collision.transform.position, Mathf.RoundToInt(damage));
                }

                // 生成命中特效
                if (hitEffectPrefab != null) 
                {
                    Vector3 hitPoint = collision.ClosestPoint(transform.position);
                    GameObject effect = Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);
                    Destroy(effect, 0.8f);
                }

                cts?.Cancel();
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 设置子弹速度（覆盖默认值）
        /// </summary>
        public void SetSpeed(float speed)
        {
            currentSpeed = speed;
        }

        /// <summary>
        /// 获取当前速度
        /// </summary>
        public float GetSpeed()
        {
            return currentSpeed;
        }

        /// <summary>
        /// 设置持续时间（覆盖默认值）
        /// </summary>
        public void SetLifeTime(float lifeTime)
        {
            currentLifeTime = lifeTime;
        }

        public void OnGetFromPool()
        {
            var attr = PlayerAttributes.Instance;
            currentSpeed = attr?.projectileSpeed ?? baseSpeed;
            currentLifeTime = attr?.duration ?? baseLifeTime;

            // 取消之前可能还在运行的异步任务
            cts?.Cancel();
            cts = new CancellationTokenSource();

            gameObject.SetActive(true);
        }
    }
}
