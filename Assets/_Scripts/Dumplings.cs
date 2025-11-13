using UnityEngine;

public class Dumplings : MonoBehaviour
{
     GameManager gameManager;
    void Start()
    {
        gameManager = GameObject.Find("Canvas").GetComponent<GameManager>();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player")
        {
            gameManager.NumberOfDumplings++;
            // Code to give the player a key goes here
            gameManager.audioSource.clip = gameManager.DumplingsClip;
            gameManager.audioSource.Play();
            Destroy(gameObject);
        }
    }
}