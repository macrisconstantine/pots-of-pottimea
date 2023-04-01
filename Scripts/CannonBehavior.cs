using System.Collections;
using UnityEngine;

public class CannonBehavior : MonoBehaviour
{
    GameObject gm;
    GameObject player;

    // Cannonball object is assigned from the editor
    [SerializeField] GameObject cannonBall;

    // Editor access created for modifying cannon atributes
    [SerializeField] float fireRate = 1;
    [SerializeField] float range = 8;
    [SerializeField] int hp = 100;

    // This bool determines whether canon will be able to rotate or not (cave cannons do not rotate)
    [SerializeField] bool isCaveCannon = false;

    // Variables to manage shooting rate and cannon state
    private float lastShootTime = 0;
    private bool wasDestroyed = false;
    
    // Retrieves player and game manager objects at start
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gm = GameObject.FindGameObjectWithTag("GM");
    }

    // Targetting and firing behavior is written in the update function
    void Update()
    {
        if (player != null)
        {
            // Assuming cannon is a regular cannon, target the player
            if (!isCaveCannon)
            {
                // Define dir as a vector with its one node at the cannon and one node at the player
                Vector3 dir = player.GetComponent<Transform>().position - transform.position;

                // Define angle according to direction defined above
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90;

                // Keep the cannon sprite on this angle every frame--in other words, keep the cannon always pointing at the player
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            
            // If the player is in range and the fire rate allows for another shot, the cannon fires
            if (Vector2.Distance(player.GetComponent<Transform>().position, transform.position) < range && Time.time > lastShootTime + 1/fireRate && !wasDestroyed)
            {
                // Temporarily disable the collider so the cannon does not shoot itself, and trigger the fire animation
                //GetComponent<Collider2D>().enabled = false;
                GetComponent<Animator>().SetTrigger("fire"); 
                
                if (isCaveCannon)
                {
                    // Cave cannons instantiate their cannonballs with with a small offset
                    Instantiate(cannonBall, GetComponent<Transform>().position + new Vector3 (0,.5f,0), Quaternion.identity);

                }
                else // Spawn cannonball at position of the cannon
                    Instantiate(cannonBall, GetComponent<Transform>().position, Quaternion.identity);

                // Set the shoot time for the fire rate timer and call the invincible time coroutine
                lastShootTime = Time.time;
                //StartCoroutine(InvincibleTime());
            }    
        }
    }

    // Re-enables the cannon's collider after a short delay
    IEnumerator InvincibleTime()
    {
        yield return new WaitForSeconds(.5f);
        GetComponent<Collider2D>().enabled = true;

    }

    // If collides with collider tagged player weapon and not already destroyed, take damage
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Cannonballs are tagged player weapon
        if (collision.gameObject.CompareTag("PlayerWeapon") && !wasDestroyed)
        {
            // Trigger damaged animation and subtract hp
            GetComponent<Animator>().SetTrigger("damaged");
            hp -= 25;

            // If hp reaches 0, kill cannon
            if (hp <= 0)
            {
                // This boolean was added to allow death animation to play out but not take damage
                wasDestroyed = true;

                // Rotation is locked so the death animation does not rotate
                GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

                //Trigger death animation and destroy object after a delay
                GetComponent<Animator>().SetTrigger("death");
                Destroy(gameObject,1f);
            }
        }
    }

    // Referenced by animation event to access item spawn function in Item Spawner script
    void SpawnItem()
    {
        if(gm != null)
        {
            gm.GetComponent<ItemSpawner>().SpawnItem(transform);
        }
    }
}
