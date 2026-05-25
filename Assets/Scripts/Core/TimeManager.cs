using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
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
