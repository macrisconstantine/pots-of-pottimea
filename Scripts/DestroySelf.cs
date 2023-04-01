using UnityEngine;

// This script is made only to kill off the object after a short lifespan
public class DestroySelf : MonoBehaviour
{
    private void OnEnable()
    {
        Destroy(gameObject,3f);
    }
}
