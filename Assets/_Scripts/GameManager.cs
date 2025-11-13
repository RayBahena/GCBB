using System.Numerics;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //number to count
    public int NumberOfLives = 3;
    public int NumberOfDumplings = 0;
    public int costPerExtraLife = 10;

    //texts to update in the UI
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI DumplingsText;

    //sounds to play
    public AudioClip DumplingsClip;

    public UnityEngine.Vector3 spawnPoint;
    public AudioSource audioSource;

    
    void Start()
    {
        spawnPoint = new UnityEngine.Vector3(-4f, -2f, 0);
        audioSource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        livesText.text = "Lives Left: " + NumberOfLives;
        DumplingsText.text = "Coins Collected: " + NumberOfDumplings;

        // check coin count
        if (NumberOfDumplings == costPerExtraLife)
        {
            NumberOfDumplings = 0;
            NumberOfLives++;
        }
    }
}
