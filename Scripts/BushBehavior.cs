using UnityEngine;

// Simple script used just to handle bush sounds and animations
public class BushBehavior : MonoBehaviour
{
    AudioManager am;
    void Start()
    {
        am = FindObjectOfType<AudioManager>();

        // Randomly inverts scale of some bush sprites for a more natural overall look
        if (Random.Range(0, 100) < 20 )
            GetComponent<SpriteRenderer>().flipX = true;
    }

    // Player can use sword to clear bushes
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerWeapon"))
        {
            // Trigger enter plays sounds, animation; and disables collider to prevent multiple collisions
            am.Play("BushSound");
            GetComponent<Animator>().SetTrigger("destroy");
            GetComponent<Collider2D>().enabled = false;

            // Destroys object after 3 seconds
            Destroy(gameObject, 3f);
        }
    }
}
