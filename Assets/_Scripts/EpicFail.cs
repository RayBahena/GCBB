using UnityEngine;
using UnityEngine.SceneManagement;

public class EpicFail : MonoBehaviour
{
    GameManager gameManager;
    public void Start()
    {
        gameManager = GameObject.Find("Canvas").GetComponent<GameManager>();
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player") 
        {

            if (gameManager.NumberOfLives == 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                gameManager.NumberOfLives--;
                other.gameObject.transform.position = gameManager.spawnPoint;
            }
            // write something to the Console just to make 
            // sure this function is being called


            // use SceneManager to load the CURRENT scene again (a reset)
            // the LoadScene function just wants a NUMBER of the scene to load
            
        }
    }

}