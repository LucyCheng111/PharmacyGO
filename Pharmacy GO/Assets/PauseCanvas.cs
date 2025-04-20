using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseCanvas : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private static PauseCanvas instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenu.SetActive(false); // Ensure the pause menu is hidden at the start
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PauseMenuOnOFF()
    {
        bool isActive = pauseMenu.activeSelf;
        pauseMenu.SetActive(!isActive);
    }

    public void ReturnToMainMenu()
    {
   
        Time.timeScale = 1f;


        Destroy(gameObject);

   
        SceneManager.LoadScene(0);
    }
}
