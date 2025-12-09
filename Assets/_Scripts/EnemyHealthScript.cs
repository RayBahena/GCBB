using UnityEngine;

public class EnemyHealthScript : MonoBehaviour
{
    public int maxHealth = 1;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Detect bullet hits
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            TakeDamage(1); // Subtract 1 health per bullet hit
            Destroy(collision.gameObject); // Remove the bullet
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Play death animation, effects, etc. here
        Destroy(gameObject); // Remove the enemy from the scene
    }
}
