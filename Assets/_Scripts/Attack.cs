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

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // MELEE INPUT
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("isMelee");
        }

        // RANGED INPUT
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("isAttacking");
        }
    }

    // Called from animation event
    public void MeleeAttack()
    {
        if (attackPoint == null) return;

        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(
            attackPoint.transform.position, radius, enemies
        );

        foreach (var e in enemiesHit)
        {
            EnemyHealthScript health = e.GetComponent<EnemyHealthScript>();
            if (health != null)
            {
                health.TakeDamage(meleeDamage);
            }
        }
    }

    // Called from animation event
    public void FireBullet()
    {
        if (bullet == null || firePoint == null) return;

        // Spawn bullet
        GameObject newBullet = Instantiate(bullet, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();

        // Determine shooting direction from player scale
        bool facingRight = transform.localScale.x > 0;
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;

        // Apply force
        rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);

        // Auto-delete bullet
        Destroy(newBullet, 2f);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.transform.position, radius);
        }
    }
}
