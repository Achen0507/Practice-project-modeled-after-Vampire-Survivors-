using Survivor.Core;
using Survivor.Player;
using UnityEngine;

namespace Survivor.Pickups
{
    public class ExpOrb : MonoBehaviour, IPoolResettable
    {
        [SerializeField] private int expValue = 10;
        [SerializeField] private float attractSpeed = 5f;
        [SerializeField] private float attractRange = 2f;

        private Transform player;
        private bool isAttracting = false;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
        public void Initialize(int value, Vector3 position)
        {
            expValue = value;
            transform.position = position;
            isAttracting = false;
        }

        public void OnGetFromPool()
        {
            // 重置状态
            isAttracting = false;
        }

        private Transform GetPlayer()
        {
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player")?.transform;
            return player;
        }

        private void Update()
        {
            Transform currentPlayer = GetPlayer();
            if (currentPlayer == null) return;
            float distance = Vector2.Distance(transform.position, currentPlayer.position);

            // 进入吸引范围
            if (!isAttracting && distance < attractRange)
            {
                isAttracting = true;
            }

            if (isAttracting)
            {
                Vector2 direction = (player.position - transform.position).normalized;
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
                    stats.GainExp(expValue);
                    AudioManager.Instance?.PlayExpPickup();
                }

                // 归还到对象池
                var returnable = GetComponent<PoolReturnable>();
                returnable?.ReturnToPool();
            } 
        }
    }
}
