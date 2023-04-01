using System.Collections;
using UnityEngine;

// This script manages behavior that occurs when a pot is broken by the player
public class PotBreak : MonoBehaviour
{
    // Virtual camera is accessed for camera shake effect and audio manager for sounds
    GameObject vCam;
    AudioManager am;

    // Editor-assignable variables for objects to be instantiated upon pot breaking
    [SerializeField] GameObject greenRupee;
    [SerializeField] GameObject blueRupee;
    [SerializeField] GameObject redRupee;
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject heart;

    // Variable created to manage pot self-destruct time after break (in experimentation I played around with how long I wanted pot shards to remain)
    [SerializeField] float timeToDestroy = 3f;

    // Assign probability of item spawns from editor
    [SerializeField] int probabilityForItem = 75;
    [SerializeField] int probabilityForHeart = 50;
    
    public int probabilityForRupee = 50;
    public int probabilityForRedRupee = 3;

    // Variables created for pot collider and animator
    Animator anim;
    Collider2D pot;

    // Array created to randomly select a sound to play when a pot breaks
    string[] potSmashSounds = new string[] {"PotSmash2", "PotSmash2"};

    // Variables are located and sprite is flipped 20% of the time just for a more random looking effect in game
    void Start()
    {
        vCam = GameObject.Find("VCam");
        anim = GetComponent<Animator>();
        pot = GetComponent<Collider2D>();
        am = FindObjectOfType<AudioManager>();
        if (Random.Range(0, 100) < 20)
            GetComponent<SpriteRenderer>().flipX = true;
    }

    // If the player rolls into a pot or player weapon contacts the pot, call the break pot function
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerWeapon") || collision.CompareTag("PlayerRoll")) 
            BreakPot();
    }
    
    /* Trigger a slight camera shake, disable collider, trigger pot breaking animation,
     * play break sound, and destroy game object shortler after collision */
    void BreakPot()
    {
        if (vCam != null) vCam.GetComponent<CameraController>().ShakeCamera(.5f, .5f);
        pot.enabled = false;
        anim.SetTrigger("isHit");
        am.Play(potSmashSounds[Random.Range(0, 2)]);
        Destroy(gameObject, timeToDestroy);
    }

    /* This function is accessed via an animation event, and it manages which items will spawn according to set probabilities.
     * It is very similar to the function in the ItemSpawner class, where it is explained more thoroughly */
    void SpawnItem()
    {
        if (Random.Range(0, 100) < probabilityForItem)
        {
            if (Random.Range(0, 100) < probabilityForHeart)
            {
                Instantiate(heart, GetComponent<Transform>().position, Quaternion.identity);
                GetComponent<Transform>().DetachChildren();
            } 
            else if (Random.Range(0, 100) < probabilityForRupee/5)
            {
                Instantiate(blueRupee, GetComponent<Transform>().position, Quaternion.identity);
                GetComponent<Transform>().DetachChildren();
            }
            else if (Random.Range(0, 100f) < probabilityForRedRupee)
            {
                Instantiate(redRupee, GetComponent<Transform>().position, Quaternion.identity);
                GetComponent<Transform>().DetachChildren();
            }
            else if (Random.Range(0, 100) < probabilityForHeart / 50)
            {
                Instantiate(enemy, GetComponent<Transform>().position, Quaternion.identity);
                GetComponent<Transform>().DetachChildren();
            }
            else
            {
                Instantiate(greenRupee, GetComponent<Transform>().position, Quaternion.identity);
                GetComponent<Transform>().DetachChildren();
            }
        }
    }
    
}
