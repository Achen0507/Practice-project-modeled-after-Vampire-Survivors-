using Survivor.Data;
using System;
using UnityEngine;


namespace Survivor.Core
{
    /// <summary>
    /// 全局事件中心 - 所有系统间通信通过这里解耦
    /// 用法: GameEvents.OnPlayerTakeDamage?.Invoke(damage);
    /// </summary>

    public static class GameEvents
    {
        // ========== 玩家相关 ==========

        /// <summary> 玩家受到伤害，参数：伤害值，当前血量，最大血量 </summary>
        public static event Action<float, float, float> OnPlayerTakeDamage;
        public static void PlayerTakeDamage(float damage, float currentHp, float maxHp) =>
            OnPlayerTakeDamage?.Invoke(damage, currentHp, maxHp);

        /// <summary> 玩家死亡 </summary>
        public static event Action OnPlayerDeath;
        public static void PlayerDeath() => OnPlayerDeath?.Invoke();

        /// <summary> 玩家获得经验，参数：经验值，当前总经验，当前等级 </summary>
        public static event Action<int, int, int> OnPlayerGainExp;
        public static void PlayerGainExp(int exp, int totalExp, int level) =>
            OnPlayerGainExp?.Invoke(exp, totalExp, level);

        /// <summary> 玩家升级，参数：新等级 </summary>
        public static event Action<int> OnPlayerLevelUp;
        public static void PlayerLevelUp(int newLevel) =>
            OnPlayerLevelUp?.Invoke(newLevel);


        // ========== 拾取相关 ==========

        /// <summary> 经验球被拾取 </summary>
        public static event Action<GameObject> OnExpOrbCollected;
        public static void ExpOrbCollected(GameObject orb) =>
            OnExpOrbCollected?.Invoke(orb);

        public static event Action<int> OnGoldCollected;
        public static void GoldCollected(int amount) => OnGoldCollected?.Invoke(amount);


        // ========== 战斗相关 ==========

        /// <summary> 敌人死亡，参数：敌人GameObject，位置 </summary>
        public static event Action<GameObject, Vector3> OnEnemyDeath;
        public static void EnemyDeath(GameObject enemy, Vector3 position) =>
            OnEnemyDeath?.Invoke(enemy, position);

        /// <summary> 敌人受到伤害 </summary>
        public static event Action<GameObject, float> OnEnemyTakeDamage;
        public static void EnemyTakeDamage(GameObject enemy, float damage) =>
            OnEnemyTakeDamage?.Invoke(enemy, damage);


        // ========== 升级相关 ==========

        /// <summary> 升级界面打开，参数：可用升级项列表 </summary>
        public static event Action<object> OnUpgradeMenuOpen;
        public static void UpgradeMenuOpen(object upgradeOptions) =>
            OnUpgradeMenuOpen?.Invoke(upgradeOptions);

        /// <summary> 升级项被选择 </summary>
        public static event Action<UpgradeData> OnUpgradeSelected;
        public static void UpgradeSelected(UpgradeData upgrade) =>
            OnUpgradeSelected?.Invoke(upgrade);


        // ========== 游戏状态 ==========

        /// <summary> 游戏开始 </summary>
        public static event Action OnGameStart;
        public static void GameStart() => OnGameStart?.Invoke();

        /// <summary> 游戏暂停 </summary>
        public static event Action<bool> OnGamePause;
        public static void GamePause(bool isPaused) => OnGamePause?.Invoke(isPaused);

        /// <summary> 游戏结束 </summary>
        public static event Action<bool> OnGameOver; // true=胜利, false=失败
        public static void GameOver(bool isWin) => OnGameOver?.Invoke(isWin);

        /// <summary> 局外数值改变 </summary>
        public static event Action OnPlayerStatsChanged;
        public static void PlayerStatsChanged() => OnPlayerStatsChanged?.Invoke();
    }
}
