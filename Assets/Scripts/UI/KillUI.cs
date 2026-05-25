using Survivor.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Survivor.UI
{
    public class KillUI : MonoBehaviour
    {
        private Text killText;
        private int currentKills = 0;

        private void Start()
        {
            killText = GetComponent<Text>();
            UpdateKillDisplay();
            GameEvents.OnEnemyDeath += OnEnemyKilled;
        }
        private void OnDestroy()
        {
            GameEvents.OnEnemyDeath -= OnEnemyKilled;
        }

        private void OnEnemyKilled(GameObject enemy, Vector3 position)
        {
            currentKills++;
            UpdateKillDisplay();

            GoldManager.Instance?.AddKill();
        }
        private void UpdateKillDisplay()
        {
            if (killText != null)
            {
                killText.text = $" {currentKills}";
            }
        }

        public int GetCurrentKills() => currentKills;
    }
}
