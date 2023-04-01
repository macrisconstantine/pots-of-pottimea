using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// This class was designed to provide a method of moving the character and camera 
/// to new areas without using excessive scenes. This script is added to
/// objects consisting of only a trigger collider. This class is essentially just 
/// one big on trigger enter function.
/// </summary>
public class CamTriggerController : MonoBehaviour
{
    AudioManager am;
    Camera mainCamera;

    // Variables to manage checking for enemies in room
    float timeLastChecked = 0;
    bool complete = false;
    int numOfEnemiesInRoom = 0;
    GameObject[] enemies;

    // Variable created to store a reference to the cinemachine camera confiner attachment
    GameObject camConfiner;

    // Takes a string to set the theme of the area being entered
    [SerializeField] string sceneTheme;

    // Stores values (unique for each trigger) to move the character and the camera on trigger enter
    [SerializeField] float xDistToMoveCam;
    [SerializeField] float yDistToMoveCam;
    [SerializeField] float xDistToMovePlayer = 0;
    [SerializeField] float yDistToMovePlayer = 3f;

    // Editor-accessible booleans allow for unique trigger behavior while using the same script
    [SerializeField] bool spawnsEnemies = false;
    [SerializeField] bool usesSpawnPoints = false;
    [SerializeField] bool spawnsSnakes = false;
    [SerializeField] bool spawnsRats = false;
    [SerializeField] bool spawnsBats = false;
    [SerializeField] bool spawnsPots = false;
    [SerializeField] bool changesTheme = true;
    [SerializeField] bool isHorizontal = false;
    [SerializeField] bool isDarkRoom = false;

    // These variables deal with enemy/object spawing behavior
    [SerializeField] int numOfEnemiesToSpawn = 5;
    [SerializeField] int spawnRangeX = 15;
    [SerializeField] int spawnRangeY = 5;
    [SerializeField] Vector2 roomCenter;
    [SerializeField] Transform[] spawnPoints;

    // The actual enemy/object game objects are added to an array for the script to access
    [SerializeField] GameObject[] enemiesToBeSpawned;

    // Allows for button reveal to open door
    [SerializeField] GameObject button;

