using UnityEngine;
using UnityEngine.SceneManagement;


public class Instructions : MonoBehaviour
{
    //Loads instructions upon loading into the main menu

    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
        // Scene name or scene build index
    }
}
