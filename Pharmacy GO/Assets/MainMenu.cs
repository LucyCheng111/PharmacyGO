using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private const int HUB_SCENE_INDEX = 7;
    private const string MASTER_VOLUME_KEY = "masterVolume";
    private const int DEfAULT_VOLUME_VALUE = 10;
    public AudioMixer audioMixer;
    public TextMeshProUGUI volumeText;
    public Slider volumeSlider;

    private void Start ()
    {
        LoadVolume ();
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(HUB_SCENE_INDEX);
        // Scene name or scene build index
        // Game scene is currently index 2 
        // Main Menu 0, Instructions 1
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
}