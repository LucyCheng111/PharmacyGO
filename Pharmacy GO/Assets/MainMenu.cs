using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(2);
        // Scene name or scene build index
        // Game scene is currently index 2 
        // Main Menu 0, Instructions 1
    }

    public void LoadInstructions()
    {
        SceneManager.LoadSceneAsync(1);
        // Scene name or scene build index
        // Instructions scene is currently index 1
        // Main Menu 0, Game 2
    }
}