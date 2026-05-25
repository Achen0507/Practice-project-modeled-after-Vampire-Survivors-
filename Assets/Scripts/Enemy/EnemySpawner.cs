using Survivor.Cam;
using Survivor.Core;
using Survivor.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("生成配置")]
        [SerializeField] private List<string> enemyTypeNames;        // 敌人类型列表
        private List<EnemyData> enemyTypes;
        [SerializeField] private float baseSpawnInterval = 2f;      // 生成间隔（秒）
        [SerializeField] private float spawnRadius = 8f;
        [SerializeField] private GameObject enemyPrefab;

        [Header("数量限制")]
        [SerializeField] private int minMaxEnemies = 20;           // 初始最大敌人数
        [SerializeField] private int maxMaxEnemies = 60;   // 最终最大敌人数
        [SerializeField] private float maxTimeForMax = 1800f; // 30分钟达到最大

        [Header("Boss 设置")]
        [SerializeField] private string[] bossDataNames;
        private EnemyData[] bossDatas;

        [SerializeField] private float[] bossSpawnTimes = { 300, 600, 900, 1200 };  // 5、10、15、20 分钟

        private float nextBossTime;
        private int currentBossIndex = 0;

        private Transform player;
        private float spawnTimer;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            spawnTimer = GetCurrentSpawnInterval();

            if (bossSpawnTimes.Length > 0) {
                nextBossTime = bossSpawnTimes[0];
            }

            LoadEnemyTypes();
            LoadBossDatas();
        }
        private void LoadEnemyTypes()
        {
            enemyTypes = new List<EnemyData>();
            foreach (string name in enemyTypeNames)
            {
                EnemyData data = Resources.Load<EnemyData>($"Data/{name}");
                if (data != null)
                    enemyTypes.Add(data);
                else
                    Debug.LogError($"找不到敌人数据: {name}");
            }
        }

        private void LoadBossDatas()
        {
            bossDatas = new EnemyData[bossDataNames.Length];
            for (int i = 0; i < bossDataNames.Length; i++)
            {
                bossDatas[i] = Resources.Load<EnemyData>($"Data/{bossDataNames[i]}");
            }
        }

        private void Update() {
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player")?.transform;

            if (player == null) return;

            // 计时
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0) {
                // 检查当前敌人数
                int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

                if (currentEnemies < GetCurrentMaxEnemies())
                {
                    SpawnEnemy();
                }

                // 重置计时器，随时间逐渐加快生成
                spawnTimer = GetCurrentSpawnInterval();
            }

            // Boss 生成
            if (currentBossIndex < bossSpawnTimes.Length && Time.timeSinceLevelLoad >= nextBossTime) {
                SpawnBoss(currentBossIndex);
                currentBossIndex++;
                if (currentBossIndex < bossSpawnTimes.Length)
                {
                    nextBossTime = bossSpawnTimes[currentBossIndex];
                }
            }
        }

        private void SpawnBoss(int bossIndex) {
            if (bossIndex >=bossDatas.Length) return;

            AudioManager.Instance?.PlayBossMusic();

            EnemyData bossData =bossDatas[bossIndex];

            // Boss 在玩家稍远处生成
            Vector2 randomOffset = Random.insideUnitCircle.normalized * spawnRadius * 1.5f;
            Vector3 spawnPos = player.position + new Vector3(randomOffset.x, randomOffset.y, 0);

            GameObject bossObj = ObjectPool.Instance.Get("Enemy", spawnPos, Quaternion.identity);
            if (bossObj == null) return;

            Enemy boss = bossObj.GetComponent<Enemy>();
            boss?.Initialize(bossData);

            // 让 Boss 变大
            bossObj.transform.localScale = Vector3.one * 2.5f;
            Debug.Log($"第{bossIndex + 1}个 Boss 出现: {bossData.enemyName} 于 {Time.timeSinceLevelLoad:F0} 秒！");

            // Boss 出场震动
            CameraShake.Instance?.Shake(0.3f,0.4f);
        }


        /// <summary>
        /// 获取当前最大敌人数（平滑增长）
        /// </summary>
        private int GetCurrentMaxEnemies() {
            float time = Time.timeSinceLevelLoad;
            float t = Mathf.Clamp01(time / maxTimeForMax);
            return Mathf.RoundToInt(Mathf.Lerp(minMaxEnemies, maxMaxEnemies, t));
        }

        /// <summary>
        /// 获取当前生成间隔（随时间变短，生成更快）
        /// </summary>
        private float GetCurrentSpawnInterval() {
            float time = Time.timeSinceLevelLoad;
            // 基础间隔 2 秒，30 分钟后降到 0.5 秒
            float newInterval = baseSpawnInterval * (1 - Mathf.Clamp01(time / maxTimeForMax) * 0.75f);
            return Mathf.Max(0.2f, newInterval);
        }

        private List<EnemyData> GetAvailableEnemies()
        {
            List<EnemyData> available = new List<EnemyData>();
            float currentTime = Time.timeSinceLevelLoad;

            foreach (var enemy in enemyTypes)
            {
                if (currentTime >= enemy.unlockTime)
                {
                    available.Add(enemy);
                }
            }
            return available;
        }

        private EnemyData GetRandomEnemyByWeight()
        {
            List<EnemyData> available = GetAvailableEnemies();
            if (available.Count == 0) return enemyTypes[0];

            float time = Time.timeSinceLevelLoad;
            float timeFactor = Mathf.Min(1f, time / 1200f);  // 20分钟封顶

            // 计算总权重
            float totalWeight = 0;
            foreach (var e in available)
            {
                float weight = e.spawnWeight * (1 + timeFactor * e.difficultyMultiplier);
                totalWeight += weight;
            }

            // 随机抽取
            float randomWeight = Random.Range(0, totalWeight);
            float currentWeight = 0;

            foreach (var e in available)
            {
                float weight = e.spawnWeight * (1 + timeFactor * e.difficultyMultiplier);
                currentWeight += weight;
                if (randomWeight < currentWeight)
                    return e;
            }
            return available[0];
        }

        private void SpawnEnemy() {
            if (enemyTypes == null || enemyTypes.Count == 0) 
            {
                Debug.LogError("没有配置敌人预制体");
                return;
            }

            // 根据权重选择敌人
            EnemyData selected = GetRandomEnemyByWeight();
            if (selected == null) return;

            // 在玩家周围随机位置生成
            Vector2 randomOffset = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 spawnPos = player.position + new Vector3(randomOffset.x, randomOffset.y, 0);

            // 从对象池获取或直接实例化
            GameObject enemyObj = ObjectPool.Instance.Get("Enemy", spawnPos, Quaternion.identity);
            if (enemyObj == null)
            {
                Debug.LogWarning("对象池中没有 Enemy，使用 Instantiate");
                enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            }
            else if (enemyObj.gameObject == null)
            {
                Debug.LogError("获取到的 Enemy 已被销毁");
                enemyObj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            }

            // 用数据初始化
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy?.Initialize(selected);

            // 生成敌人后，注册到管理器
            IDamageable enemyDamageable = enemyObj.GetComponent<IDamageable>();
            if (enemyDamageable != null)
            {
                EnemyManager.Instance?.RegisterEnemy(enemyDamageable);
            }
        }
    }
}
