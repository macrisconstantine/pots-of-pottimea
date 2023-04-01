using UnityEngine;
using System.Collections;

// This script is used only to active the event triggered by the button
public class Button : MonoBehaviour
{
    AudioManager am; 

    // Camera and unique platform/barrier are assigned from editor
    [SerializeField] GameObject vcam;
    [SerializeField] GameObject platform;
    [SerializeField] GameObject barrier;

    // Tell the script what type of object to manipulate
    [SerializeField] bool isBarrier = false;

    // Accesses Audiomanager
    private void Start()
    {
        am = FindObjectOfType<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Activates on collision with player's shadow, so he must be directly over the button to activate it
        if (collision.CompareTag("Shadow"))
        {   
            if (platform != null && !isBarrier)
            {
                // Activates the animator, disables collider to prevent multiple activations, and calls effect coroutine
                GetComponent<Animator>().SetTrigger("press");
                GetComponent<Collider2D>().enabled = false;
                StartCoroutine(PlatformPushEffects());
            }
            else if (barrier != null)
            {
                // Activates the animator, disables collider to prevent multiple activations, and calls effect coroutine
                GetComponent<Animator>().SetTrigger("press");
                GetComponent<Collider2D>().enabled = false;
                StartCoroutine(BarrierOpenEffects());

              
            }
        }
    }

    // Effect coroutine is necessary to create effects with delays
    IEnumerator PlatformPushEffects()
    {
        if (am != null && vcam != null)
        {
            // Pauses theme music, plays secret reveal sound effect, and resumes theme after a short delay
            am.Stop(am.currTheme);
            am.Play("SecretSound");
            
            // Prevents secret sound from conflicting with theme music
            yield return new WaitForSeconds(1f);
            am.Play(am.currTheme);

            // Triggers camera shake, plays sound, and triggers moving platform animation with delays
            vcam.GetComponent<CameraController>().ShakeCamera(.5f, 1f);
            yield return new WaitForSeconds(1.5f);
            am.Play("MovingObject");
            vcam.GetComponent<CameraController>().ShakeCamera(2f, 1f);
            platform.GetComponent<Animator>().SetTrigger("reveal");
            yield return new WaitForSeconds(2.5f);
            vcam.GetComponent<CameraController>().ShakeCamera(2f, 1f);
            am.Stop("MovingObject");
        }
    }

    IEnumerator BarrierOpenEffects()
    {
        if (am != null)
        {
            // Pauses theme music, plays secret reveal sound effect, and resumes theme after a short delay
            am.Stop(am.currTheme);
            am.Play("SecretSound");

            if (vcam != null)
                // Triggers camera shake, plays sound, and triggers moving platform animation with delays
                vcam.GetComponent<CameraController>().ShakeCamera(.5f, 1f);
            
            // Prevents unique sounds from conflicting with theme music
            yield return new WaitForSeconds(.5f);
            am.Play("Cut");
            yield return new WaitForSeconds(.5f);

            // Disable barrier collider and trigger animation
            barrier.GetComponent<Collider2D>().enabled = false;
            barrier.GetComponent<Animator>().SetBool("open", true);

            am.Play(am.currTheme);
        }
    }
}
