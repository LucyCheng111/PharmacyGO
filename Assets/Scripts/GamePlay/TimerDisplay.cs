using TMPro;
using UnityEngine;

public class TimerDisplay : MonoBehaviour
{

    // Handles the timer game object

    [SerializeField] private TMP_Text timerText;

    private void Update()
    {
        timerText.text = "Time: " + TimerManager.Instance.GetTime().ToString("#.00");
    }
}