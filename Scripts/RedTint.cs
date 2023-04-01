using UnityEngine;

// Only purpose of this script is to allow for the access and destruction of the red tint canvas object created when a player is hurt
public class RedTint : MonoBehaviour
{
   public void DestroyCanvas()
    {
        Destroy(transform.parent.gameObject);
    }
}
