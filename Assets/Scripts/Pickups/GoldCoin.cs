using Survivor.Core;
using UnityEngine;

namespace Survivor.Pickups
{
    /// <summary>
    /// ½ð±ÒµôÂä
    /// </summary>
    
    public class GoldCoin : MonoBehaviour
    {
        [SerializeField] private int goldValue = 10;
        [SerializeField] private float attractSpeed = 5f;
        [SerializeField] private float attractRange = 1.5f;

        private Transform player;
        private bool isAttracting = false;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        private void Update()
        {
            if (player == null) return;

            float distance = Vector2.Distance(transform.position, player.position);

            if (!isAttracting && distance < attractRange) {
                isAttracting = true;
            }

            if (isAttracting) {
                Vector2 direction = (player.position - transform.position).normalized;
                transform.Translate(direction * attractSpeed * Time.deltaTime, Space.World);
            }
        }

        public void Initialize(int value)
        {
            goldValue = value;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                GameEvents.GoldCollected(goldValue);
                GoldManager.Instance?.AddGold(goldValue);
                AudioManager.Instance?.PlayCoinPickup();
                Destroy(gameObject);
            }
        }
    }
}
