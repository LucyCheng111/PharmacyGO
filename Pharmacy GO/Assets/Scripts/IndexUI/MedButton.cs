using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MedButton : MonoBehaviour
{
    [SerializeField] TMP_Text medName;
    [SerializeField] Image medImage;
    private Medication medication;

    public GameObject medInfoPrefab; // Assign the MedInfoPage prefab in Inspector
    public GameObject IndexUIPanel; // Assign the IndexUIPanel in Inspector
    private void Awake()
    {
        if (IndexUIPanel == null)
        {
            IndexUIPanel = GameObject.Find("IndexUIPanel"); // Ensure correct name
        }
    }

    public void Initialize(Medication med)
    {
        medication = med;
        medName.text = medication.medicationName;
        medImage.sprite = medication.image; 
    }

    public void OnButtonClick()
    {
        MedInfoPage existingPage = IndexUIPanel.GetComponentInChildren<MedInfoPage>();
        if (existingPage != null)
        {
            Destroy(existingPage.gameObject); // Destroy old page before creating a new one
        }

        GameObject medInfoObj = Instantiate(medInfoPrefab, IndexUIPanel.transform);
        MedInfoPage medInfoPage = medInfoObj.GetComponent<MedInfoPage>();
        medInfoPage.DisplayMedication(medication);

    }
    public void SetIndexUIPanel(GameObject panel)
    {
        IndexUIPanel = panel;
    }

}
