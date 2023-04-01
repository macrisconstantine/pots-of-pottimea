using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// I'm not sure why this code was not deleted, since it seems like a less-developed duplicate of PlayerHealth, but I left it in for good measure.
/// </summary>

public class Player : MonoBehaviour
{
    public int health;
    public int numOfHearts;

    // Sprites to be used to represent health levels
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite quarterHeart;
    public Sprite halfHeart;
    public Sprite threeQuartHeart;
    public Sprite emptyHeart;

    // This code is a bit sloppy perhaps, but it was the only working solution I arrived at on my own.
    // All attempts for simpler code ended up with unexpected bugs
    private void Update()
    {   
        // Loop through the number of hearts and match the full hearts with the respective HP level
        for (int i = 0; i < hearts.Length; i++)
        {
            if (health == 0)
            {
                hearts[i].sprite = emptyHeart;
            } else if (i<health/4)
            {
                hearts[i].sprite = fullHeart;

                // If the the number is not divisible by four, select an appropriate sprite to display either quarter, half, or three-quarter's health
            } else if (i==health/4)
            {
                if (health % 4 == 3)
                {
                    hearts[i].sprite = threeQuartHeart;
                }
                else if (health % 4 == 2)
                {
                    hearts[i].sprite = halfHeart;
                }
                else if (health % 4 == 1)
                {
                    hearts[i].sprite = quarterHeart;
                }
                // Otherwise put an empty heart sprite
            } else if (i>health/4)
            {
                hearts[i].sprite = emptyHeart;
            }

            // Display the appropriate number of hearts in the GUI respective of how many hearts the player has unlocked in game
            if (i < numOfHearts)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}

