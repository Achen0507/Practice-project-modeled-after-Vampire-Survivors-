using Survivor.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor.Upgrades
{
    /// <summary>
    /// 升级选项（单个选项的数据）
    /// </summary>
     [System.Serializable]
    public class UpgradeOption
    {
        public string title;
        public string description;
        public Sprite icon;
        public UpgradeData upgradeData;
        public Action onApply; // 实际应用升级的回调
    }
}
