using UnityEngine;
using UnityEngine.SceneManagement;

// This script provides simple functions to be accessed by GUI buttons
public class GameOverManager : MonoBehaviour
{
    // Loads main menu scene
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    // Closes GUI
    public void CloseGameOverGui()
    {
        gameObject.SetActive(false);
    }

    // Quits game
    public void QuitGame()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }
}