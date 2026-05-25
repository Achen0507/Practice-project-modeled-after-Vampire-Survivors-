using Survivor.Data;
using Survivor.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Survivor.MainMenu
{
    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Instance { get; private set; }
        private HashSet<string> recordedWeapons = new HashSet<string>();

        [Header("成就弹窗")]
        [SerializeField] private GameObject toastPrefab;

        private List<AchievementData> achievements;

        private Dictionary<string, int> progress;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadAllAchievements();

            LoadProgress();
        }

        private void LoadAllAchievements()
        {
            AchievementData[] loaded = Resources.LoadAll<AchievementData>("Data/RecordData");
            System.Array.Sort(loaded, (a, b) => a.name.CompareTo(b.name));
            achievements = new List<AchievementData>(loaded);

            if (achievements.Count == 0)
            {
                Debug.LogError("没有找到成就数据！请把 AchievementData 文件放到 Assets/Resources/Achievements/ 文件夹");
            }
        }

        public List<AchievementData> GetAllAchievements()
        {
            return achievements;
        }

        public int GetProgress(AchievementData achievement)
        {
            return progress.ContainsKey(achievement.achievementName) ? progress[achievement.achievementName] : 0;
        }

        public bool IsCompleted(AchievementData achievement)
        {
            return GetProgress(achievement) >= achievement.targetValue;
        }

        public void AddProgress(AchievementType type,int amount,string enemyName="")
        {
            foreach (var ach in achievements)
            {
                if (ach.type == type && !IsCompleted(ach))
                {
                    if (type == AchievementType.TotalKills && ach.enemyName != enemyName) continue;

                    //如果是武器解锁成就，检查这个武器是否已经记录过
                    if (type == AchievementType.WeaponsUnlocked)
                    {
                        if (recordedWeapons.Contains(enemyName)) continue;

                        recordedWeapons.Add(enemyName);
                    }

                    int current = GetProgress(ach);
                    int newValue = Mathf.Min(current + amount, ach.targetValue);
                    progress[ach.achievementName] = newValue;
                    SaveProgress();

                    if (newValue >= ach.targetValue)
                    {
                        ShowToast(ach);
                        AudioManager.Instance?.PlayAchievementUnlock();
                    }              
                }
            }      
        }

        public void LoadProgress() {
            progress = new Dictionary<string, int>();
            recordedWeapons.Clear();  // 清空记录

            foreach (var ach in achievements)
            {
                int value = PlayerPrefs.GetInt($"Achievement_{ach.achievementName}", 0);
                progress[ach.achievementName] = value;
            }
        }

        private void SaveProgress() {
            foreach (var ach in achievements)
            {
                PlayerPrefs.SetInt($"Achievement_{ach.achievementName}", progress[ach.achievementName]);
            }
            PlayerPrefs.Save();
        }

        private void ShowToast(AchievementData achievement) {
            if (toastPrefab == null) return;

            Canvas canvas = GameObject.Find("UpgradeCanvas")?.GetComponent<Canvas>();
            if (canvas == null) return;

            GameObject toastObj = Instantiate(toastPrefab, canvas.transform);
            AchievementToast toast = toastObj.GetComponent<AchievementToast>();

            if (toast != null)
            {
                toast.Initialize(achievement);
            }
            Destroy(toastObj, 3f);
        }
    }
}
