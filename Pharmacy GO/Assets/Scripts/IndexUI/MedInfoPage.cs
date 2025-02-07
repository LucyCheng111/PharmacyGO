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
    [SerializeField] private TMP_Text sideEffects;
    [SerializeField] private GameObject infoPanel; // Reference to infoPage

    public void DisplayMedication(Medication med)
    {
        medName.text = med.medicationName;
        medImage.sprite = med.image;
        level.text = med.level;
        organ.text = med.treatmentOrganList;
        information.text = med.information;
        sideEffects.text = med.sideEffects;

        infoPanel.SetActive(true); // Show MedInfoPage
    }

    public void CloseInfo()
    {
        infoPanel.SetActive(false);
    }
}
