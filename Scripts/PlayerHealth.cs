using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// The basic structure of this code was written following a tutorial at: https://www.youtube.com/watch?v=3uyolYVsiWc
/// I struggled extensively but eventually came up with a way to display hearts with 4 segments of health.
/// </summary>


public class PlayerHealth : MonoBehaviour
{
    int maxHealth;
    int heartContainerIndex = 0;
    int currContainerCost = 100;
    public int health;
    GameObject vCam;
    GameObject player;
    AudioManager am;

    [SerializeField] TextMeshPro currRupeeCost;
    [SerializeField] int numOfHearts = 3;

    // Sprites to be used to represent health levels
    [SerializeField] Image[] hearts;
    [SerializeField] GameObject[] heartContainers;
    [SerializeField] Sprite fullHeart;
    [SerializeField] Sprite threeQuartHeart;
    [SerializeField] Sprite halfHeart;
    [SerializeField] Sprite quarterHeart;
    [SerializeField] Sprite emptyHeart;

    // Serialized fields for managing movement of player and camera according to health changes
    [SerializeField] GameObject camPoint;
    [SerializeField] GameObject respawnPoint;
    [SerializeField] GameObject camConfiner;
    [SerializeField] GameObject gameOver;
    [SerializeField] GameObject hurtPlayerTint;


    // Initializes max health as four units per heart and sets health to health at awake
    private void Start()
    {
        am = FindObjectOfType<AudioManager>();
        player = GameObject.FindGameObjectWithTag("Player");
        maxHealth = numOfHearts * 4;
        health = maxHealth;
        vCam = GameObject.Find("VCam");
        ResetTheme();
    }

    // This code is a bit sloppy perhaps, but it was the only working solution I arrived at on my own.
    // All attempts for simpler code ended up with unexpected bugs
    private void Update()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (health == 0)
            {
                hearts[i].sprite = emptyHeart;
            }

            else if (i < health / 4)
            {
                hearts[i].sprite = fullHeart;
            }
            // If the the number is not divisible by four, select an appropriate sprite to display either quarter, half, or three-quarter's health
            else if (i == health / 4)
            {
                if (health % 4 == 3)
                {
                    hearts[i].sprite = threeQuartHeart;
                }
                else if (health % 4 == 2)
                {
                    hearts[i].sprite = halfHeart;
                }
                else if (health % 4 == 1)
                {
                    hearts[i].sprite = quarterHeart;
                }
                else if (health % 4 == 0)
                {
                    if (i < health / 4)
                        hearts[i].sprite = fullHeart;
                    // Otherwise put an empty heart sprite
                    else
                        hearts[i].sprite = emptyHeart;
                }
            }
            else if (i > health / 4)
            {
                //Debug.Log(health / 4);
                hearts[i].sprite = emptyHeart;
            }

            // Display the appropriate number of hearts in the GUI respective of how many hearts the player has unlocked in game
            if (i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }

       /* for (int i = 0; i < hearts.Length; i++)
        {
            if (i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }*/
    }

    public void DamagePlayer(int damage)
    {
        if (hurtPlayerTint != null) Instantiate(hurtPlayerTint);
        if (vCam != null)
        {
            vCam.GetComponent<CameraController>().ShakeCamera(2f,1f);
        }

        health -= damage;
        if (health <= 0)
        {
            health = 0;
            if (player != null)
            {
                StartCoroutine(GameOverScreen());
                player.GetComponent<PlayerMovement>().enabled = false;
                player.GetComponent<Animator>().SetBool("death", true);
                player.GetComponent<PlayerMovement>().currentState = PlayerMovement.PlayerState.dead;
                player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
    }

    public void UpgradeMaxHealth()
    {
        if (GetComponent<UIManager>().rupeeCount >= currContainerCost)
        {
            GetComponent<UIManager>().rupeeCount -= currContainerCost;
            currContainerCost += 50;
            if (heartContainers.Length > 0 && heartContainerIndex <= 12)
            {
                if (heartContainers[heartContainerIndex] != null)
                {
                    heartContainers[heartContainerIndex].GetComponent<HeartContainer>().GiveToPlayer(player);
                    heartContainerIndex++;
                }
            }
            if (currContainerCost > 700)
                currContainerCost = 700;
            if (currRupeeCost != null)
            {
                currRupeeCost.text = currContainerCost.ToString();
            }
        }
        else if (am != null)
        {
            am.Play("Deflect");
        }
    }
    public void HealPlayer(int heal)
    {
        StartCoroutine(HealUp(heal));
    }

    public void RespawnPlayer()
    {
        if (player != null)
        {
            ResetTheme();
            camConfiner.transform.position = camPoint.gameObject.transform.position;
            player.transform.position = respawnPoint.gameObject.transform.position;
            player.GetComponent<Rigidbody2D>().constraints = ~RigidbodyConstraints2D.FreezePosition;
            player.GetComponent<Animator>().SetBool("death", false);
            //player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            player.GetComponent<Collider2D>().enabled = true;
            player.GetComponent<PlayerMovement>().enabled = true;
            player.GetComponent<PlayerMovement>().canMove = true;
            player.GetComponent<Animator>().Play("southIdle");
            HealPlayer(maxHealth);
        }
    }

    public void ResetTheme()
    {
        if (am != null)
        {
            am.Stop(am.currTheme);
            am.currTheme = "InsideTheme";
            am.Play(am.currTheme);
        }
    }

    public void IncreaseHeartContainers()
    {
        numOfHearts++;
        if (numOfHearts > 16) numOfHearts = 16;
        maxHealth = numOfHearts * 4;
        health = maxHealth;
    }

    IEnumerator GameOverScreen()
    {
        if (am != null)
        {
            am.Stop(am.currTheme);
            am.currTheme = "GameOverTheme";
            am.Play(am.currTheme);
        }
        yield return new WaitForSeconds(2f);
        gameOver.SetActive(true);
    }

      IEnumerator HealUp(int heal)
    {
        for (int i = 0; i < heal; i++)
        {
            health++;
            if (health >= maxHealth)
            {
                health = maxHealth;
            }
            yield return new WaitForSeconds(.25f);
        }
        
    }
}
