using UnityEngine;

public class EpicHalfWin : MonoBehaviour
{
    GameManager gameManager;
    public Color startColor = Color.red;
    public Color endColor = Color.green;
    void Start()
    {
        gameManager = GameObject.Find("Canvas").GetComponent<GameManager>();
        GetComponent<SpriteRenderer>().color = startColor; 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            gameManager.spawnPoint = transform.position;
            GetComponent<SpriteRenderer>().color = endColor;
        }
    }
    
}
