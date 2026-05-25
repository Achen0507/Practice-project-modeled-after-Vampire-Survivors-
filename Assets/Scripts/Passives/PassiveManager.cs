using Survivor.Data;
using Survivor.MainMenu;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor.Passives
{
    /// <summary>
    /// 管理局内Passive
    /// </summary>
    
    public class PassiveManager : MonoBehaviour
    {
        public static PassiveManager Instance { get; private set; }

        [SerializeField] private List<PassiveInstance> passives = new List<PassiveInstance>();

        public IReadOnlyList<PassiveInstance> Passives => passives;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// 添加或升级被动技能
        /// </summary>
        public void AddPassive(PassiveData data) {
            PassiveInstance existing = passives.Find(p => p.data != null && p.data.passiveName == data.passiveName);

            float increment;  // 本次增加的数值

            if (existing != null) {
                float oldValue = existing.GetCurrentValue();
                existing.LevelUp();
                float newValue = existing.GetCurrentValue();
                increment = newValue - oldValue;
            }
            else{
                passives.Add(new PassiveInstance(data));
                increment = data.baseValue;

                // 解锁武器收藏
                CollectionItem item = GetCollectionItemForPassive(data);
                if (item != null)
                {
                    CollectionManager.Instance?.UnlockItem(item);
                }
            }
            // 只增加本次的增量
            PlayerAttributes.Instance.ApplyStatModifier(data.statType, increment, data.isPercentage);
        }

        private CollectionItem GetCollectionItemForPassive(PassiveData data)
        {
            return Resources.Load<CollectionItem>($"Data/CollectionData/{data.passiveName}");
        }

        /// <summary>
        /// 获取被动技能等级
        /// </summary>
        public int GetPassiveLevel(PassiveData data) {
            PassiveInstance existing = passives.Find(p => p.data != null && p.data.passiveName == data.passiveName);
            return existing?.level ?? 0;
        }
    }
}
