using Survivor.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor.Passives
{
    /// <summary>
    /// 被动技能实例（玩家拥有）
    /// </summary>
    public class PassiveInstance
    {
        public PassiveData data;
        public int level = 1;

        public PassiveInstance(PassiveData data) {
            this.data = data;
            this.level = 1;
        }

        public void LevelUp() {
            if (level < data.maxLevel) level++;
        }

        public float GetCurrentValue() {
            return data.GetValueAtLevel(level);
        }
    }
}
