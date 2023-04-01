using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnCave : MonoBehaviour
{
    GameObject player;

    // Define delay for player respawn and transform to reswpawn player
    public float waitTime = 1f;
    public Transform respawnPoint;

    // Hidden public bool needed to be accessed outside but not by the editor
    [HideInInspector] public bool isRespawning = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Respanw the player if boolean is set to true
    private void Update()
    {
        if (isRespawning)
        {
            ReSpawnPlayer();
        }
    }

    // Coroutine used to manage delay for effects and trying to get rid of movement and animation bugs
    public IEnumerator ReSpawnPlayer()
    {
        if(player == null)
        {
            Debug.Log("player not found by respawn point");
        }
        yield return new WaitForSeconds(waitTime);
        player.GetComponent<Animator>().ResetTrigger("isFalling");

        // Movement frozen to not interrupt animation
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;

        // Move the player to the respawn point
        player.transform.position = respawnPoint.position;
        isRespawning = false;
    }
    
}
