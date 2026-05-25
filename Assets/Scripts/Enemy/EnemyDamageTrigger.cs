using Survivor.Enemy;
using Survivor.Player;
using UnityEngine;

public class EnemyDamageTrigger : MonoBehaviour
{
    private Enemy parentEnemy;
    private Animator parentAnimator;

    void Start()
    {
        parentEnemy = GetComponentInParent<Enemy>();
        parentAnimator = GetComponentInParent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parentAnimator?.SetTrigger("Attack");
            PlayerStats player = other.GetComponent<PlayerStats>();
            if (player != null && parentEnemy != null)
            {
                player.TakeDamage(parentEnemy.GetBaseDamage());
            }
        }
    }
}
