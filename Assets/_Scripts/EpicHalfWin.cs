using UnityEngine;

public class EpicHalfWin : MonoBehaviour
{
    private bool activated = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !activated)
        {
            other.GetComponent<PlayerRespawn>().SetCheckpoint(transform.position);
            activated = true;
        }
    }
}
