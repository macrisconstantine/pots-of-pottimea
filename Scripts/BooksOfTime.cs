using UnityEngine;

public class BooksOfTime : MonoBehaviour
{
    GameObject gm;
    
    // Allows for pot upgrade after last book found
    [SerializeField] GameObject brownPots;

    // Showing in editor allows for each book to be given a unique id
    [SerializeField] int bookId;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Checks if player or player's weapon comes in contact with trigger area
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerWeapon"))
        {
            if (gm != null)
            {
                gm.GetComponent<UIManager>().collectBook(bookId);
            }

            // When the player wins the game, he gets a fun bonus speed boost, and pots drop only red rupees
            if (bookId == 2)
            {
                collision.gameObject.GetComponent<PlayerMovement>().UpgradeSpeed();
                brownPots.GetComponent<PotBreak>().probabilityForRedRupee = 100;
                brownPots.GetComponent<PotBreak>().probabilityForRupee = 0;
            }

            // Disables player movement input so that animation can play out
            collision.gameObject.GetComponent<PlayerMovement>().FreezePlayer();
            collision.gameObject.GetComponent<Animator>().SetTrigger("gotItem");

            // Attaches book to player and positions it over his head
            gameObject.transform.parent = collision.gameObject.transform;
            gameObject.transform.position = collision.gameObject.transform.position + new Vector3(0, 2f);

            GetComponent<Collider2D>().enabled = false;

            // Destroys after a delay
            Destroy(gameObject, 3f);
        }
    }
}