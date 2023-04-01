using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallBehavior : MonoBehaviour
{
    GameObject player;
    AudioManager am;
    GameObject gm;
    Vector2 dir;

    // Serialized fields to specify deflect effect and cannonball attributes
    [SerializeField] GameObject effect;
    [SerializeField] bool isCaveCannonBall = false;
    [SerializeField] float speed = 3f;
    [SerializeField] int damage = 2;
    [SerializeField] int fireDir = 0;
    bool collided = false;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM");
        am = FindObjectOfType<AudioManager>();
        player = GameObject.FindGameObjectWithTag("Player");

        // If statement allows for distinct behavior for different cannon balls while using the same script
        if (player != null && !isCaveCannonBall)
        {
            // Cave cannons fire only in set directions, while non-cave cannons move in the direction of the player
            dir = (player.GetComponent<Transform>().position - transform.position) + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f),0);
        }
        else if (fireDir == 0)
        {
            dir = new Vector2(0, 1f);
        }
        else if (fireDir == 1)
        {
            dir = new Vector2(0, -1f);
        }
        else if (fireDir == 2)
        {
            dir = new Vector2(1f, 0);
        }
        else if (fireDir == 3)
        {
            dir = new Vector2(-1f, 0);
        }
        Destroy(gameObject, 5); 
    }

    // Move the cannonball only until collision
    void Update()
    {
        if (!collided)
            transform.Translate(dir.normalized * speed * Time.deltaTime);
    }

    // Deflect if collision with player shield
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerShield"))
        {
            if (am != null) am.Play("Deflect");

            // Spawn deflect effect
            Instantiate(effect, collision.GetComponent<Transform>().position + new Vector3(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f)), Quaternion.identity);

            // Deflect ball with a little variation in angle for more natural feel
            dir = -dir + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

            // Change the tag of the cannon ball to allow the ball to damage the cannon
            gameObject.tag = "PlayerWeapon";
        }
    }

    // Damage the player on collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (gm != null)
                gm.GetComponent<PlayerHealth>().DamagePlayer(damage);
            if (am != null) am.Play("ShortHurtGrunt");
            collided = true;

            // Disable collider to prevent multiple collisions, trigger death animation of cannon ball, and destroy the object after a delay
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Animator>().SetTrigger("death");
            Destroy(gameObject, 3f);
        }
        

    }
}
