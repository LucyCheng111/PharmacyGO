using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MedInfoPage : MonoBehaviour
{
    public static MedInfoPage Instance; // Singleton for easy access

    [SerializeField] private TMP_Text medName;
    [SerializeField] private Image medImage;
    [SerializeField] private TMP_Text level;
    [SerializeField] private TMP_Text organ;
    [SerializeField] private TMP_Text information;
    [SerializeField] private GameObject infoPanel; // Reference to infoPage

    public void DisplayMedication(Medication med)
    {
        medName.text = med.medicationName;
        medImage.sprite = med.image;
        level.text = med.level;
        organ.text = med.treatmentOrganList;
        
        // curently the string is fix to just Description and Side Effect, can make it dynamic later if there is a need
        information.text = "Description: \n" + med.information + "\n\nSide Effect: \n" + med.sideEffects;

        infoPanel.SetActive(true); // Show MedInfoPage
    }

    public void CloseInfo()
    {
        infoPanel.SetActive(false);
    }
}
