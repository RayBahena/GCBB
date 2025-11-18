using UnityEngine;

public class Dumplings : MonoBehaviour
{
    GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.NumberOfDumplings++;

            if (gameManager.audioSource && gameManager.DumplingsClip)
            {
                gameManager.audioSource.PlayOneShot(gameManager.DumplingsClip);
            }

            Destroy(gameObject);
        }
    }
}
