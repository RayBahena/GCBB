using UnityEngine;

public class EnemyHealthScript : MonoBehaviour , IDamageable
{
    [SerializeField] private float maxHealth = 1f;

    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Damage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
