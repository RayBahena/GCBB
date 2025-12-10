using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int NumberOfLives = 5;
    public int NumberOfDumplings = 0;
    public int costPerExtraLife = 50;

    public TextMeshProUGUI livesText;
    public TextMeshProUGUI DumplingsText;

    public AudioClip DumplingsClip;
    public AudioSource audioSource;

    public Vector3 spawnPoint;

    public GameObject gameOverUI;
    private bool isDead = false;

    void Start()
    {
        spawnPoint = new Vector3(-4f, -2f, 0);
        audioSource = GetComponent<AudioSource>();
        gameOverUI.SetActive(false);
    }

    void Update()
    {

        // Update dumplings UI
        DumplingsText.text = NumberOfDumplings.ToString();

        // Buy extra life
       while (NumberOfDumplings >= costPerExtraLife)
        {
            NumberOfDumplings -= costPerExtraLife;
            NumberOfLives++;
        }

        // Check death
        if (NumberOfLives <= 0 && !isDead)
        {
            isDead = true;
            gameObject.SetActive(false);
            gameOver();
        }
    }

    public void gameOver()
    {
        gameOverUI.SetActive(true);
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void quit()
    {
        Application.Quit();
    }
}
