using TMPro;
using UnityEngine;

public class MultiplierDisplay : MonoBehaviour
{

    // Handles multiplier display game object
    // Multiplier is based off of timer

    [SerializeField] private TMP_Text multText;

    private void Update()
    {
        multText.text = "Bonus: " + TimerManager.Instance.GetMultiplier() + "x";
    }
}