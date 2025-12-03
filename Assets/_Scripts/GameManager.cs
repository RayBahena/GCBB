using System.Numerics;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //number to count
    public int NumberOfLives = 5;
    public int NumberOfDumplings = 0;
    public int costPerExtraLife = 50;

    //texts to update in the UI
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI DumplingsText;

    //sounds to play
    public AudioClip DumplingsClip;

    public UnityEngine.Vector3 spawnPoint;
    public AudioSource audioSource;

    public GameObject gameOverUI;
    void Start()
    {
        spawnPoint = new UnityEngine.Vector3(-4f, -2f, 0);
        audioSource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        DumplingsText.text = NumberOfDumplings.ToString();


        // check coin count
        if (NumberOfDumplings >= costPerExtraLife){
            NumberOfDumplings -= costPerExtraLife; 
            NumberOfLives++;
        }

    }
    public void gameOver()
    {
        gameOverUI.SetActive(true);
    }
}
