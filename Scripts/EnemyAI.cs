using System.Collections;
using UnityEngine;

// Namespace pathfinding used to access A*'s scripts used to control enemy movment
namespace Pathfinding {

    // This class handles player-enemy interactions
    public class EnemyAI : MonoBehaviour
    {
        AudioManager am;
        GameObject player;

        // Editor-assignment of sound names and animated effects for hitting or blocking
        [SerializeField] GameObject hitEffect;
        [SerializeField] GameObject blockEffect;
        [SerializeField] string enemyHitSound = "KeeseHit";
        [SerializeField] string enemyDeathSound = "KeeseDie";

        // Variables to control enemy interaction
        [SerializeField] float weaponDamage = 40f;
        [SerializeField] float enemyKnockBack = 2f;
        [SerializeField] float playerKnockBack = 1000f;
        [SerializeField] float speed;

        // Sets distance at which an enemy will begin to target the player
        [SerializeField] [Range(1f,10f)] float distanceToPlayer = 3f;

        // Allows for enemy behavior that does not target the player
        [SerializeField] bool onlyPatrol = false;
        [SerializeField] bool onlyRoam = false;
        //[SerializeField] bool changesDirection = false;
        //[SerializeField] bool onlyRoamHorizontal = false;

        int dx;
        int dy;
        bool isMoving = false;
        bool isStuck = false;
        Vector2 moveDelta;

        void Start()
        {
            // Sets enemies to patrol by default
            if (GetComponent<AIDestinationSetter>())
                GetComponent<AIDestinationSetter>().enabled = false;
            GetComponent<Patrol>().enabled = false;
            
            // Assigning player and audiomanager
            am = FindObjectOfType<AudioManager>();
            player = GameObject.FindWithTag("Player");
        }

        void Update()
        {
            // Sets enemy ai behavior to AI destination setter as a default when player is in range
            if (player != null && !onlyPatrol && !onlyRoam)
            {
                // Disabled patrol script and enables script that allows enemy to target and approach the player
                float distance = Vector2.Distance(transform.position, player.transform.position);
                if (distance < distanceToPlayer)
                {
                    GetComponent<AIDestinationSetter>().enabled = true;
                    GetComponent<Patrol>().enabled = false;
                }
                // If then enemy is out of range of the player, enable patrol script again and disable ai destination setting script
                else
                {
                    GetComponent<AIDestinationSetter>().enabled = false;
                    GetComponent<Patrol>().enabled = true;
                }
            }
            // Allows for simple enemy movement not dependant on A* pathfinding
            if (onlyRoam)
            {
                if (!isMoving)
                {
                    StartCoroutine(ChangeDirection());
                }
                GetComponent<Rigidbody2D>().velocity = (moveDelta * speed);
            }
        }

        // Selects random direction from NSEW as options
        void ChooseDirection()
        {
            // 50 percent chance to move on x or y axis
            if (Random.Range(0, 100) < 50)
            {
                dy = 0;
                // 50 percent chance to move in positive direction on selected axis
                if (Random.Range(0, 100) < 50)
                {
                    dx = -1;
                }
                else dx = 1;
            }
            else
            {
                dx = 0;
                if (Random.Range(0, 100) < 50)
                {
                    dy = -1;
                }
                else dy = 1;
            }
            // Apply movement to object
            moveDelta = new Vector3(dx, dy, 0).normalized;
        }

        // Changes direction every random length interval of time
        IEnumerator ChangeDirection()
        {
            isMoving = true;
            ChooseDirection();
            yield return new WaitForSeconds(Random.Range(1, 5));
            isMoving = false;
        }

        // If the enemy enters the trigger of a player weapon, shield, or roll, play the appropriate sounds/effects
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Keep enemies on platforms
            if (onlyRoam)
            {
                moveDelta = -moveDelta;
            }
            // Set direction as from the player weapon to the enemy
            Vector2 direction = (transform.position - collision.GetComponent<Transform>().position).normalized;

            if (collision.CompareTag("PlayerWeapon"))
            {
                // Cut sound if enters trigger of player weapon
                if (am != null) am.Play("Cut");

                // Call function to subtract weapon damage from the enemies current health
                GetComponent<EnemyStats>().DamageEnemy(weaponDamage);

                // Pass the direction and effectto the knockback coroutine
                StartCoroutine(Knockback(direction, hitEffect));

            }
            else if (collision.CompareTag("PlayerShield") || collision.CompareTag("PlayerRoll"))
            {
                // Deflect sound if enters trigger of player shield or roll
                if (am != null) am.Play("Deflect");

                // Pass direction, game object of collision to blocked coroutine
                StartCoroutine(Blocked(collision.gameObject, direction, blockEffect));
            }
        }

