using UnityEngine;

/// <summary>
/// This ultra simple script was made to provide a convenient method 
/// of assinging item-producing functionality to game objects.
/// </summary>
public class DeathBehavior : MonoBehaviour
{
    GameObject gm;

    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM");
    }
    public void SpawnItem()
    {
        if (gm != null)
        {
            // Spawns item at the location of this gameobject
            gm.GetComponent<ItemSpawner>().SpawnItem(transform);
        }
    }
}