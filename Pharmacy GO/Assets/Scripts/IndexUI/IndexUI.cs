using UnityEngine;
using UnityEngine.UI;

public class IndexUI : MonoBehaviour
{
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
        IndexUIPanel.SetActive(true);
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

    private void LoadMedications()
    {
        Medication[] medications = Resources.LoadAll<Medication>("Medications");

        foreach (Medication med in medications)
        {
            CreateButton(med, Topic1ScrollRect);
            CreateButton(med, Topic2ScrollRect);
            CreateButton(med, Topic3ScrollRect);
            CreateButton(med, Topic4ScrollRect);
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
        IndexUIPanel.SetActive(false);
    }
}
