using UnityEngine;
using UnityEngine.SceneManagement;

// This simple script is just for the UI button to be able to load the main game scene from the menu as well as quit
public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }
}
