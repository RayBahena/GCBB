using UnityEngine;
using UnityEngine.SceneManagement;

public class EpicFail : MonoBehaviour
{
    GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
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
    }
}
