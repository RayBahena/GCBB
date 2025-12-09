using UnityEngine;

public class Fuzzwitch_Script : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletPos;
    private float timer;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance < 4.0f) {
            timer += Time.deltaTime;
            if (timer > 2.0f){
                timer = 0.0f;
                shoot();
            }
        }
        
        
    }
    void shoot()
    {
        Instantiate(bullet, bulletPos.position, Quaternion.identity);
    }
}
