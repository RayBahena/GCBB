using UnityEngine;

public class KeyPickUp : MonoBehaviour
{
    GameManager gameManager;
    private bool didCountKey = false;
    void Start()
    {
        gameManager = GameObject.Find("Canvas").GetComponent<GameManager>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !didCountKey)
        {
            gameManager.NumberOfKeys++;
            // Code to give the player a key goes here
            gameManager.audioSource.clip = gameManager.keyClip;
            gameManager.audioSource.Play();
            Debug.Log("Key Picked Up!");
            Destroy(gameObject); // Remove the key from the scene
        }
    }
}
