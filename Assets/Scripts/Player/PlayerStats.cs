using DG.Tweening;
using Survivor.Cam;
using Survivor.Combat;
using Survivor.Core;
using Survivor.Passives;
using Survivor.UI;
using Survivor.Upgrades;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor.Player
{
    /// <summary>
    /// 玩家属性：生命值、经验、等级的相关事件
    /// </summary>
    public class PlayerStats : MonoBehaviour, IDamageable
    {
        [Header("生命值")]
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        [Header("经验值")]
        [SerializeField] private int startLevel = 1;
        private int currentLevel;
        private int currentExp;

        [Header("升级曲线")]
        [SerializeField] private int baseExpRequired = 50;
        [SerializeField] private float expGrowthFactor = 1.2f;  // 每级所需经验增长倍率

        [Header("材质")]
        private SpriteRenderer sr;
        private Sprite originalSprite;
        private Sprite yellowSprite;

        // 公开属性供其他系统访问
        public float CurrentHealth => currentHealth;
        public float MaxHealth => PlayerAttributes.Instance?.maxHealth ?? maxHealth;
        public int CurrentLevel => currentLevel;
        public int CurrentExp => currentExp;
        public Transform Transform => transform;
        public bool IsAlive => currentHealth > 0;
        public int ExpRequiredForNextLevel => GetExpRequiredForLevel(currentLevel + 1);

        private void Awake()
        {
            currentLevel = startLevel;
            currentExp = 0;
        }
        private void Start()
        {
            if (PlayerAttributes.Instance != null)
            {
                currentHealth = MaxHealth;
            }
            else
            {
                currentHealth = maxHealth;
            }

            GameEvents.PlayerGainExp(0, currentExp, currentLevel);

            sr = GetComponent<SpriteRenderer>();
            originalSprite = sr.sprite;
            yellowSprite = CreateYellowSilhouette();
        }

        public void RefreshHealthFromAttributes()
        {
            currentHealth = MaxHealth;
            GameEvents.PlayerTakeDamage(0, currentHealth, MaxHealth);
        }

        /// <summary>
        /// 受到伤害
        /// </summary>
        public void TakeDamage(float damage)
        {
            if (currentHealth <= 0) return;

            // 应用防御减伤
            float finalDamage = PlayerAttributes.Instance?.CalculateDamage(damage) ?? damage;

            currentHealth -= finalDamage;

            // 受击变色
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.DOColor(Color.red, 0.05f).OnComplete(() =>
                {
                    if (sr != null)
                        sr.DOColor(Color.white, 0.1f);
                });
            }

            // 震动：只有伤害超过最大生命值 15% 才震
            float damageRatio = finalDamage / MaxHealth;
            if (damageRatio > 0.15f)
            {
                CameraShake.Instance?.Shake(0.15f, 0.2f * damageRatio);
            }

            if (PlayerPrefs.GetInt("FlashEffect", 1) == 1)
            {
                sr.DOColor(Color.red, 0.05f).OnComplete(() => sr.DOColor(Color.white, 0.1f));
            }

            GameEvents.PlayerTakeDamage(finalDamage, currentHealth, MaxHealth);

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
        }

        /// <summary>
        /// 治疗
        /// </summary>
        public void Heal(float amount)
        {

            float oldHealth = currentHealth;
            currentHealth = Mathf.Min(MaxHealth, currentHealth + amount);

            // 血条 UI 更新
            if (currentHealth > oldHealth)
            {
                GameEvents.PlayerTakeDamage(0, currentHealth, MaxHealth);
            }
        }

        /// <summary>
        /// 获得经验
        /// </summary>
        public void GainExp(int amount)
        {
            if (amount <= 0) return;

            // 应用经验加成（成长 + 贪欲）
            float bonus = PlayerAttributes.Instance?.GetExpBonus() ?? 1f;
            int finalAmount = Mathf.RoundToInt(amount * bonus);

            currentExp += finalAmount;

            CheckLevelUp();  
            GameEvents.PlayerGainExp(finalAmount, currentExp, currentLevel);
        }

        /// <summary>
        /// 升级
        /// </summary>
        private void LevelUp()
        {
            currentLevel++;
            currentHealth = MaxHealth;

            CameraShake.Instance?.Shake(0.1f, 0.15f);

            GameEvents.PlayerLevelUp(currentLevel);
        }

        /// <summary>
        /// 获取指定等级所需经验
        /// </summary>
        private int GetExpRequiredForLevel(int level)
        {
            if (level <= 1) return 0;

            return Mathf.FloorToInt(baseExpRequired * Mathf.Pow(expGrowthFactor, level - 2));
        }

        private int GetMaxLevel()
        {
            return 100;
        }

        private void Die()
        {
            if (WeaponManager.Instance == null)
            {
                Debug.LogError("WeaponManager.Instance is null!");
                return;
            }

            // 收集本局数据，结算用
            GameStats stats = new GameStats
            {
                playTime = GetPlayTime(),
                goldEarned = GetGoldEarned(),
                kills = GetKills(),
                level = currentLevel
            };

            // 武器列表
            List<WeaponBase> weapons = new List<WeaponBase>();
            foreach (var w in WeaponManager.Instance.Weapons)
            {
                weapons.Add(w);
            }

            // 收集物列表
            List<CollectionRecord> collections = new List<CollectionRecord>();

            // 武器
            foreach (var w in WeaponManager.Instance.Weapons)
            {
                collections.Add(new CollectionRecord
                {
                    icon = w.GetWeaponData().icon,
                    level = w.GetCurrentLevel()
                });
            }

            // 被动
            foreach (var passive in PassiveManager.Instance.Passives)
            {
                collections.Add(new CollectionRecord
                {
                    icon = passive.data.icon,
                    level = passive.level
                });
            }

            // 三选一属性升级
            if (UpgradeManager.Instance != null && UpgradeManager.Instance.obtainedUpgrades != null)
            {
                foreach (var upgrade in UpgradeManager.Instance.obtainedUpgrades)
                {
                    collections.Add(new CollectionRecord
                    {
                        icon = upgrade.icon,
                        level = 1
                    });
                }
            }
            GameOverUI.Instance.Show(false, stats, weapons,collections);

            AudioManager.Instance?.PlayPlayerDeath();

            GameEvents.PlayerDeath();
            GameEvents.GameOver(false);
        }

        private float GetPlayTime()
        {
            GameTimer gameTimer = FindObjectOfType<GameTimer>();
            if (gameTimer != null)
            {
                return gameTimer.GetElapsedTime();
            }
            return Time.timeSinceLevelLoad;
        }

        private int GetGoldEarned()
        {
            GoldUI goldUI = FindObjectOfType<GoldUI>();
            if (goldUI != null)
            {
                return goldUI.GetCurrentGold();
            }
            return 0;
        }

        private int GetKills()
        {
            KillUI killUI = FindObjectOfType<KillUI>();
            if (killUI != null)
            {
                return killUI.GetCurrentKills();
            }
            return 0;
        }

        private Sprite CreateYellowSilhouette()  //升级时的特效
        {
            Texture2D originalTex = originalSprite.texture;
            Rect rect = originalSprite.rect;
            int x = (int)rect.x;
            int y = (int)rect.y;
            int width = (int)rect.width;
            int height = (int)rect.height;

            Color[] pixels = originalTex.GetPixels(x, y, width, height);
            Texture2D newTex = new Texture2D(width, height);

            for (int i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a > 0.1f)
                    newTex.SetPixel(i % width, i / width, new Color(1.2f, 1.1f, 0.9f, 1f));
                else
                    newTex.SetPixel(i % width, i / width, Color.clear);
            }
            newTex.Apply();

            Vector2 pivot = new Vector2(
                originalSprite.pivot.x / originalSprite.rect.width,
                originalSprite.pivot.y / originalSprite.rect.height
            );

            return Sprite.Create(newTex, new Rect(0, 0, width, height), pivot, originalSprite.pixelsPerUnit);
        }


        public void PlayLevelUpEffect()
        {
            if (sr != null && yellowSprite != null)
            {
                Animator anim = GetComponent<Animator>();
                if (anim != null) anim.enabled = false;

                sr.sprite = yellowSprite;

                DOVirtual.DelayedCall(0.4f, () =>
                {
                    if (sr != null)
                    {
                        sr.sprite = originalSprite;
                        if (anim != null) anim.enabled = true;
                    }
                });
            }
        }

        public void CheckLevelUp()
        {
            int targetExpForNext = GetExpRequiredForLevel(currentLevel + 1);
            while (currentExp >= targetExpForNext && currentLevel < GetMaxLevel())
            {
                currentExp -= targetExpForNext;
                LevelUp();
                targetExpForNext = GetExpRequiredForLevel(currentLevel + 1);
            }
        }

        public void WinGame()
        {
            GameStats stats = new GameStats
            {
                playTime = GetPlayTime(),
                goldEarned = GetGoldEarned(),
                kills = GetKills(),
                level = currentLevel
            };

            // 武器列表
            List<WeaponBase> weapons = new List<WeaponBase>();
            foreach (var w in WeaponManager.Instance.Weapons)
            {
                weapons.Add(w);
            }

            // 收集物列表
            List<CollectionRecord> collections = new List<CollectionRecord>();

            foreach (var w in weapons)
            {
                collections.Add(new CollectionRecord
                {
                    icon = w.GetWeaponData().icon,
                    level = w.GetCurrentLevel()
                });
            }

            foreach (var passive in PassiveManager.Instance.Passives)
            {
                collections.Add(new CollectionRecord
                {
                    icon = passive.data.icon,
                    level = passive.level
                });
            }

            GameOverUI.Instance.Show(true, stats, weapons, collections);
            AudioManager.Instance?.PlayVictoryMusic();

            GameEvents.GameOver(true);
        }
    }
}
