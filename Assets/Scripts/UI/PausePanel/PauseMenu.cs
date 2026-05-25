using UnityEngine;
using UnityEngine.SceneManagement;

namespace Survivor.UI
{
    /// <summary>
    /// 局内esc暂停面板
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [Header("面板")]
        [SerializeField] private GameObject pausePanel;

        public static PauseMenu Instance { get; private set; }
        public bool IsPaused => isPaused;

        private bool isPaused = false;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                    ResumeGame();
                else
                    PauseGame();
            }
        }

        public void PauseGame()
        {
            isPaused = true;
            pausePanel.SetActive(true);
            TimeManager.Instance.AddPause();
            AudioManager.Instance?.PauseMusic();
        }

        public void ResumeGame() {
            isPaused = false;
            pausePanel.SetActive(false);
            TimeManager.Instance.RemovePause();
            AudioManager.Instance?.ResumeMusic();
        }

        public void RestartGame() {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
