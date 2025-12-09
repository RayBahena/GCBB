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
    public float bulletForce = 1500f;

    private Animator animator;
    private SpriteRenderer sr;
    private bool fireForward = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Read facing direction from sprite flip (BEST way)
        fireForward = !sr.flipX;

        // Melee attack
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("isMelee");
        }

        // Ranged attack
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("isAttacking");
        }
    }

    // Called by animation event
    public void MeleeAttack()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius, enemies);

        foreach (var e in enemiesHit)
        {
            var health = e.GetComponent<EnemyHealthScript>();
            if (health != null)
                health.TakeDamage(meleeDamage);
        }
    }

    // Called by animation event
    public void FireBullet()
    {
        GameObject newBullet = Instantiate(bullet, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();

        Vector2 dir = fireForward ? Vector2.right : Vector2.left;
        rb.AddForce(dir * bulletForce);

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
