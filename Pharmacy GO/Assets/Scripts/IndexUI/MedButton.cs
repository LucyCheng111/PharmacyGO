using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MedButton : MonoBehaviour
{
    [SerializeField] TMP_Text medName;
    [SerializeField] Image medImage;
    private Medication medication;

    public void Initialize(Medication med)
    {
        medication = med;
        medName.text = medication.medicationName;
        medImage.sprite = medication.image; 
    }

    public void OnButtonClick()
    {
        // Display medication details
        Debug.Log($"Name: {medication.medicationName}\n");
        // Implement additional logic to display details in the UI
    }
}
