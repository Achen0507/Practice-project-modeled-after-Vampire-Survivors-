using Survivor.Core;
using Survivor.Data;
using Survivor.MainMenu;
using Survivor.Pickups;
using UnityEngine;

namespace Survivor.Enemy
{
    public class Enemy : MonoBehaviour, IDamageable, IPoolResettable
    {
        [SerializeField] private string enemyDataName;
        private EnemyData data;

        [Header("掉落")]
        [SerializeField] private GameObject healthOrbPrefab;
        [SerializeField] private float healthOrbDropChance = 0.08f;  // 8% 概率
        [SerializeField] private GameObject goldPrefab;
        [SerializeField] private float goldDropChance = 0.5f;

        private float currentHealth;
        private bool isAlive = true;
        private bool isOverlappingPlayer = false;

        private Transform playerTransform;
        private SpriteRenderer spriteRenderer;
        private Animator animator;

        public Transform Transform => transform;
        public bool IsAlive => isAlive;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            LoadEnemyData();
        }

        private void LoadEnemyData()
        {
            if (!string.IsNullOrEmpty(enemyDataName))
            {
                data = Resources.Load<EnemyData>($"Data/{enemyDataName}");
                if (data == null)
                {
                    Debug.LogError($"找不到敌人数据: {enemyDataName}");
                }
            }
        }

        public void Initialize(EnemyData enemyData)
        {
            data = enemyData;
            ResetState();
        }

        public float GetBaseDamage()
        {
            return data.baseDamage;
        }

        public void OnGetFromPool()
        {
            ResetState();
        }

        private void ResetState()
        {
            if (data == null) return;

            currentHealth = data.baseHealth;
            isAlive = true;

            if (spriteRenderer != null && data.sprite != null)
                spriteRenderer.sprite = data.sprite;

            if (animator != null && data.animatorController != null)
            {
                animator.runtimeAnimatorController = data.animatorController;
                animator.enabled = true;
            }

            // 判断是否是 精英怪，Boss（血量 > 100 或经验 > 50）
            bool isBoss = data.baseHealth > 100 || data.expValue > 50;

            if (isBoss)
            {
                // Boss 特殊处理
                transform.localScale = Vector3.one * 2.5f;
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = new Color(1f, 0.5f, 0.5f);
                }
            }
            else
            {
                transform.localScale = Vector3.one;
                if (spriteRenderer != null)
                    spriteRenderer.color = Color.white;
            }

            if (GetComponent<Collider2D>() != null)
                GetComponent<Collider2D>().enabled = true;
        }

        public void TakeDamage(float damage)
        {
            if (!isAlive) return;

            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            isAlive = false;

            // 生成经验球
            if (data.expValue > 0)
            {
                GameObject expOrb = ObjectPool.Instance.Get("ExpOrb", transform.position+new Vector3(-0.3f, 0, 0), Quaternion.identity);
                if (expOrb != null)
                {
                    var orb = expOrb.GetComponent<ExpOrb>();
                    orb?.Initialize(data.expValue, transform.position);
                }
            }

            // 生成金币
            if (data.goldValue > 0 && Random.value < goldDropChance)
            {
                GameObject gold = ObjectPool.Instance.Get("GoldCoin", transform.position, Quaternion.identity);
                if (gold != null)
                {
                    GoldCoin goldCoin = gold.GetComponent<GoldCoin>();
                    if (goldCoin != null)
                    {
                        goldCoin.Initialize(data.goldValue);
                    }
                }
               
            }

            // 生成红心
            if (healthOrbPrefab != null && Random.value < healthOrbDropChance)
            {
                Instantiate(healthOrbPrefab, transform.position + new Vector3(0.3f, 0, 0), Quaternion.identity);
            }
            AchievementManager.Instance?.AddProgress(AchievementType.TotalKills, 1, data.enemyName);
            GameEvents.EnemyDeath(gameObject, transform.position);

            float delay = 0f;

            var returnable = GetComponent<PoolReturnable>();
            if (returnable != null)
                Invoke(nameof(ReturnToPool), delay);
            else
                Destroy(gameObject, delay);
        }

        private void ReturnToPool()
        {
            GetComponent<PoolReturnable>()?.ReturnToPool();
        }

        private void Update()
        {
            if (!isAlive) return;
            if (data == null) return;

            if (playerTransform == null)
                playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (playerTransform != null)
            {
                Vector2 direction = (playerTransform.position - transform.position).normalized;
                transform.Translate(direction * data.baseSpeed * Time.deltaTime, Space.World);

                if (animator != null)
                {
                    animator.SetBool("IsMoving", true);
                }

                if (spriteRenderer != null)
                {
                    if (!isOverlappingPlayer)
                    {
                        bool shouldFlip = direction.x < 0;
                        if (spriteRenderer.flipX != shouldFlip)
                        {
                            spriteRenderer.flipX = shouldFlip;
                        }
                    }
                }
            }
        }


       //private void OnTriggerEnter2D(Collider2D other)
       //{
       //    if (!isAlive) return;
       //    if (Time.time - lastDamageTime < damageCooldown) return;
       //
       //    if (other.CompareTag("Player"))
       //    {
       //        isOverlappingPlayer = true;
       //        animator?.SetTrigger("Attack");
       //
       //        // 立即造成伤害
       //        PlayerStats playerStats = other.GetComponent<PlayerStats>();
       //        playerStats?.TakeDamage(data.baseDamage);
       //
       //        lastDamageTime = Time.time;
       //    }
       //}
       //
       //private void OnTriggerExit2D(Collider2D collision)
       //{
       //    if (collision.CompareTag("Player"))
       //    {
       //        isOverlappingPlayer = false;
       //    }
       //}
    }
}
