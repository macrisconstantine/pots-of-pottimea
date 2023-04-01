using UnityEngine;

public class UnlockItem : MonoBehaviour
{
    [SerializeField] bool sword;
    [SerializeField] bool item;

    // This class consists of a single oncollision function to enable use of sword and shield when they are collected
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<PlayerMovement>().FreezePlayer();
            if (sword)
            {
                Destroy(gameObject);

                // Play the animation for getting the sword and set the sword unlocked boolean true in player movement, which allows input to activate the attack animation
                collision.gameObject.GetComponent<Animator>().SetTrigger("gotSword");
                collision.gameObject.GetComponent<PlayerMovement>().swordUnlocked = true;
            } else if (item)
            {
                Destroy(gameObject,3f);

                // Position the item above the player's head and trigger animation, repeating code functionality from the sword unlock
                gameObject.transform.parent = collision.gameObject.transform;
                gameObject.transform.position = collision.gameObject.transform.position + new Vector3(0, 1.75f);
                collision.gameObject.GetComponent<Animator>().SetTrigger("gotItem");
                collision.gameObject.GetComponent<PlayerMovement>().shieldUnlocked = true;
            }
        }
    }
}
