using UnityEngine;

public class PickUp : MonoBehaviour
{
     GameManager gameManager;
    void Start()
    {
        gameManager = GameObject.Find("Canvas").GetComponent<GameManager>();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player")
        {
            gameManager.NumberOfCoins++;
            // Code to give the player a key goes here
            gameManager.audioSource.clip = gameManager.coinClip;
            gameManager.audioSource.Play();
            Destroy(gameObject);
        }
    }
}