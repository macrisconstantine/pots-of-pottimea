using UnityEngine;

public class HeartContainer : MonoBehaviour
{
    GameObject gm;
    AudioManager am;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM");
        am = FindObjectOfType<AudioManager>();    
    }

    // Heart containers increase the players max health by 4, or a full heart sprite in the GUI
    public void GiveToPlayer(GameObject player)
    {
        if (am != null) am.Play("GetHeartPiece");

        // Freeze movement and play animation, placing the container above the player's head
        player.gameObject.GetComponent<PlayerMovement>().FreezePlayer();
        player.gameObject.GetComponent<Animator>().SetTrigger("gotItem");
        gameObject.transform.parent = player.gameObject.transform;
        gameObject.transform.position = player.gameObject.transform.position + new Vector3(0, 2f);

        // Inrement the player's heart containers and destroy the game object after a delay
        gm.GetComponent<PlayerHealth>().IncreaseHeartContainers();
        Destroy(gameObject, 3f);
    }
 }
