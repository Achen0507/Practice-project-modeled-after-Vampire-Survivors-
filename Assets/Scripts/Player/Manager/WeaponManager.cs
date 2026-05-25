using Survivor.Combat;
using Survivor.Data;
using Survivor.MainMenu;
using System.Collections.Generic;
using UnityEngine;


namespace Survivor.Player
{
    public class WeaponManager : MonoBehaviour
    {
        public static WeaponManager Instance { get; private set; }

        private GameObject damageTextPrefab;
        private Transform canvasTransform;

        [Header("武器列表")]
        [SerializeField] private List<WeaponBase> weapons = new List<WeaponBase>();

        // 武器数据（用于 UI 显示和升级）
        public IReadOnlyList<WeaponBase> Weapons => weapons;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            RefreshWeaponList();
        }


        public void SetDamageTextPrefab(GameObject prefab, Transform canvas)
        {
            this.damageTextPrefab = prefab;
            this.canvasTransform = canvas;

            // 传给所有武器
            foreach (var weapon in weapons)
            {
                weapon.SetDamageTextPrefab(prefab);
                weapon.SetCanvasTransform(canvas);
            }
        }

        /// <summary>
        /// 刷新武器列表（用于动态添加武器时）
        /// </summary>
        public void RefreshWeaponList() {
            weapons.Clear();
            var found = GetComponentsInChildren<WeaponBase>(true);
            Debug.Log($"找到 {found.Length} 个武器");
            foreach (var w in found)
            {
                Debug.Log($"武器: {w.name}");
            }
            weapons.AddRange(found);
        }

        /// <summary>
        /// 添加新武器
        /// </summary>
        public void AddWeapon(WeaponBase weapon) {
            
            string weaponName = weapon.GetWeaponData().weaponName;
            Debug.Log($"AddWeapon 被调用，武器数量: {weapons.Count + 1}");
            if (weapons.Contains(weapon)) return;

            weapons.Add(weapon);
            weapon.transform.SetParent(transform);

            // 解锁收藏
            CollectionItem item = GetCollectionItemForWeapon(weapon);
            if (item != null)
            {
                CollectionManager.Instance?.UnlockItem(item);
            }

            AchievementManager.Instance?.AddProgress(AchievementType.WeaponsUnlocked, 1, weaponName);

            // 添加时立即设置伤害数字预制体
            if (damageTextPrefab != null)
            {
                weapon.SetDamageTextPrefab(damageTextPrefab);
                weapon.SetCanvasTransform(canvasTransform);
            }        
        }

        private CollectionItem GetCollectionItemForWeapon(WeaponBase weapon)
        {
            string weaponName = weapon.GetWeaponData().weaponName;

            CollectionItem item = Resources.Load<CollectionItem>($"Data/CollectionData/{weaponName}");

            if (item == null)
            {
                // 如果找不到，尝试遍历所有 CollectionItem
                CollectionItem[] all = Resources.LoadAll<CollectionItem>("Data/CollectionData");
                foreach (var ci in all)
                {
                    // 去掉开头的数字，如 "01爱之箭" → "爱之箭"
                    string cleanName = System.Text.RegularExpressions.Regex.Replace(ci.name, @"^\d+", "");
                    if (cleanName == weaponName)
                    {
                        Debug.Log($"找到收藏品: {ci.name} -> {cleanName}");
                        return ci;
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// 获取武器当前等级
        /// </summary>
        public int GetWeaponLevel(WeaponBase weapon) {
            return weapons != null ? weapon.GetCurrentLevel() : 0;
        }

        /// <summary>
        /// 升级武器
        /// </summary>
        public void UpgradeWeapon(WeaponBase weapon) {
            if (weapon != null && weapons.Contains(weapon))
            {
                weapon.Upgrade();
                Debug.Log($"武器升级: {weapon.name} -> Lv.{weapon.GetCurrentLevel()}");
            }
        }

        /// <summary>
        /// 获取武器图标（用于 UI）
        /// </summary>
        public Sprite GetWeaponIcon(WeaponBase weapon) {
            if (weapon == null) return null;
            var data = weapon.GetWeaponData();
            return data != null ? data.icon : null;
        }

        public bool CanAddNewWeapon() {
            return weapons.Count < 6;
        }
    }
}
