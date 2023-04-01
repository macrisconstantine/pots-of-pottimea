using System.Collections;
using UnityEngine;

public class FallingOff : MonoBehaviour
{
    AudioManager am;
    GameObject player;
    public float waitTime = 1.5f;
    public Transform respawnPoint;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        am = FindObjectOfType<AudioManager>();
    }

    // If the player is in contact with a collider with this tag, trigger appropriate animations and teleport the player
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Shadow"))
        {
            if (player != null)
            {
                // This allows for one of the main components of the game. 
                // Rolling allows the player to cross the void
                if (!player.GetComponent<PlayerMovement>().playerIsRolling)
                {
                    if (am != null && player.GetComponent<PlayerMovement>().canMove == true)
                    {
                        am.Play("FallSound");
                    }
                    // Freeze movement, trigger appropriate animations, and teleport the player
                    player.GetComponent<PlayerMovement>().canMove = false;
                    player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                    player.GetComponent<Animator>().SetTrigger("isFalling");
                    StartCoroutine(Teleport(player));
                }
            }
        }
    }

  
    // IEnum used to add a delay before respawning the player
    IEnumerator Teleport(GameObject go)
    {
        
        yield return new WaitForSeconds(waitTime);
        go.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        go.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        go.GetComponent<Animator>().ResetTrigger("isFalling");
        go.transform.position = respawnPoint.gameObject.transform.position;
        if (am != null) am.Play("LongHurtGrunt");
        yield return new WaitForSeconds(waitTime);
        go.GetComponent<PlayerMovement>().canMove = true;
    }
}

