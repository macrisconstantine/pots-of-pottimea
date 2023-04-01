using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This class was created to control some basic elements of the UI overlay.
/// </summary>
public class UIManager : MonoBehaviour
{
    // Variables made to keep track of books and rupees
    private int bookCount = 0;
    public int rupeeCount = 0;

    // Editor assignment used for the different Canvas objects
    public TextMeshProUGUI rupeeUI;
    [SerializeField] Image[] books;
    [SerializeField] GameObject fadeInToGame;

    void Start()
    {
        if (fadeInToGame != null)
        {
            //fadeInToGame.SetActive(true);
        }
    }

    // Update function used to display the current rupee count in the in-game UI overlay
    void Update()
    {
        if (rupeeUI != null)
            rupeeUI.text = "" + (rupeeCount).ToString("D3");
    }

    // Public function used to add rupees
    public void AddRupees(int rupees)
    {
        rupeeCount += rupees;
    }

    // Public function used to collect book with the ID given as a parameter
    public void collectBook(int id)
    {
        // Sets the respective book image as active in the pause screen and increments the number of books collected
        books[id].gameObject.SetActive(true);
        bookCount++;
    }
}
