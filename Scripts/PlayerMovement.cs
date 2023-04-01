using System.Collections;
using System;
using UnityEngine;
using TMPro;

/// <summary>
/// This is one of the biggest classes in the game. It manages and coordinates input,
/// player movement, animation, player states, and interactions with other objects.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    // Declaration of variables to reference key objects and components
    GameObject gm;
    AudioManager am;
    Animator animator;

    // Editor-accessible means of manipulating player movement attributes
    [SerializeField] [Range(1f, 20f)] float speed = 5f;
    [SerializeField] [Range(1f, 3f)] float rollSpeedIncreaseFactor = 1.5f;
    [SerializeField] [Range(.2f, 7f)] float rollDuration = 0.5f;
    [SerializeField] [Range(0, 2f)] float rollCooldown = 0.1f; 
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject shieldUI;
    [SerializeField] GameObject swordUI;
    [SerializeField] GameObject effect;

    // Character emote manipulation
    [SerializeField] GameObject thought;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject question;

    // Player state and input restrictions must be accessible to other classes but not the editor
    [HideInInspector] public bool canInput = true;
    [HideInInspector] public PlayerState currentState;
    
    // Action indicator must be public so that it can be accessed by the editor and other scripts
    public TextMeshProUGUI actionIndicator;

    // Vectors declared to define directions for movement and sprite inversion
    private Vector3 invertSpriteVector = new Vector3(-1f, 1f, 1f);
    private Vector3 moveDelta;

    // Various booleans used to manage what actions/movement can be executed
    private bool isRolling = false;
    private bool canRoll = true;
    private bool playerCanMove = true;
    private bool playerCanAttack = true;
    private bool playerCanPush = false;
    private bool onPurchasePlatform = false;

    // These booleans were made public just for development and testing convenience
    public bool swordUnlocked = false;
    public bool shieldUnlocked = false;
   
    // Variables to keep track of input directional values
    private float dx;
    private float dy;

    // Directions named just for ease of translation from code to animator parameter values
    private int north = 0;
    private int south = 1;
    private int west = 2;

    // Player state enum used to limit available actions/movements
    public enum PlayerState
    {
        move, fight, interact, dead
    }

    // Getters and setters for private booleans that need external access
    public bool playerIsRolling
    {
        get {return isRolling;}
    }
    public bool canAttack
    {
        get { return playerCanAttack; }
        set { playerCanAttack = value; }
    }
    public bool canMove
    {
        get { return playerCanMove; }
        set { playerCanMove = value; }
    }
    public bool canPush
    {
        get { return playerCanPush; }
        set { playerCanPush = value; }
    }

    // Direction variable just for keeping track of the animator state within the script
    private string direction = "S";

    // Initializes variables for referenced objects and current state of the player
    private void Start()
    {
        animator = GetComponent<Animator>();
        am = FindObjectOfType<AudioManager>();
        gm = GameObject.FindGameObjectWithTag("GM");
        currentState = PlayerState.move;
    }

    // Update method used to takes input for moving, actions, and GUI events
    void Update()
    {
       /* foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(kcode))
                Debug.Log("KeyCode down: " + kcode);
        }*/
        // Assuming canInput is true, takes directional input from WASD or arrow keys
        if (canInput)
        {
            dx = Input.GetAxisRaw("Horizontal");
            dy = Input.GetAxisRaw("Vertical");
        }
        // Else clause is needed here to ensure player movement stops when canInput is false
        else dx = dy = 0;
        
        // Toggles the active status of the pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Changes the pause menu based on what items are unlocked
            if (!pauseScreen.activeInHierarchy)
            {
                if (shieldUnlocked)
                {
                    shieldUI.SetActive(true);
                }
                if (swordUnlocked)
                {
                    swordUI.SetActive(true);
                }

                // Accesses the action indicator button of the action indicator text and disables it
                actionIndicator.transform.parent.gameObject.SetActive(false);
              
                // Activates pause screen
                pauseScreen.SetActive(true);
            } else
            {
                // Accesses the action indicator button of the action indicator text and enables it
                actionIndicator.transform.parent.gameObject.SetActive(true);
                
                // De-Activates pause screen
                pauseScreen.SetActive(false);
            }
        }

        // If the player is moving, can roll and space is pressed, activates roll coroutine
        if ((dx != 0 || dy != 0) && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire3")) && canRoll == true)
        {
            StartCoroutine(Roll(dx, dy));
        }

        // Attempts to purchase heart container if on the purchase platform
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire3")) && onPurchasePlatform && gm != null)
        {
            // Accesses function from the gamemaster object's player health script
            gm.GetComponent<PlayerHealth>().UpgradeMaxHealth();
        }
        // Starts attack coroutine if swordUnlocked and input 
        if (swordUnlocked && (Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Fire1")) && currentState != PlayerState.fight) 
        {
            StartCoroutine(Attack());
        }
        // Sets bool for shield true in the animator on key down
        else if (shieldUnlocked && (Input.GetKeyDown(KeyCode.K) || Input.GetButtonDown("Fire2")))
        {
            animator.SetBool("shielding", true);
        }
        // Sets bool for shield false in the animator on key up
        else if (Input.GetKeyUp(KeyCode.K) || Input.GetButtonUp("Fire2"))
        {
            animator.SetBool("shielding", false);
        }
        // Allows player movement when not attacking
        else if (currentState != PlayerState.fight)
        {
            MoveAnimatedPlayer();
        }
    }

    /* While player is in contact with trigger areas of various interactable objects,
     * change the player state, and set the appropriate text for the action indicator.
    */ 
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Ensures chest can only affect player if it has not been opened yet
        if (collision.CompareTag("Chest") && !collision.GetComponent<ChestOpen>().isOpen)
        {
            currentState = PlayerMovement.PlayerState.interact;
            actionIndicator.text = "Open";
            if (exclamation != null)
                exclamation.SetActive(true);
        }
        else if (collision.CompareTag("Text") && !collision.GetComponent<SignScript>().isOpen)
        {
            currentState = PlayerMovement.PlayerState.interact;
            actionIndicator.text = "Read";
            if (question != null)
                question.SetActive(true);
        }
        else if (collision.CompareTag("StoreKeeper") && !collision.GetComponent<SignScript>().isOpen)
        {
            currentState = PlayerMovement.PlayerState.interact;
            actionIndicator.text = "Speak";
            if (thought != null)
                thought.SetActive(true);
        }
        else if (collision.CompareTag("PurchasePlatform"))
        {
            // Special case clause created just for the purchase platform mechanic
            onPurchasePlatform = true;
            currentState = PlayerMovement.PlayerState.interact;
            if (thought != null)
                thought.SetActive(true);
            actionIndicator.text = "Buy";
        }
    }

    // When interactable object triggers are exited, reset action indicator and player state
    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.CompareTag("Text") || collision.CompareTag("Chest") || collision.CompareTag("StoreKeeper"))
        {
            exclamation.SetActive(false);
            question.SetActive(false);
            thought.SetActive(false);
            currentState = PlayerMovement.PlayerState.move;
            actionIndicator.text = "Space";
        }
        else if (collision.CompareTag("PurchasePlatform"))
        {
            thought.SetActive(false);
            onPurchasePlatform = false;
            currentState = PlayerMovement.PlayerState.move;
            actionIndicator.text = "Space";
        }
    }

    // This function handles movement and animator states
    void MoveAnimatedPlayer()
    {
        // If positive y value input, set animator direction to north
        if (dx == 0 && dy > 0)
        {
            animator.SetFloat("speed", 1);
            animator.SetInteger("direction", north);
            direction = "N";
        }
        // If negative y value input, set animator direction to south
        else if (dx == 0 && dy < 0)
        {
            animator.SetFloat("speed", 1);
            animator.SetInteger("direction", south);
            direction = "S";
        }
        // If non-negative x input value, set animator direction to west
        else if (dx != 0 && dy == 0)
        {
            animator.SetFloat("speed", 1);
            animator.SetInteger("direction", west);
            direction = "W";
        }
        else if (dx != 0 && dy > 0)
        {
            animator.SetFloat("speed", 1);
            animator.SetInteger("direction", north);
            direction = "N";
        }
        else if (dx != 0 && dy < 0)
        {
            animator.SetFloat("speed", 1);
            animator.SetInteger("direction", south);
            direction = "S";
        }
        // If x and y values are zero, set animator speed to zero and maintain the last direction
        else if (dx == 0 && dy == 0)
        {
            animator.SetFloat("speed", 0);
            if (direction == "N") animator.SetInteger("direction", north);
            else if (direction == "S") animator.SetInteger("direction", south);
            else if (direction == "W") animator.SetInteger("direction", west);
            else animator.SetInteger("direction", south);
        }
        // This section of code fixed a bug in which two simultaneous direction inputs caused
        // the animator to remain in idle state even with transform movement
        if (Input.GetKeyDown(KeyCode.W) && (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A))
                || (Input.GetKeyDown(KeyCode.UpArrow) && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))))
        {
            animator.SetFloat("speed", 1);
            animator.SetInteger("direction", north);
            direction = "N";
        }
        if (Input.GetKeyDown(KeyCode.S) && (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A))
            || (Input.GetKeyDown(KeyCode.DownArrow) && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))))
        {
            animator.SetFloat("speed", 1);
            animator.SetInteger("direction", south);
            direction = "S";
        }

        // change direction of sprite according to direction
        if (dx < 0)
        {
            transform.localScale = Vector3.one;
            if (thought != null)
            {
                thought.GetComponent<SpriteRenderer>().flipX = false;
                question.GetComponent<SpriteRenderer>().flipX = false;
            }
        }
        else if (dx > 0)
        {
            transform.localScale = invertSpriteVector;
            if (thought != null)
            {
                thought.GetComponent<SpriteRenderer>().flipX = true;
                question.GetComponent<SpriteRenderer>().flipX = true;
            }

        }

        // Apply inputed values as a vector (multiplied by the speed factor) to move the character
        if (canMove)
        {
            moveDelta = new Vector3(dx, dy, 0).normalized;
            GetComponent<Rigidbody2D>().velocity = moveDelta * speed;
        }
        // If no input, stop footstep sounds
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && Input.GetAxisRaw("Horizontal") == 0f && Input.GetAxisRaw("Vertical") == 0)
        {
            if (am != null)
                am.Stop("Steps");
        }
        // Show in the UI that the player can roll while he is moving
        if (currentState != PlayerState.interact && actionIndicator)
        {
            if (dx != 0 || dy != 0)
            {
               actionIndicator.text = "Roll";
            }
            else
            {
                actionIndicator.text = "Space";
            }
        }
        // Fixes bug where shield persists after input stops
        if (!Input.GetKey(KeyCode.K) && !Input.GetButton("Fire2"))
        {
            animator.SetBool("shielding", false);
        }
    }

    // When game objective is complete, this function increases the player's movement speed
    public void UpgradeSpeed()
    {
        speed = speed * 1.5f;
    }

    // IEnumerator function used to handle attack behavior  
    IEnumerator Attack()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0f,0f);
        // Trigger attack animation and set player state to attack for a short time
        currentState = PlayerState.fight;
        canInput = false;
        animator.SetFloat("speed", 0);
        animator.SetBool("attacking", true);
        yield return null;
        animator.SetBool("attacking", false);
        yield return new WaitForSeconds(.1f);
        canInput = true;
        currentState = PlayerState.move;
    }

    // IEnumerator used to control roll behavior
    IEnumerator Roll(float x, float y)
    {
        if (GetComponent<TrailRenderer>() != null)
            GetComponent<TrailRenderer>().enabled = true;
        // Pauses footstep sounds for duration of roll
        if (am != null)
        {
            am.Play("AttackGrunt2");
            am.Stop("Steps");
        }

        // Sets booleans to tell other objects how to interact with player during roll
        canRoll = false;
        isRolling = true;

        // Increases speed by a given increase factor and triggers animation
        speed *= rollSpeedIncreaseFactor;
        animator.SetBool("isRolling", true);

        // Waits given amount of time to complete roll
        yield return new WaitForSeconds(rollDuration);

        // Resets speed to original speed and stops animation
        speed /= rollSpeedIncreaseFactor;
        animator.SetBool("isRolling", false);
        isRolling = false;

        // Implements a short roll cooldown 
        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
        yield return new WaitForSeconds(.2f);
        if (GetComponent<TrailRenderer>() != null)
            GetComponent<TrailRenderer>().enabled = false;

    }

    // Public function to call Freeze coroutine
    public void FreezePlayer()
    {
        StartCoroutine(Freeze());
    }

    // Used to pause movement and spawn effect animation for when character obtains a new item
    IEnumerator Freeze()
    {
        // Pauses directional input
        GetComponent<PlayerMovement>().canInput = false;

        // For loop spawns 3 effect prefabs (with a short delay between each) in a random location within an offset range of the player
        for (int i = 0; i < 3; i++)
        {
            Instantiate(effect, GetComponent<Transform>().position + new Vector3(UnityEngine.Random.Range(-.5f, .5f), 2f + UnityEngine.Random.Range(-.5f, .5f)), Quaternion.identity);
            yield return new WaitForSeconds(.2f);
        }
        yield return new WaitForSeconds(3f);
        
        // Unpauses directional input
        GetComponent<PlayerMovement>().canInput = true;
    }

    // Used by animation event to injure player if player came in contact with falling off collider
    public void FallDamage()
    {
        if (gm != null) gm.GetComponent<PlayerHealth>().DamagePlayer(4);
    }

    // Following 3 scripts are for animation events to play step sounds while player is walking and attack sound mid-swing
    void StartStepSound()
    {
        if (am != null)
        {
            am.Play("Steps");
        }
    }

    void StopStepSound()
    {
        if (am != null)
        {
            am.Stop("Steps");
        }
    }
    void AttackSound()
    {
        if (am != null)
        {
            am.Play("SwordSwipe");
            am.Play("AttackGrunt");
        }
    }

    // Following two scripts are for use by animation events to disable player movement script
    void PauseMovement()
    {
        enabled = false;
    }
    void UnpauseMovement()
    {
        if (gm.GetComponent<PlayerHealth>().health > 0)
        {
            enabled = true;
        }
    }
}
