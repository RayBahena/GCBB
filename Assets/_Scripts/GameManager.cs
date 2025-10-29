using System.Numerics;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //number to count
    public int NumberOfKeys = 0;
    public int NumberOfLives = 3;
    public int NumberOfCoins = 0;
    public int costPerExtraLife = 10;

    //texts to update in the UI
    public TextMeshProUGUI keyText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI coinsText;

    //sounds to play
    public AudioClip keyClip;
    public AudioClip coinClip;

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
        keyText.text = "Keys Found: " + NumberOfKeys;
        livesText.text = "Lives Left: " + NumberOfLives;
        coinsText.text = "Coins Collected: " + NumberOfCoins;

        // check coin count
        if (NumberOfCoins == costPerExtraLife)
        {
            NumberOfCoins = 0;
            NumberOfLives++;
        }
    }
}
