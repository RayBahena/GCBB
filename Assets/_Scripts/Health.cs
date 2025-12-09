using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int health = 5;
    public int maxHealth = 5;

    public Image[] croisants;
    public Sprite fullCroisant;
    public Sprite emptyCroisant;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        health = maxHealth;
    }

    void Update()
    {
        // Update croissant UI
        for (int i = 0; i < croisants.Length; i++)
        {
            if (i < health)
                croisants[i].sprite = fullCroisant;
            else
                croisants[i].sprite = emptyCroisant;

            croisants[i].enabled = (i < maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0) health = 0;

        if (health == 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (gameManager != null)
        {
            if (gameManager.NumberOfLives > 0)
            {
                gameManager.NumberOfLives--;
                health = maxHealth; // restore hearts for the new life
                GetComponent<PlayerRespawn>().RespawnPlayer();
            }
            else
            {
                gameManager.gameOver();
                gameObject.SetActive(false); // disable player
            }
        }
        else
        {
            Debug.Log("Player died but no GameManager found!");
        }
    }
}
