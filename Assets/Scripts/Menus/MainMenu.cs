using System.Collections;
using System.Diagnostics.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    // Loads level structure and manager for the menu functions

    private const int HUB_SCENE_INDEX = 7;
    private const string MASTER_VOLUME_KEY = "masterVolume";
    private const int DEfAULT_VOLUME_VALUE = 10;
    public AudioMixer audioMixer;
    public TextMeshProUGUI volumeText;
    public Slider volumeSlider;

    public GameObject[] menus;

    [SerializeField] private Button continueButton;
    private bool hasSave;

    private void Start ()
    {

        hasSave = PlayerPrefs.HasKey("UnlockedLevel")
              || PlayerPrefs.HasKey("CoinCount")
              || PlayerPrefs.HasKey("ScoreCount")
              || PlayerPrefs.HasKey("xPos")
              || PlayerPrefs.HasKey("CurrentLevel");

        if (continueButton != null)
            continueButton.gameObject.SetActive(hasSave);


        LoadVolume ();
    }

    public void ContinueGame()
    {
        LevelManager.Instance.continuing = true;
        SceneManager.LoadSceneAsync(HUB_SCENE_INDEX);
    }

    public void PlayGame()
    {
        LevelManager.Instance.continuing = false;
        PlayerPrefs.DeleteKey("UnlockedLevel");
        PlayerPrefs.DeleteKey("CoinCount");
        PlayerPrefs.DeleteKey("ScoreCount");
        PlayerPrefs.DeleteKey("SpawnPointID");
        PlayerPrefs.Save();


        SceneManager.LoadSceneAsync(HUB_SCENE_INDEX);
        // Scene name or scene build index
        // Game scene is currently index 2 
        // Main Menu 0, Instructions 1
    }

    public void LoadInstructions()
    {
        SceneManager.LoadSceneAsync(1);
        // Scene name or scene build index
        // Game scene is currently index 2 
        // Main Menu 0, Instructions 1
    }

    public void SetMenu (GameObject newMenu)
    {
        foreach (GameObject menu in menus)
        {
            menu.SetActive (menu == newMenu);
        }
    }

    public void SetVolume (float sliderValue)
    {
        SaveVolume (sliderValue);

        // Prevents math error in Mathf.Log10()
        if (sliderValue <= 0)
            sliderValue = 0.0001f;

        // Sets the mixer's masterVolume
        float volume = Mathf.Log10 (sliderValue / 20f) * 20f;
        audioMixer.SetFloat (MASTER_VOLUME_KEY, volume);

        SetVolumeText (sliderValue);
    }

    private void SetVolumeText (float volume)
    {
        // Updates the volume text
        string displayText = string.Format ("Volume: {0}%", Mathf.RoundToInt (volume * 5f));
        volumeText.text = displayText;
    }

    public void SaveVolume (float newVolume)
    {
        PlayerPrefs.SetFloat (MASTER_VOLUME_KEY, newVolume);
    }
    public void LoadVolume ()
    {
        float savedVolume = PlayerPrefs.GetFloat (MASTER_VOLUME_KEY, DEfAULT_VOLUME_VALUE);
        volumeSlider.value = savedVolume;
        SetVolume (savedVolume);
    }
    public void Feedback()
    {
        Application.OpenURL("https://oregonstate.qualtrics.com/jfe/form/SV_3IfO3l2H7FbOOuW");
    }
}