using UnityEngine;
using TMPro;

// Code mostly inspired by Mister Taft Creates: https://www.youtube.com/watch?v=1NCvpZDtTMI

/// <summary>
/// This class was made to handle general sign behavior
/// </summary>
public class SignScript : MonoBehaviour
{
    // Boolean used to track whether the player is within the trigger radius of the sign object
    bool inRange;

    // Public boolean used to tell the PlayerMovement script whether or not to display "Open" text on the action indicator bar in the GUI
    [HideInInspector] public bool isOpen;

    // Variables created for assigning the components of the sign canvas
    public GameObject dialogBox;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] string dialog;
    [SerializeField] bool isBook;

    // Checks for input given that the player is in range of the sign
    void Update()
    {
        if (inRange && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire3")))
        {
            // In the case of the book object, input triggers the animation of the book opening or closing
            if (isBook)
            {
                if (!isOpen)
                {
                    GetComponent<Animator>().SetBool("isOpen", true);
                    isOpen = true;
                }
                else
                {
                    GetComponent<Animator>().SetBool("isOpen", false);
                    isOpen = false;
                }
            }
            if (dialogText != null)
            {
                // Toggles the active status of the sign canvas in the GUI 
                if (dialogBox.activeInHierarchy)
                    dialogBox.SetActive(false);
                else
                {
                
                    dialogText.text = dialog;
                    dialogBox.SetActive(true);
                }
            }
        }
    }

    // Entering the trigger radius sets the boolean "inRange" to true
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    // Exiting the trigger radius sets the booleans "inRange" and "isOpen" to false
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dialogBox.SetActive(false);
            inRange = false;
            if (isBook)
            {
                GetComponent<Animator>().SetBool("isOpen", false);
                isOpen = false;
            }
        }
    }
}
