using UnityEngine;

public class EpicHalfWin : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite passive, active;
    private bool activated = false;
    Collider2D coll;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            var respawn = other.GetComponent<PlayerRespawn>();
            if (respawn != null)
            {
                respawn.SetCheckpoint(transform.position);
                activated = true;
                spriteRenderer.sprite = active;
                coll.enabled = false;
            }
        }
    }
}