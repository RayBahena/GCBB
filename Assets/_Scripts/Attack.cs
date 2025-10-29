using UnityEngine;

public class Attack : MonoBehaviour
{
   public GameObject bullet;
    public GameObject firePoint;

    Animator animator;

    public bool fireForward = true;
    public float bulletForce = 1500.0f;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput > 0)
        {
            fireForward = true;
        }
        else if (horizontalInput < 0)
        {
            fireForward = false;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("doAttack");
            FireBullet();
        }
    }

    void FireBullet()
    {
        // Bullet instantiate at the position of GameObject
        GameObject newBullet = Instantiate(bullet, firePoint.transform.position, firePoint.transform.rotation) as GameObject;

        // get Rigidbody2D component of instantiated Bullet
        Rigidbody2D tempRigidBody = newBullet.GetComponent<Rigidbody2D>();

        // push the Bullet forward by amount bulletForce
        if (fireForward)
        {
            // fireForward is fire to the right
            tempRigidBody.AddForce(transform.right * bulletForce);
        }
        else
        {
            // fire left, a.k.a. "negative right"
            tempRigidBody.AddForce(-transform.right * bulletForce);
        }

        // basic Clean Up, set Bullets to self destruct after 2 seconds
        Destroy(newBullet, 2.0f);
    }
}
