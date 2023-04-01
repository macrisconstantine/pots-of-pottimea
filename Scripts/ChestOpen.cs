using System.Collections;
using UnityEngine;

/// <summary>
/// This script managing chest behavior was written before the ItemSpawner script, so some aspects of it may seem redundant,
/// but I left it as is to retain maximum customizability over what items a given chest produces.
/// </summary>
public class ChestOpen : MonoBehaviour
{
    // Variables for references to audiomanager and player
    AudioManager am;
    GameObject player;

    // Editor-accessible variables created so the chest script is given prefabs of the objects it needs to instantiate
    [SerializeField] GameObject sword;
    [SerializeField] GameObject shield;
    [SerializeField] GameObject heartContainer;
    [SerializeField] GameObject bookOfTime;
    [SerializeField] GameObject greenRupee;
    [SerializeField] GameObject blueRupee;
    [SerializeField] GameObject redRupee;

    // Editor-accessible means of specifying characteristics of each chest
    [SerializeField] int probabilityForItem = 75;
    [SerializeField] int probabilityForRupee = 50;
    [SerializeField] bool dropsHeartContainer = false;
    [SerializeField] bool dropsBooksOfTime = false;
    [SerializeField] bool dropsShield = false;
    [SerializeField] bool dropsSword = false;

    // Public bool used to tell PlayerMovement script what to display as text on the GUI action indicator
    [HideInInspector] public bool isOpen = false;

    // Used by update method to allow or prevent the player to open the chest
    bool inRange = false;

    // Assigns player and AudioManager
    private void Start()
    {
        am = FindObjectOfType<AudioManager>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Checks for player in range and input to activate the chest, reset action indicator text, and set the booleans appropriately
    private void Update()
    {
        if (inRange && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire3")) && player != null)
        {
            player.GetComponent<PlayerMovement>().actionIndicator.text = "Space";
            isOpen = true;
            inRange = false;
            ActivateChest();
        }
    }

    // Trigger enter checks for player and that the chest is not opened yet before telling the script that the player is in range
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player") && !isOpen)
        {
            inRange = true;
        }
    }
  
    // Sets in range to false on trigger exit.
    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            inRange = false;
        }
    }


    // Function used to disable the collider and activate the opening animation
    public void ActivateChest()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Animator>().SetTrigger("open");
    }

    // In the case of a single chest, instantiates approprate objects upon chest open
    public void SpawnRupeesChest()
    {
        /* This part of the if statement is functionally obselete since the single chest will currently never drop a heart container, 
         * but I left this code in so that if I ever want a chest to be able to drop heart containers, I don't need to re-write the code. */
        if (dropsHeartContainer)
        {
            Instantiate(heartContainer, GetComponent<Transform>().position + new Vector3(0, 1f, 0), Quaternion.identity);
            GetComponent<Transform>().DetachChildren();
        }
        // Spawns a book of time if respective boolean is checked
        else if (dropsBooksOfTime)
        {
            // Instantiates book object over the open chest with an offset 
            Instantiate(bookOfTime, GetComponent<Transform>().position + new Vector3(0, 1f, 0), Quaternion.identity);

            // Detaches the object from its parent so the destruction of one does not affect the other
            GetComponent<Transform>().DetachChildren();
        }
        else
        {
            // In the case of the single chest, drop five items (for now, only rupees)
            for (int i = 0; i < 3; i++)
            {
                SpawnItem();
            }
        }
        
    }

    // Same as above script, but with different rules for double chest
    public void SpawnRupeesDoubleChest()
    {
        if (dropsShield)
        {
            Instantiate(shield, GetComponent<Transform>().position + new Vector3(0, 1f, 0), Quaternion.identity);
            GetComponent<Transform>().DetachChildren();
        }
        else if (dropsSword)
        {
            Instantiate(sword, GetComponent<Transform>().position + new Vector3(0, 1f, 0), Quaternion.identity);
            GetComponent<Transform>().DetachChildren();
        }
        else
        {
            // In the case of the double chest, drop ten items (for now, only rupees)
            for (int i = 0; i < 5; i++)
                SpawnItem();
        }
    }

    // Function created to trigger chest creak sound as well as additional sound effects according to item drop
    public void ChestCreak()
    {
        if (am != null)
        {
            am.Play("ChestCreak");
            if (dropsShield || dropsSword)
            {
                StartCoroutine(MusicControl("GetNewItem"));
            }
            else if (dropsBooksOfTime || dropsHeartContainer)
            {
                StartCoroutine(MusicControl("GetHeartPiece"));
            }
        }
    }

    // Function used to spawn rupees based on various probabilities
    public void SpawnItem()
    {
        // Determines whether or not anywill will drop at all
        if (Random.Range(0, 100) < probabilityForItem)
        {
            // 10% of the time a rupee drops, it will be a blue rupee, with a value of 5
            if (Random.Range(0, 100) < probabilityForRupee / 5)
            {
                Instantiate(blueRupee, GetComponent<Transform>().position, Quaternion.identity);
                GetComponent<Transform>().DetachChildren();
            }

            // 90% * 5% of the time a rupee drops, it will be a red rupee, with a value of 20
            else if (Random.Range(0, 100f) < probabilityForRupee / 10)
            {
                Instantiate(redRupee, GetComponent<Transform>().position, Quaternion.identity);
                GetComponent<Transform>().DetachChildren();
            }

            // Otherwise default to dropping a green rupee with a value of 1
            else
            {
                Instantiate(greenRupee, GetComponent<Transform>().position, Quaternion.identity);
                GetComponent<Transform>().DetachChildren();
            }
        }
    }

    // IEnumerator used to pause the current theme while a given special sound effect plays
    IEnumerator MusicControl(string soundEffect)
    {
        if (am != null)
        {
            am.Stop(am.currTheme);
            am.Play(soundEffect);
            yield return new WaitForSeconds(3f);
            am.Play(am.currTheme);
        }
    }
 }
