using TMPro;
using UnityEngine;

public class MultiplierDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text multText;

    private void Update()
    {
        multText.text = "Bonus: " + TimerManager.Instance.GetMultiplier() + "x";
    }
}