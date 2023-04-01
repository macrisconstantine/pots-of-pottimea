using UnityEngine;

// This class manages simple movement and animation for bunny object
public class BunnyMove : MonoBehaviour
{
    // Allows changes to speed from the editor
    [SerializeField] float speed = 1f;
    Vector2 moveDelta;
    float x, y;
    Vector2 dir;

    // Each time the script is enabled, new random x and y vales are assigned
    private void OnEnable()
    {
        x = Random.Range(-1f, 1f);
        y = Random.Range(-1f, 1f);
        dir = new Vector2(x, y);

        // Sprite is oriented according to direction
        if (x < 0)
            transform.localScale = new Vector2(-1, 1);
        else transform.localScale = Vector2.one;
    }

    // While script is enabled, bunny game object is tranlsated according to the freshly set direction
    private void Update()
    {
        moveDelta = dir * speed * Time.deltaTime;
        GetComponent<Transform>().Translate(moveDelta);
    }

    // Change direction of bunny if it runs into an obstacle
    private void OnCollisionEnter2D(Collision2D collision)
    {
        dir = -dir;
        transform.localScale = new Vector2(-1, 1);
    }

    // Function to enable script (which activates movement) is used by an animation event
    void startMoveBunny()
    {
        enabled = true;
    }
    
    // Function to disable script and freeze movement, also used by animation event
    void stopMoveBunny()
    {
        enabled = false;
    }
    
}
