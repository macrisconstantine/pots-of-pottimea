using UnityEngine;

public class MoveableBehavior : MonoBehaviour
{
    AudioManager am;
    [SerializeField] GameObject vcam;
    [SerializeField] [Range(0,3f)] float shakeAmplitude = 1f;
    [SerializeField] [Range(0, 3f)] float shakeFrequency = 1f;


    private void Start()
    {
        am = FindObjectOfType<AudioManager>();
    }

    // Shakes camera while object is being moved
    void Update()
    {
        if (GetComponent<Rigidbody2D>().velocity != new Vector2(0, 0) && vcam != null)
        {
            vcam.GetComponent<CameraController>().ShakeCamera(shakeAmplitude, shakeFrequency);
        }     
    }

    // Plays object moving sound when player is touching an object with this script
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && am != null)
        {
            am.Play("MovingObject");
        }
    }
    
    // Stops object moving sound when player is touching an object with this script
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && am != null)
        {
            am.Stop("MovingObject");
        }
    }
}
