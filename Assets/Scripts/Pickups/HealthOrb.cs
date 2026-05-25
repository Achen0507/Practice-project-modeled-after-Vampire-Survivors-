using Survivor.Player;
using UnityEngine;

namespace Survivor.Pickups
{
    /// <summary>
    /// 红心掉落
    /// </summary>
    
    public class HealthOrb : MonoBehaviour
    {
        [Header("恢复量")]
        [SerializeField] private int healAmount = 5;

        [Header("吸引效果")]
        [SerializeField] private float attractSpeed = 5f;
        [SerializeField] private float attractRange = 2f;

        private Transform playerTransform;
        private bool isAttracting = false;

        private void Start()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        private void Update()
        {
            if (playerTransform == null) return;

            float distance = Vector2.Distance(transform.position, playerTransform.position);

            if (!isAttracting && distance < attractRange) {
                isAttracting = true;
            }
            if (isAttracting)
            {
                Vector2 direction = (playerTransform.position - transform.position).normalized;
                transform.Translate(direction * attractSpeed * Time.deltaTime, Space.World);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                PlayerStats stats = collision.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    stats.Heal(healAmount);
                    AudioManager.Instance?.PlayHealthPickup();
                }
                Destroy(gameObject);
            }
        }
    }
}
