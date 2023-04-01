using System.Collections;
using UnityEngine;

// Script provides animation event function to randomly delay start of flicker
public class LightFlicker : MonoBehaviour
{
   // Calls delay coroutine
    void Start()
    {
        StartCoroutine(StartDelay());
    }

    // Adds random delay to make flicker more natural
    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(Random.Range(0f, 5f));
        GetComponent<Animator>().SetTrigger("Start");
    }

}
