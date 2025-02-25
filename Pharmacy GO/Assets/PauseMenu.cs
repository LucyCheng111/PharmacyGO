using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    
    [SerializeField] GameObject pauseMenu;

    [SerializeField] private Slider brightness;
    [SerializeField] private Image brightLevel;
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

    
}
