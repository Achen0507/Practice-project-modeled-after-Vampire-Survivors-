using UnityEngine;

public class TimeManager : MonoBehaviour
{
    /// <summary>
    /// 管理暂停和继续游戏的时间管理器
    /// </summary>
    public static TimeManager Instance { get; private set; }

    private int pauseCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddPause()
    {
        pauseCount++;
        Time.timeScale = 0f;
    }

    public void RemovePause() {
        pauseCount--;
        if (pauseCount <= 0) {
            pauseCount = 0;
            Time.timeScale = 1f;
        }
    }
}