    // These variables allow for a canvas with unique text to be activated on room enter
    [SerializeField] GameObject roomText;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] string roomName;

    // Allows for global light control
    [SerializeField] GameObject globalLight;

    void Start()
    {
        am = FindObjectOfType<AudioManager>();
        mainCamera = Camera.main;
        camConfiner = GameObject.Find("CamConfiner");
    }

    private void Update()
    {
        // If an enemy spawning room, check the number of enemies within that room every second
        if (Time.time > timeLastChecked + 1f && spawnsEnemies)
        {
            // Reset the count
            numOfEnemiesInRoom = 0;

            // Find enemies in map
            enemies = GameObject.FindGameObjectsWithTag("Enemy");

            // Count how many are within the area defined as the current room
            foreach (GameObject gameObject in enemies)
            {
                if (gameObject.transform.position.x < (roomCenter.x + 22) && gameObject.transform.position.x > (roomCenter.x + -20)
                    && gameObject.transform.position.y > (roomCenter.y + -9) && gameObject.transform.position.y < (roomCenter.y + 15))
                    numOfEnemiesInRoom++;
            }

            // If there is a button to reveal and the enemies are eliminated, reveal the button
            if (numOfEnemiesInRoom == 0 && button && !complete)
            {
                if (am != null)
                {
                    am.Stop(am.currTheme);
                    am.Play("SecretSound");
                    am.Play(am.currTheme);
                    button.SetActive(true);
                    complete = true;
                }
            }
            timeLastChecked = Time.time;
        }
    }

    // Transforms the main camera's confining parameters, transitioning the game view to new "scene".
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ensures collider for camera confinement is not null
        if (camConfiner == null)
        { Debug.LogError("No camera confiner located."); return; }

        // Checks to make sure collider belongs to an instance of the main player, not an npc or other object
        if (collision.gameObject.CompareTag("Player"))
        {
            if (am != null && changesTheme)
            {
                am.Stop(am.currTheme);
                am.currTheme = sceneTheme;
                am.Play(am.currTheme);
            }
            // Ensures camera moves in the right direction relative to its current position
            if (!isHorizontal)
            {
                // If the trigger is for vertical direction, and camera is above the position of the trigger, move the camera down a provided distance
                if (mainCamera.transform.position.y > GetComponent<Transform>().position.y)
                {
                    // Translate the camera confiner (and the camera comes with it) the parameterized distance
                    camConfiner.GetComponent<PolygonCollider2D>().transform.Translate(new Vector2(xDistToMoveCam, -yDistToMoveCam));

                    // Translate the player according to the respective values
                    collision.gameObject.transform.Translate(new Vector2(-xDistToMovePlayer, -yDistToMovePlayer));
                }
                else
                {
                    // If the camera is below the trigger, move the camera and player up
                    camConfiner.GetComponent<PolygonCollider2D>().transform.Translate(new Vector2(xDistToMoveCam, yDistToMoveCam));
                    collision.gameObject.transform.Translate(new Vector2(xDistToMovePlayer, yDistToMovePlayer));
                }
            }
            else
            {
                // The same code as above, only it checks the relative position of the camera on the x instead of the y axis
                if (mainCamera.transform.position.x > GetComponent<Transform>().position.x)
                {
                    camConfiner.GetComponent<PolygonCollider2D>().transform.Translate(new Vector2(-xDistToMoveCam, yDistToMoveCam));
                    collision.gameObject.transform.Translate(new Vector2(-xDistToMovePlayer, -yDistToMovePlayer));
                }
                else
                {
                    camConfiner.GetComponent<PolygonCollider2D>().transform.Translate(new Vector2(xDistToMoveCam, yDistToMoveCam));
                    collision.gameObject.transform.Translate(new Vector2(xDistToMovePlayer, yDistToMovePlayer));
                }
            }

            // Set the UI dialogue box text to name of the room
            if (dialogText != null)
                dialogText.text = roomName;

            // Set UI dialogue box active to display room name
            if (roomText != null)
            {
                if (!roomText.activeInHierarchy)
                {
                    StartCoroutine(DisplayRoomName());
                }
            }

            // Darkens light if in dark room and lightens if not
            if (isDarkRoom && globalLight != null)
            {
                globalLight.GetComponent<Animator>().SetBool("isDarkRoom", true);
            }
            else if (!isDarkRoom && globalLight != null)
            {
                globalLight.GetComponent<Animator>().SetBool("isDarkRoom", false);
            }

            // Entering an enemy-spawning room randomly distributes a set of enemies into the room
            if (spawnsEnemies)
            {
                if (numOfEnemiesInRoom < numOfEnemiesToSpawn)
                {
                    if (usesSpawnPoints) 
                    {
                        // If room uses spawn points, spawn select enemies at each spawn point
                        foreach (Transform transform in spawnPoints)
                        {
                            if (spawnsBats)
                                Instantiate(enemiesToBeSpawned[0], new Vector3(transform.position.x + Random.Range(-spawnRangeX, spawnRangeX), 
                                    transform.position.y + Random.Range(-spawnRangeY, spawnRangeY), 0), Quaternion.identity);
                            if (spawnsRats)
                                Instantiate(enemiesToBeSpawned[1], new Vector3(transform.position.x + Random.Range(-spawnRangeX, spawnRangeX), 
                                    transform.position.y + Random.Range(-spawnRangeY, spawnRangeY), 0), Quaternion.identity);
                            if (spawnsSnakes)
                                Instantiate(enemiesToBeSpawned[2], new Vector3(transform.position.x + Random.Range(-spawnRangeX, spawnRangeX), 
                                    transform.position.y + Random.Range(-spawnRangeY, spawnRangeY), 0), Quaternion.identity);
                        }
                    }
                    else // Randomly spread new enemy instantations accross the defined radius in the room
                    {
                        for (int i = 0; i < numOfEnemiesToSpawn - numOfEnemiesInRoom; i++)
                        {
                            if (spawnsBats)
                                Instantiate(enemiesToBeSpawned[0], new Vector3(roomCenter.x + Random.Range(-spawnRangeX, spawnRangeX), 
                                    roomCenter.y + Random.Range(-spawnRangeY, spawnRangeY), 0), Quaternion.identity);
                            if (spawnsRats)
                                Instantiate(enemiesToBeSpawned[1], new Vector3(roomCenter.x + Random.Range(-spawnRangeX, spawnRangeX), 
                                    roomCenter.y + Random.Range(-spawnRangeY, spawnRangeY), 0), Quaternion.identity);
                            if (spawnsSnakes)
                                Instantiate(enemiesToBeSpawned[2], new Vector3(roomCenter.x + Random.Range(-spawnRangeX, spawnRangeX), 
                                    roomCenter.y + Random.Range(-spawnRangeY, spawnRangeY), 0), Quaternion.identity);
                            if (spawnsPots)
                                Instantiate(enemiesToBeSpawned[3], new Vector3(roomCenter.x + Random.Range(-spawnRangeX, spawnRangeX), 
                                    roomCenter.y + Random.Range(-spawnRangeY, spawnRangeY), 0), Quaternion.identity);
                        }
                    }
                }
            }
        }
    }

    // Show the room name with a delay before deactivating the dialogue box
    IEnumerator DisplayRoomName()
    {
        roomText.SetActive(true);
        yield return new WaitForSeconds(3f);
        roomText.SetActive(false);
    }
}  


