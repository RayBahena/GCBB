using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 spawnPoint;

    void Start()
    {
        // Default starting point (before checkpoints)
        spawnPoint = transform.position;
    }

    public void SetCheckpoint(Vector3 newSpawnPoint)
    {
        spawnPoint = newSpawnPoint;
        Debug.Log("Checkpoint set to: " + spawnPoint);
    }

    public void RespawnPlayer()
    {
        transform.position = spawnPoint;
        // Reset player velocity if needed
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }
}
