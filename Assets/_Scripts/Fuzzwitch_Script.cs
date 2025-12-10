using UnityEngine;
using System.Collections;

public class Fuzzwitch_Script : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletPos;

    private Animator animator;
    private SpriteRenderer sr;
    private GameObject player;

    private bool isAttacking = false;

    // Animation clip durations (auto–detected)
    private float makingLength;
    private float throwingLength;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        // Read actual animation lengths from Animator
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "makingFireball")
                makingLength = clip.length;

            if (clip.name == "throwingFireball")
                throwingLength = clip.length;
        }
    }

    void Update()
    {
        if (player == null) return;

        FacePlayer();

        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (!isAttacking && distance < 4f)
            StartCoroutine(AttackSequence());
    }

    void FacePlayer()
    {
        // Fix for "getting stuck facing left":
        // Flip only on X and only when player is not at exact same X
        float diff = player.transform.position.x - transform.position.x;

        if (diff > 0.05f)
            sr.flipX = false;  // face right
        else if (diff < -0.05f)
            sr.flipX = true;   // face left
    }

    IEnumerator AttackSequence()
    {
        isAttacking = true;

        // Play making animation
        animator.SetTrigger("makingFireball");
        yield return new WaitForSeconds(makingLength);

        // Play throwing animation
        animator.SetTrigger("throwingFireball");
        yield return new WaitForSeconds(throwingLength);

        // Shoot
        Instantiate(bullet, bulletPos.position, Quaternion.identity);

        // Small cooldown (optional)
        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }
}
