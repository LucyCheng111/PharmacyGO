using UnityEngine;
using UnityEngine.UI;

public class IndexUI : MonoBehaviour
{
    [SerializeField] private Button Topic1Button;
    [SerializeField] private Button Topic2Button;
    [SerializeField] private Button Topic3Button;
    [SerializeField] private Button Topic4Button; 
    
    [SerializeField] private ScrollRect Topic1ScrollRect;
    [SerializeField] private ScrollRect Topic2ScrollRect;
    [SerializeField] private ScrollRect Topic3ScrollRect;
    [SerializeField] private ScrollRect Topic4ScrollRect;

    public GameObject buttonPrefab;
    public void Start()
    {
        Topic1Button.onClick.AddListener(() =>
        {
            Topic1ScrollRect.gameObject.SetActive(true);
            Topic2ScrollRect.gameObject.SetActive(false);
            Topic3ScrollRect.gameObject.SetActive(false);
            Topic4ScrollRect.gameObject.SetActive(false);
        });
        Topic2Button.onClick.AddListener(() =>
        {
            Topic1ScrollRect.gameObject.SetActive(false);
            Topic2ScrollRect.gameObject.SetActive(true);
            Topic3ScrollRect.gameObject.SetActive(false);
            Topic4ScrollRect.gameObject.SetActive(false);
        });
        Topic3Button.onClick.AddListener(() =>
        {
            Topic1ScrollRect.gameObject.SetActive(false);
            Topic2ScrollRect.gameObject.SetActive(false);
            Topic3ScrollRect.gameObject.SetActive(true);
            Topic4ScrollRect.gameObject.SetActive(false);
        });
        Topic4Button.onClick.AddListener(() =>
        {
            Topic1ScrollRect.gameObject.SetActive(false);
            Topic2ScrollRect.gameObject.SetActive(false);
            Topic3ScrollRect.gameObject.SetActive(false);
            Topic4ScrollRect.gameObject.SetActive(true);
        });
    }

    private void OnEnable()
    {
        // Get all medications from the database
        // Medications Exist in Resources/Medications Folder
        Medication[] medications = Resources.LoadAll<Medication>("Medications");
        foreach (Medication med in medications)
        {
            CreateButton(med, Topic1ScrollRect);
            CreateButton(med, Topic2ScrollRect);
            CreateButton(med, Topic3ScrollRect);
            CreateButton(med, Topic4ScrollRect);
            // if (med.level == "1")
            //     CreateButton(med, Topic1ScrollRect);
            // else if (med.level == "2")
            //     CreateButton(med, Topic2ScrollRect);
            // else if (med.level == "3")
            //     CreateButton(med, Topic3ScrollRect);
            // else if (med.level == "4")
            //     CreateButton(med, Topic4ScrollRect);
            // else
            //     Debug.LogError("Invalid level for medication: " + med.medicationName);
        }
    }

    private void CreateButton(Medication med, ScrollRect scrollRect)
    {
        GameObject button = Instantiate(buttonPrefab, scrollRect.content.transform);
        MedButton medButton = button.GetComponent<MedButton>();
        medButton.Initialize(med);

        // Ensure button is interactive
        Button btnComponent = button.GetComponent<Button>();
        if (btnComponent != null)
        {
            btnComponent.onClick.AddListener(medButton.OnButtonClick);
        }
    }
}
