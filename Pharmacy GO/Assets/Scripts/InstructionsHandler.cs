using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InstructionsHandler : MonoBehaviour
{

    // Handler for the intructions object

    public GameObject startMenu;
    [TextArea(2,5)]
    public string[] instructions;
    public int currentInstructionIndex;
    public TextMeshProUGUI displayText;

    public TextMeshProUGUI nextButtonText;

    public void OnEnable ()
    {
        currentInstructionIndex = 0;
        UpdateText ();
    }

    public void IncrementInstructionsText ()
    {
        if (currentInstructionIndex == instructions.Length - 1)
        {
            FindFirstObjectByType<MainMenu> ().PlayGame ();
            return;
        }
        currentInstructionIndex++;
        UpdateText ();
    }

    public void DecrementInstructionsText ()
    {
        if (currentInstructionIndex == 0)
        {
            FindFirstObjectByType<MainMenu> ().SetMenu (startMenu);
            return;
        }
        currentInstructionIndex--;
        UpdateText ();
    }
    private void UpdateText ()
    {
        displayText.text = instructions[currentInstructionIndex];
        if (currentInstructionIndex == instructions.Length - 1)
        {
            nextButtonText.text = "Start";
        }
        else
        {
            nextButtonText.text = "Next";
        }
    }
}