        // If the enemy contacts the collider of the player, behaviors are triggered and the player is damaged
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (onlyRoam)
            {
                moveDelta = -moveDelta;
            }
            // Ensures the enemies cannot damage the player if rolling or dead
            if (collision.gameObject.CompareTag("Player") && !collision.gameObject.GetComponent<PlayerMovement>().playerIsRolling 
                && collision.gameObject.GetComponent<PlayerMovement>().currentState != PlayerMovement.PlayerState.dead)
            {
                // Play sound and damage player
                if (am != null) am.Play("ShortHurtGrunt");
                GetComponent<EnemyStats>().DamagePlayer();

                // Set direction as before and pass variables to injured player coroutine
                Vector2 direction = (collision.gameObject.GetComponent<Transform>().position - transform.position).normalized;
                StartCoroutine(InjuredPlayer(direction, collision.gameObject));
            }
        }

        // Lets collider be disabled only if still stuck
        private void OnCollisionExit2D(Collision2D collision)
        {
            isStuck = false;
        }

        // If an enemy is touching a collider, call the unstick from wall coroutine
        private void OnCollisionStay2D(Collision2D collision)
        {
            isStuck = true;
            if (Vector2.Distance(player.GetComponent<Transform>().position, transform.position) < 5f)
                StartCoroutine(UnstickFromWall());
            
        }

        IEnumerator UnstickFromWall()
        {
            // After a 3 second delay, check if the player is within a set distance
            yield return new WaitForSeconds(3f);
            if (Vector2.Distance(player.GetComponent<Transform>().position, transform.position) < 5f && isStuck)
            {
                // If the player is still in range of the enemy after the time limit, for a brief time
                GetComponent<Collider2D>().enabled = false;
            }
            yield return new WaitForSeconds(.5f);
            GetComponent<Collider2D>().enabled = true;
        }
        // Controls what happens when an enemy is knocked back
        IEnumerator Knockback(Vector2 direction, GameObject effect)
        {
            if (am != null) am.Play(enemyHitSound);
            yield return new WaitForSeconds(.05f);

            // After a short delay, move the enemy away from the player, temporarily freeze its movement and instantiate parameterized effect object
            GetComponent<Transform>().Translate(direction * enemyKnockBack);
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            Instantiate(effect, GetComponent<Transform>().position, Quaternion.identity);

            // If the enemy's health reaches zero, play death sound and destroy game object after a delay (to allow animations to play out and item spawns)
            if (GetComponent<EnemyStats>().health <= 0 && am != null)
            {
                am.Play(enemyDeathSound);
                Destroy(gameObject, 3f);
            }
            yield return new WaitForSeconds(.5f);

            // Given the enemy is still alive, unfreeze enemy movements after a brief time being frozen
            if (GetComponent<EnemyStats>().health > 0)
            {
                GetComponent<Rigidbody2D>().constraints = ~RigidbodyConstraints2D.FreezeAll;
                GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }

        // Very similar to knockback IEnumerator, except the effects and effect behaviors are different
        IEnumerator Blocked(GameObject shield, Vector2 direction, GameObject effect)
        {
            GetComponent<Transform>().Translate(direction * enemyKnockBack);
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            Instantiate(effect, shield.GetComponent<Transform>().position + new Vector3(Random.Range(-.5f,.5f), Random.Range(-.5f,.5f)), Quaternion.identity);
            yield return new WaitForSeconds(.7f);
            if (GetComponent<EnemyStats>().health > 0)
            {
                GetComponent<Rigidbody2D>().constraints = ~RigidbodyConstraints2D.FreezeAll;
                GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }

        // Similar to the last two functions, Injured player handles how the player behaves on enemy contact
        IEnumerator InjuredPlayer(Vector2 direction, GameObject player)
        {
            // Both enemy and player are knocked back slightly
            GetComponent<Transform>().Translate(-direction * enemyKnockBack);
            player.GetComponent<Transform>().Translate(direction * playerKnockBack);
           
            // This function also implements stun functionality, but with different parameters
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            yield return new WaitForSeconds(2f);
            GetComponent<Rigidbody2D>().constraints = ~RigidbodyConstraints2D.FreezeAll;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }
}