using UnityEngine;

namespace Cinemachine
{
    // This script was supposed to be a method of finding the player to track
    public class FindPlayer : MonoBehaviour
    {
        GameObject player;
        Transform camTransform; 

        void Start()
        {
            camTransform = GetComponent<CinemachineVirtualCamera>().Follow;
        }

        // I could not get this to work properly, so I believe I ended up working around it
        void Update()
        {
            if (!camTransform)
            {
                Debug.Log("player vanished.");
                player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Debug.Log("found gameobject.");
                    camTransform = player.GetComponent<Transform>();
                }
            }
        }
    }
}