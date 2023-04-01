using UnityEngine;

public class HeartReplenish : MonoBehaviour
{
    GameObject gm;
    AudioManager am;

    // Used for effect of ruppee bouncing out of spawn
    public float bounceForce = 200f;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM");
        am = FindObjectOfType<AudioManager>();
    }

    // Heal player and play heal sound on collision with player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerWeapon"))
        {
            gm.GetComponent<PlayerHealth>().HealPlayer(4);
            am.Play("GetHeart");
            Destroy(gameObject);
        }
    }

    // Following functions are used by animation events
    void Bounce()
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, bounceForce), ForceMode2D.Impulse);
    }

    void Freeze()
    {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
    }

    void Despawn()
    {
        Destroy(gameObject);
    }
}
