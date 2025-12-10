using UnityEngine;

public class Fireball_Script : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;

    public float force = 5f;
    public int damage = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Fireball can't find player!");
            return;
        }

        // Calculate direction
        Vector3 direction = player.transform.position - transform.position;

        // Apply velocity toward player
        rb.linearVelocity = direction.normalized * force;

        // Rotate fireball to face movement direction
        float rot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }

    private void OnTriggerEnter2D(Collider2D other)
{
    Health playerHealth = other.GetComponent<Health>();
    if (playerHealth != null)
    {
        playerHealth.TakeDamage(1);
        Destroy(gameObject); // destroy fireball on hit
    }
}

}
