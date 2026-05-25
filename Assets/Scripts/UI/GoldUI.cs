using Survivor.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Survivor.UI
{
    public class GoldUI : MonoBehaviour
    {
        private Text goldText;
        private int currentGold = 0;

        private void Start()
        {
            goldText = GetComponent<Text>();
            UpdateGoldDisplay();
            GameEvents.OnGoldCollected += OnGoldGained;
        }

        private void OnDestroy()
        {
            GameEvents.OnGoldCollected -= OnGoldGained;
        }

        private void OnGoldGained(int amount)
        {
            currentGold += amount;
            UpdateGoldDisplay();
        }

        private void UpdateGoldDisplay()
        {
            if (goldText != null)
            {
                goldText.text = $"{currentGold}";
            }
        }

        public int GetCurrentGold() => currentGold;
    }
}
