using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class IndexUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public static IndexUI instance;    
    [SerializeField] PlayerControl playerControl;
    [SerializeField] private Button Topic1Button;
    [SerializeField] private Button Topic2Button;
    [SerializeField] private Button Topic3Button;
    [SerializeField] private Button Topic4Button; 

    private ScrollRect Topic1ScrollRect;
    private ScrollRect Topic2ScrollRect;
    private ScrollRect Topic3ScrollRect;
    private ScrollRect Topic4ScrollRect;

    public GameObject buttonPrefab;
    public GameObject scrollRectPrefab;
    public GameObject IndexUIPanel;
    

    public void Start()
    {
        // IndexUIPanel.SetActive(true);
        // Set the canvas group to be visible and interactable
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
    }
    /// This method is called when the IndexUI is opened and will keep the game object to the next scene 
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this duplicate
            Destroy(gameObject);
        }
    }

    private void LoadMedications()
    {
        Debug.Log("Loading Medications...");
        Medication[] allMedications = new Medication[0];
        // Load all medications from the Resources folder
        
        Debug.Log("Loaded Level 1 Medications");
        Medication[] medsLevel1 = Resources.LoadAll<Medication>("MedicationData/Level1");
        allMedications = allMedications.Concat(medsLevel1).ToArray();
        
        if(playerControl.HasTriggeredArea(2))
        {
            Debug.Log("Loaded Level 2 Medications");
            Medication[] medsLevel2 = Resources.LoadAll<Medication>("MedicationData/Level2");
            allMedications = allMedications.Concat(medsLevel2).ToArray();
        }
        if(playerControl.HasTriggeredArea(3))
        {
            Debug.Log("Loaded Level 3 Medications");
            Medication[] medsLevel3 = Resources.LoadAll<Medication>("MedicationData/Level3");
            allMedications = allMedications.Concat(medsLevel3).ToArray();
        }
        if(playerControl.HasTriggeredArea(4))
        {
            Debug.Log("Loaded Level 4 Medications");
            Medication[] medsLevel4 = Resources.LoadAll<Medication>("MedicationData/Level4");
            allMedications = allMedications.Concat(medsLevel4).ToArray();
        }

        foreach (Medication med in allMedications)
        {
            if (med.level == "1")
            {
                CreateButton(med, Topic1ScrollRect);
            }
            else if (med.level == "2")
            {
                CreateButton(med, Topic2ScrollRect);
            }
            else if (med.level == "3")
            {
                CreateButton(med, Topic3ScrollRect);
            }
            else if (med.level == "4")
            {
                CreateButton(med, Topic4ScrollRect);
            }
        }
    }

    private void ShowScrollRect(ScrollRect scrollRect)
    {
        Topic1ScrollRect.gameObject.SetActive(scrollRect == Topic1ScrollRect);
        Topic2ScrollRect.gameObject.SetActive(scrollRect == Topic2ScrollRect);
        Topic3ScrollRect.gameObject.SetActive(scrollRect == Topic3ScrollRect);
        Topic4ScrollRect.gameObject.SetActive(scrollRect == Topic4ScrollRect);
    }

    private void SetScrollRectColor(ScrollRect scrollRect, string colorCode)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(colorCode, out color))
        {
            scrollRect.GetComponent<Image>().color = color;
        }
    }

    private void CreateButton(Medication med, ScrollRect scrollRect)
    {
        if (scrollRect == null || scrollRect.content == null)
        {
            Debug.LogError("ScrollRect or its Content is null!");
            return;
        }

        GameObject button = Instantiate(buttonPrefab, scrollRect.content.transform);
        MedButton medButton = button.GetComponent<MedButton>();
        medButton.Initialize(med);
        medButton.SetIndexUIPanel(IndexUIPanel); // Pass IndexUIPanel reference

        Button btnComponent = button.GetComponent<Button>();
        if (btnComponent != null)
        {
            btnComponent.onClick.AddListener(medButton.OnButtonClick);
        }
    }


    private ScrollRect CreateScrollRect()
    {
        GameObject scrollRectObj = Instantiate(scrollRectPrefab, IndexUIPanel.transform);
        ScrollRect scrollComponent = scrollRectObj.GetComponent<ScrollRect>();

        if (scrollComponent == null)
        {
            Debug.LogError("ScrollRect component is missing from prefab!");
            return null;
        }

        Transform contentTransform = scrollRectObj.transform.Find("Viewport/Content");
        if (contentTransform == null)
        {
            Debug.LogError("Content Transform not found in ScrollRect prefab!");
            return null;
        }

        scrollComponent.content = contentTransform.GetComponent<RectTransform>();

        // Set inactive initially
        scrollRectObj.SetActive(false);
        return scrollComponent;
    }

    public void CloseIndexUI()
    {
        // IndexUIPanel.SetActive(false);
        canvasGroup.alpha = 0f;
        playerControl.gameObject.SetActive(true);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    public void OpenIndexUI()
    {
        // IndexUIPanel.SetActive(true);
        canvasGroup.alpha = 1f;
        playerControl.gameObject.SetActive(false);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // playerControl.gameObject.SetActive(false);
        // Create ScrollRects for Topics
        Topic1ScrollRect = CreateScrollRect();
        Topic2ScrollRect = CreateScrollRect();
        Topic3ScrollRect = CreateScrollRect();
        Topic4ScrollRect = CreateScrollRect();

        if (Topic1ScrollRect == null || Topic2ScrollRect == null || 
            Topic3ScrollRect == null || Topic4ScrollRect == null)
        {
            Debug.LogError("One or more ScrollRects failed to instantiate!");
            return;
        }

        // Assign Colors
        SetScrollRectColor(Topic1ScrollRect, "#FF9FE3");
        SetScrollRectColor(Topic2ScrollRect, "#8DE3AD");
        SetScrollRectColor(Topic3ScrollRect, "#F8BE60");
        SetScrollRectColor(Topic4ScrollRect, "#79E8F3");

        // Show Topic 1 by default
        ShowScrollRect(Topic1ScrollRect);
        // Assign Button Clicks
        Topic1Button.onClick.AddListener(() => ShowScrollRect(Topic1ScrollRect));
        Topic2Button.onClick.AddListener(() => ShowScrollRect(Topic2ScrollRect));
        Topic3Button.onClick.AddListener(() => ShowScrollRect(Topic3ScrollRect));
        Topic4Button.onClick.AddListener(() => ShowScrollRect(Topic4ScrollRect));
        
        // Now Load Medications
        LoadMedications();
    }
}

