using Survivor.Core;
using Survivor.Data;
using Survivor.Enemy;
using Survivor.UI;
using UnityEngine;

namespace Survivor.Combat
{
    /// <summary>
    /// 武器基类 - 所有武器的父类
    /// </summary>
    public abstract class WeaponBase : MonoBehaviour
    {
        [Header("武器配置")]
        [SerializeField] protected WeaponData weaponData;

        [Header("运行时状态")]
        private int currentLevel = 1;
        protected float currentCooldown;

        protected Transform playerTransform;
        protected IDamageable currentTarget;

        protected GameObject damageTextPrefab;
        protected Transform canvasTransform;

        private float totalDamage = 0;
        private float firstGetTime = -1;

        protected virtual void Start() {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            currentCooldown = 0f;
        }

        protected virtual void Update()
        {
            // 冷却计时
            if (currentCooldown > 0)
                currentCooldown -= Time.deltaTime;
            FindTarget();

            if (currentCooldown <= 0 && currentTarget != null && currentTarget.IsAlive)
            {
                Attack();
                currentCooldown = GetCurrentCooldown();
            }
        }
       
        /// <summary>
        /// 寻找最近的敌人
        /// </summary>
        protected virtual void FindTarget() {
            if (playerTransform == null) return;

            if (EnemyManager.Instance == null) return;

            // 使用 EnemyManager 快速获取最近敌人 
            currentTarget = EnemyManager.Instance?.GetNearestEnemy(playerTransform.position);

            if (currentTarget != null) {
                float distance = Vector2.Distance(playerTransform.position, currentTarget.Transform.position);
                if (distance > GetCurrentRange())
                {
                    currentTarget = null;  // 太远，不攻击
                }
            }
        }

        /// <summary>
        /// 攻击方法 - 子类实现具体攻击逻辑
        /// </summary>
        protected abstract void Attack(); 

        /// <summary>
        /// 获取当前冷却时间（已应用等级加成）
        /// </summary>
        protected virtual float GetCurrentCooldown() {
            float baseCooldown = weaponData?.baseCooldown ?? 1f;
            float multiplier = PlayerAttributes.Instance?.GetCooldownMultiplier() ?? 1f;
            return Mathf.Max(0.1f, baseCooldown * multiplier);
        }

        /// <summary>
        /// 获取当前攻击范围（已应用等级加成）
        /// </summary>
        protected virtual float GetCurrentRange() {
            float baseRange = weaponData?.baseRange ?? 5f;
            float bonus = PlayerAttributes.Instance?.attackRange ?? 0f;
            return baseRange + bonus;
        }

        /// <summary>
        /// 获取当前伤害（已应用等级加成）
        /// </summary>
        protected virtual float GetCurrentDamage() {
            float baseDamage = weaponData?.baseDamage ?? 10f;
            float multiplier = PlayerAttributes.Instance?.GetDamageMultiplier() ?? 1f;

            // 基础伤害
            float finalDamage = baseDamage * multiplier;

            // 随机波动 ±10%（波动范围可配置）
            float randomFactor = Random.Range(0.8f, 1.2f); 
            finalDamage = finalDamage * randomFactor;

            return Mathf.Max(1f, finalDamage);
        }

        /// <summary>
        ///获取子弹速度
        /// </summary>
        protected virtual float GetProjectileSpeed() {
            float baseSpeed = weaponData?.baseProjectileSpeed ?? 10f;
            float bonus = PlayerAttributes.Instance?.projectileSpeed ?? 0f;
            return baseSpeed + bonus;
        }

        protected virtual int GetProjectileCount()
        {
            if (PlayerAttributes.Instance == null)
            {
                return 1;
            }
            return PlayerAttributes.Instance.projectileCount;
        }

        public void SetDamageTextPrefab(GameObject prefab)
        {
            damageTextPrefab = prefab;
        }

        public void SetCanvasTransform(Transform canvas)
        {
            canvasTransform = canvas;
        }

        public void ShowDamageNumber(Vector3 worldPos, int damage)
        {
            if (PlayerPrefs.GetInt("DamageNumber", 1) == 0) return;

            GameObject canvas = GameObject.Find("DamageTextCanvas");
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            GameObject obj = Instantiate(damageTextPrefab, screenPos, Quaternion.identity, canvas.transform);
            DamageText dt = obj.GetComponent<DamageText>();
            if (dt != null)
            {
                dt.Initialize(damage, false);
            }
        }

        /// <summary>
        /// 升级武器
        /// </summary>
        public virtual void Upgrade() {
            int oldLevel = currentLevel;
            currentLevel++;
        }

        public int GetCurrentLevel()
        {
            return currentLevel;
        }

        public void SetWeaponData(WeaponData data)
        {
            weaponData = data;
        }

        public void RecordFirstGetTime(float gameTime)   //结算界面用
        {
            if (firstGetTime < 0)
                firstGetTime = gameTime;
        }
        public string GetFirstGetTimeString()  //结算界面用
        {
            if (firstGetTime < 0) return "--:--";
            int minutes = Mathf.FloorToInt(firstGetTime / 60f);
            int seconds = Mathf.FloorToInt(firstGetTime % 60f);
            return $"{minutes:00}:{seconds:00}";
        }

        public WeaponData GetWeaponData() => weaponData;
        public void AddDamage(float damage) => totalDamage += damage;
        public float GetTotalDamage() => totalDamage;

    }
}
