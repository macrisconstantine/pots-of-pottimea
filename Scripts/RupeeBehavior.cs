using UnityEngine;

public class RupeeBehavior : MonoBehaviour
{
    GameObject gm;
    AudioManager am;

    // Similar script to heart thrown script
    public float bounceForce = 200f;
    public int value = 1;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM");
        am = FindObjectOfType<AudioManager>();
    }

    // Increments rupee sum according to what rupee was collided with
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerWeapon"))
        {
            am.Play("GetRupee");
            gm.GetComponent<UIManager>().AddRupees(value);
            Destroy(gameObject);
        }
    }

    // Used for animation events
    void Bounce()
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-10f,10f), bounceForce), ForceMode2D.Impulse);
        Destroy(gameObject, 20f);
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