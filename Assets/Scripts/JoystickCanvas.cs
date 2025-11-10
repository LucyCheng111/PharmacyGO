using UnityEngine;

public class JoystickCanvas : MonoBehaviour
{

    // Handles the Joystick UI object

    private static JoystickCanvas instance;
    [SerializeField] private GameObject joystickCanvas;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        // If running on a mobile device, enable joystick by default
        if (Application.isMobilePlatform)
        {
            joystickCanvas.SetActive(true);
        }
        else
        {
            // For desktop or other platforms, maybe hide it by default
            joystickCanvas.SetActive(false);
        }
    }
    public void JoystickOnOff(){
        bool isActive = joystickCanvas.activeSelf;
        joystickCanvas.SetActive(!isActive);
    }
}
