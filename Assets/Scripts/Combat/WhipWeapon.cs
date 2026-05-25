using Survivor.Core;
using Survivor.Player;
using UnityEngine;


namespace Survivor.Combat
{
    /// <summary>
    ///  效果更像Slash
    /// </summary>
    
    public class WhipWeapon : WeaponBase
    {
        [Header("剑气配置")]
        [SerializeField] private GameObject slashPrefab;     // 剑气预制体
        [SerializeField] private float baseRange = 1.5f;      
        [SerializeField] private float baseWidth = 0.8f;     
        [SerializeField] private float effectDuration = 0.4f;

        protected override void Attack()
        {
            // 获取升级后的数值
            float range = GetCurrentRange();
            float width = GetCurrentWidth();
            int count = GetSlashCount();

            SpawnSlashEffect(range, width, count);
            DealDamage(range, width, count);

            AudioManager.Instance?.PlayWhipAttack();
        }

        private Vector2 GetPlayerFacing()
        {
            PlayerController player = GetComponentInParent<PlayerController>();
            if (player != null)
            {
                return player.FacingDirection;
            }
            return Vector2.right;
        }

        private void SpawnSlashEffect(float range,float width,int count) {
            if (slashPrefab == null) return;

            Vector2 facing = GetPlayerFacing();
            // 特效生成在玩家前方，距离玩家1.5f的位置
            float offset = 1.5f;

            // 默认剑气角度：根据玩家朝向（左右移动时z轴的值需要改变）
            float defaultAngle = facing == Vector2.right ? 0f : 180f;

            if (count == 1)
            {
                Vector3 pos = transform.position + (Vector3)facing * (range / 2f + offset);
                CreateSlash(pos, range, width, defaultAngle);
            }
            else if (count == 2)
            {
                Vector3 front = transform.position + (Vector3)facing * (range / 2f + offset);
                Vector3 back = transform.position - (Vector3)facing * (range / 2f + offset);

                CreateSlash(front, range, width, defaultAngle);           // 左边：默认角度
                CreateSlash(back, range, width, defaultAngle + 180f);     // 右边：默认角度 + 180°
            }
        }

        private void CreateSlash(Vector3 pos, float range, float width,float zAngle)
        {
            GameObject slash = Instantiate(slashPrefab, pos, Quaternion.identity);
            slash.transform.rotation = Quaternion.Euler(0, 0, zAngle);
            slash.transform.localScale = new Vector3(range, width, 1);
            Destroy(slash, effectDuration);
        }

        private void DealDamage(float range, float width, int count)
        {
            Vector2 facing = GetPlayerFacing();
            float offset = 1.5f;

            if (count == 1)
            {
                Vector2 center = (Vector2)transform.position + facing * (range / 2f + offset);
                CheckDamage(center, new Vector2(range, width), facing);
            }
            else if (count == 2)
            {
                // 前方：方向 = facing
                Vector2 frontCenter = (Vector2)transform.position + facing * (range / 2f + offset);
                CheckDamage(frontCenter, new Vector2(range, width), facing);

                // 后方：方向 = -facing
                Vector2 backCenter = (Vector2)transform.position - facing * (range / 2f + offset);
                CheckDamage(backCenter, new Vector2(range, width), -facing);
            }
        }

        private void CheckDamage(Vector2 center, Vector2 size, Vector2 damageDirection)
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0);
            foreach (var hit in hits)
            {
                if (!hit.CompareTag("Enemy")) continue;

                // 用传入的方向判断，而不是 GetPlayerFacing
                Vector2 toEnemy = hit.transform.position - transform.position;
                if (Vector2.Dot(toEnemy, damageDirection) < 0) continue;

                IDamageable enemy = hit.GetComponent<IDamageable>();
                if (enemy != null && enemy.IsAlive)
                {
                    float damage = GetCurrentDamage();
                    enemy.TakeDamage(damage);
                    AddDamage(damage);

                    ShowDamageNumber(hit.transform.position, Mathf.RoundToInt(damage));
                }
            }
        }

        protected override float GetCurrentRange()
        {
            // 只从武器数据读取，不加全局加成
            return weaponData?.baseRange ?? 1.5f;
        }

        // 获取当前攻击宽度（升级后增加）
        private float GetCurrentWidth()
        {
            if (weaponData?.shotPatterns == null || weaponData.shotPatterns.Length == 0)
                return baseWidth;
            int level = GetCurrentLevel();
            int idx = Mathf.Min(level - 1, weaponData.shotPatterns.Length - 1);
            return weaponData.shotPatterns[idx].width;       
        }

        // 获取挥砍数量（升级后增加），最多2个剑风
        private int GetSlashCount()
        {
            if (weaponData?.shotPatterns == null || weaponData.shotPatterns.Length == 0)
                return 1;

            int level = GetCurrentLevel();
            int idx = Mathf.Min(level - 1, weaponData.shotPatterns.Length - 1);
            return weaponData.shotPatterns[idx].projectileCount;
        }

        public void SetWhip(GameObject prefab)
        {
            slashPrefab = prefab;
        }
    }
}
