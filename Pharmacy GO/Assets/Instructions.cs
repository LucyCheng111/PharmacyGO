using UnityEngine;
using UnityEngine.SceneManagement;


public class Instructions : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
        // Scene name or scene build index
    }
}
