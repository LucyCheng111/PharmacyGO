using System.Collections;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Xml.XPath;
using System.Threading.Tasks;

public class saveManager : MonoBehaviour
{
    public GameObject player;
    public static saveManager Instance { get; private set; }
    public bool running = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        // Check if there already is an existing GameController
        // Especially when returning to the Sample Scene
        if(Instance != null & Instance != this )
        {
            Destroy( gameObject );
            return;
        }
        Instance = this;
        DontDestroyOnLoad( gameObject );
    }

    void OnApplicationQuit()
    {
        UnityEngine.Debug.Log("Application ending after " + Time.time + " seconds");
    }

    public void HandleUpdate()
    {
        if (!running)
        {
            StartCoroutine(AutoSave());
        }
    }

    IEnumerator AutoSave()
    {
        running = true;
        yield return new WaitForSeconds(1);
        SaveGame();
        running = false;
    }

    // called in game controller awake
    public async void LoadGame()
    {
        int destindex = PlayerPrefs.GetInt("CurrentLevel");
        if (SceneManager.GetActiveScene().buildIndex != destindex)
        {
            await SceneManager.LoadSceneAsync(destindex);
        }
        await Task.Yield(); //wait for one frame for Awake()

        float xval = PlayerPrefs.GetFloat("xPos");
        float yval = PlayerPrefs.GetFloat("yPos");
        player.transform.position = new Vector2(xval, yval);
    }
    public void SaveGame()
    {
        if(SceneManager.GetActiveScene().name != "Main Menu")
        {
            PlayerPrefs.SetFloat("xPos", player.transform.position.x);
            PlayerPrefs.SetFloat("yPos", player.transform.position.y);
            PlayerPrefs.SetInt("CurrentLevel", SceneManager.GetActiveScene().buildIndex);

            float xval = PlayerPrefs.GetFloat("xPos");
            float yval = PlayerPrefs.GetFloat("yPos");
        }
        
        
        //All other playerprefs are saved upon editing in their respective scripts.
        
    }
}
