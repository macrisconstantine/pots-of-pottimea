using UnityEngine;

// Simple script merely triggers a splash animation for water splash objects
public class SplashBehavior : MonoBehaviour
{
    AudioManager am;

    private void Start()
    {
        am = FindObjectOfType<AudioManager>();
    }

    // If player steps on object, trigger sound and animation
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Shadow"))
        {
            if (am != null) am.Play("Splash");
            GetComponent<Animator>().SetTrigger("splash");
        }    
    }
}
