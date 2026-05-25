using Survivor.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Survivor.UI
{
    public class GameTimer : MonoBehaviour
    {
        [SerializeField] private Text timerText;

        private float elapsedTime;
        private bool isRunning = true;

        private void Start() {
            if (timerText == null) timerText = GetComponent<Text>();
        }

        private void Update()
        {
            if (!isRunning) return;

            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();

            // 胜利条件：30 分钟
            if (elapsedTime >= 1200f)
            {
                PlayerStats playerStats = FindObjectOfType<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.WinGame();
                    isRunning = false; // 停止计时
                }
            }
        }

        private void UpdateTimerDisplay() {
            if (timerText == null) return;

            int hours = Mathf.FloorToInt(elapsedTime / 3600f);
            int minutes = Mathf.FloorToInt((elapsedTime % 3600f) / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);

            string display = $"{hours:00}:{minutes:00}:{seconds:00}";

            timerText.text = display; 
        }

        public void Pause() {
            isRunning = false;
        }

        public void Resume()
        {
            isRunning = true;
        }

        public void Reset()
        {
            elapsedTime = 0f;
            UpdateTimerDisplay();
        }

        public float GetElapsedTime() => elapsedTime;
    }
}
