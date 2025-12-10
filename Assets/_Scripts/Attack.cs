using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("Melee Settings")]
    public GameObject attackPoint;
    public float radius = 1f;
    public LayerMask enemies;
    public int meleeDamage = 1;

    [Header("Ranged Settings")]
    public GameObject bullet;
    public Transform firePoint;
    public float bulletForce = 800f;

    [Header("Debug")]
    public bool showDebugLogs = true;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        
        // Check setup on start
        if (attackPoint == null)
            Debug.LogWarning("AttackPoint is not assigned!");
        if (bullet == null)
            Debug.LogWarning("Bullet prefab is not assigned!");
        if (firePoint == null)
            Debug.LogWarning("FirePoint is not assigned!");
    }

    void Update()
    {
        // MELEE INPUT
        if (Input.GetMouseButtonDown(0))
        {
            if (showDebugLogs) Debug.Log("Left mouse clicked - Melee triggered");
            animator.SetTrigger("isMelee");
            
            // TEMPORARY: Call melee directly to test (remove this once animation event works)
            // Invoke("MeleeAttack", 0.3f); // Adjust delay to match animation timing
        }

        // RANGED INPUT
        if (Input.GetMouseButtonDown(1))
        {
            if (showDebugLogs) Debug.Log("Right mouse clicked - Ranged triggered");
            animator.SetTrigger("isAttacking");
        }
    }

    // Called from animation event
    public void MeleeAttack()
    {
        if (showDebugLogs) Debug.Log("MeleeAttack() called!");
        
        if (attackPoint == null)
        {
            Debug.LogError("AttackPoint is null! Cannot perform melee attack.");
            return;
        }

        // Check for enemies in range
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(
            attackPoint.transform.position, 
            radius, 
            enemies
        );

        if (showDebugLogs) 
            Debug.Log($"Found {enemiesHit.Length} enemies in melee range");

        foreach (var e in enemiesHit)
        {
            if (showDebugLogs) 
                Debug.Log($"Hit enemy: {e.gameObject.name}");
            
            EnemyHealthScript health = e.GetComponent<EnemyHealthScript>();
            if (health != null)
            {
                health.TakeDamage(meleeDamage);
                if (showDebugLogs) 
                    Debug.Log($"Dealt {meleeDamage} damage to {e.gameObject.name}");
            }
            else
            {
                Debug.LogWarning($"Enemy {e.gameObject.name} has no EnemyHealthScript!");
            }
        }
    }

    // Called from animation event
    public void FireBullet()
    {
        if (showDebugLogs) Debug.Log("FireBullet() called!");
        
        if (bullet == null || firePoint == null)
        {
            Debug.LogError("Bullet or FirePoint is null!");
            return;
        }

        // Spawn bullet
        GameObject newBullet = Instantiate(bullet, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Bullet prefab is missing Rigidbody2D!");
            return;
        }

        // Determine shooting direction from player scale
        bool facingRight = transform.localScale.x > 0;
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;

        if (showDebugLogs) 
            Debug.Log($"Firing bullet in direction: {direction}");

        // Apply force
        rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);

        // Auto-delete bullet
        Destroy(newBullet, 2f);
    }

    void OnDrawGizmos()
    {
        // Always show attack range in scene view
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.transform.position, radius);
        }
    }
}