using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    // Manager for the Pause Menu
    
    [SerializeField] GameObject pauseMenu;

    [SerializeField] private Slider brightness;
    [SerializeField] private Image brightLevel;
    private const int HUB_SCENE_INDEX = 7;
    private const string MASTER_VOLUME_KEY = "masterVolume";
    private const int DEfAULT_VOLUME_VALUE = 10;
    public AudioMixer audioMixer;
    public TextMeshProUGUI volumeText;
    public Slider volumeSlider;


    private SpriteRenderer[] spriteRenderers;


    void Start()
    {
        // Ensure the slider and image are assigned
        if (brightness != null && brightLevel != null)
        {

            // Set the slider's starting value to 1 (full brightness)
            brightness.value = 1;

            // Ensure the black screen's alpha matches the slider's starting value
            OnBrightnessChanged(brightness.value);

            // Add listener to the slider
            brightness.onValueChanged.AddListener(OnBrightnessChanged);
        }
    }

    private void OnBrightnessChanged(float value)
    {
        // Get the current color
        Color color = brightLevel.color;
        if (value < 0.02f)
        {
            color.a = 0.98f;
        }
        // Otherwise, set alpha to 1 - value (inverse of the slider value)
        else
        {
            color.a = 1 - value;
        }
        // Assign the modified color back to the Image
        brightLevel.color = color;
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void returnMain()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 0;
        SceneManager.LoadSceneAsync(0);
        // Scene name or scene build index
        // Game scene is currently index 2 
        // Main Menu 0, Instructions 1
    }

    public void SetVolume(float sliderValue)
    {
        SaveVolume(sliderValue);

        // Prevents math error in Mathf.Log10()
        if (sliderValue <= 0)
            sliderValue = 0.0001f;

        // Sets the mixer's masterVolume
        float volume = Mathf.Log10(sliderValue / 20f) * 20f;
        audioMixer.SetFloat(MASTER_VOLUME_KEY, volume);

        SetVolumeText(sliderValue);
    }

    private void SetVolumeText(float volume)
    {
        // Updates the volume text
        string displayText = string.Format("Volume: {0}%", Mathf.RoundToInt(volume * 5f));
        volumeText.text = displayText;
    }

    public void SaveVolume(float newVolume)
    {
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, newVolume);
    }
    public void LoadVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, DEfAULT_VOLUME_VALUE);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);
    }

    public void Feedback()
    {
        Application.OpenURL("https://oregonstate.qualtrics.com/jfe/form/SV_3IfO3l2H7FbOOuW");
    }

    public void AdjustBrightness(float BrightnessValue)
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = new Color(BrightnessValue, BrightnessValue, BrightnessValue, spriteRenderer.color.a);
        }
    }


}
