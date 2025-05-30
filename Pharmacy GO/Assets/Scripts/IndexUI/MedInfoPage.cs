using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MedInfoPage : MonoBehaviour
{

    // Handles the index page game object

    public static MedInfoPage Instance; // Singleton for easy access

    [SerializeField] private TMP_Text medName;
    [SerializeField] private Image medImage;
    [SerializeField] private TMP_Text level;
    [SerializeField] private TMP_Text organ;
    [SerializeField] private TMP_Text information;
    [SerializeField] private GameObject infoPanel; // Reference to the panel that displays medication info

    public void DisplayMedication(Medication med)
    {
        // Basic Info
        medName.text = med.medicationName;
        medImage.sprite = med.image;
        level.text = med.level;
        organ.text = med.treatmentOrgan;

        // Build a descriptive text from the Medication fields
        // Adjust format and labels to suit your needs
        string descriptionText =
            $"<b>Pharmacologic Class:</b> {med.pharmacologicClass}\n" +
            $"<b>Mechanism of Action:</b> {med.mechanismOfAction}\n" +
            $"<b>Therapeutic Uses:</b> {med.therapeuticUses}\n" +
            $"<b>Side Effects:</b> {med.sideEffects}\n" +
            $"<b>Major Drug Interactions:</b> {med.majorDrugInteractions}\n" +
            $"<b>Fun Fact:</b> {med.funFact}";

        // Assign to the UI text field
        information.text = descriptionText;

        // Show the MedInfoPage panel
        infoPanel.SetActive(true);
    }

    public void CloseInfo()
    {
        infoPanel.SetActive(false);
    }
}
