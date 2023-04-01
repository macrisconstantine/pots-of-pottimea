using UnityEngine;


// This code ultimately was not used in the game, but it was left intact for future use
public class CrateBehavior : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Animator>().SetBool("isPushing", true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Animator>().SetBool("isPushing", false);
        }
    }
}
