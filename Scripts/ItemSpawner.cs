using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    // GameObjects to be spawned and respective probabilities are assigned in the editor
    [SerializeField] GameObject greenRupee;
    [SerializeField] GameObject blueRupee;
    [SerializeField] GameObject redRupee;
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject heart;
    [SerializeField] int probabilityForItem = 75;
    [SerializeField] int probabilityForHeart = 50;
    [SerializeField] int probabilityForRupee = 50;

    // Items spawn when function is called
    public void SpawnItem(Transform parent)
    {
        // Determines if an item will be spawned at all according to a set probability percentage
        if (Random.Range(0, 100) < probabilityForItem)
        {
            if (Random.Range(0, 100) < probabilityForHeart)
            {
                // Determines heart's spawn probability and instantiates heart game object
                Instantiate(heart, parent.position, Quaternion.identity);
                parent.DetachChildren();
            }
            else if (Random.Range(0, 100) < probabilityForRupee / 3)
            {
                // Determines rupees spawn probability and instantiates blue rupee game object
                Instantiate(blueRupee, parent.position, Quaternion.identity);
                parent.DetachChildren();
            }
            else if (Random.Range(0, 100f) < probabilityForRupee / 10)
            {
                // Rarely spawns a red rupee worth 20 rupees
                Instantiate(redRupee, parent.position, Quaternion.identity);
                parent.DetachChildren();
            }
            else if (Random.Range(0, 100) < probabilityForHeart / 15)
            {
                // 1 out of a hundred pots will produce an enemy instead of a heart
                Instantiate(enemy, parent.position, Quaternion.identity);
                parent.DetachChildren();
            }
            else
            {
                // Defaults to producing a green rupee if nothing else spawns
                Instantiate(greenRupee, parent.position, Quaternion.identity);
                parent.DetachChildren();
            }
        }
    }

}
