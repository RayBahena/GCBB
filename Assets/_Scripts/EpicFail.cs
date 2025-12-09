using UnityEngine;
using UnityEngine.SceneManagement;

public class EpicFail : MonoBehaviour
{
    GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameManager.NumberOfLives == 0)
            {
                // Reload the scene
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                // Lose a life
                gameManager.NumberOfLives--;

                // NEW CHECKPOINT SYSTEM:
                other.GetComponent<PlayerRespawn>().RespawnPlayer();
            }
        }
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
        }
    }
}
