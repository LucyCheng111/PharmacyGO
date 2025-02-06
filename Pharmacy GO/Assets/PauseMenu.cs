using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public void Pause()
    {
        pauseMenu.SetActive(true);
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
    }
}
