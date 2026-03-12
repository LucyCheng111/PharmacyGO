using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class DialogBox : MonoBehaviour
{

    // Manager for the dialog box
    // Also handles dialog typing and logic

    private string fullText;
    private Coroutine typingCoroutine;

    public enum AnswersType
    {
        None,
        String,
        Image
    };
    public int letterPerSecond = 30;
    private bool answerSelected = false;
    private int aiCurrentChoice = -1;
    public bool GetAnswerSelected()
    { 
        return answerSelected; 
    }
    public void SetAnswerSelected(bool value)
    {
        answerSelected = value;
    }
    [SerializeField] private Color highlightedColor;
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private GameObject actionSelector;
    [SerializeField] private GameObject optionSelector;
    [SerializeField] private List<TMP_Text> actionTexts;

    // Separated into distinct arrays collected by child object name.
    private RawImage[] optionImages;        // Image child on each option slot
    private TMP_Text[] optionStrings;       // Text child on each option slot
    private TMP_Text[] optionCaptions;      // CaptionText child on each option slot
    private Image[] optionOutlines;         // Outline child on each option slot

    public AnswersType currentOptions = AnswersType.None;

    private readonly Color SELECTED_COLOR = Color.black;
    private readonly Color UNSELECTED_COLOR = Color.clear;
    private readonly Color CORRECT_COLOR = Color.green;
    private readonly Color INCORRECT_COLOR = Color.red;
    private readonly Color AI_CHOICE_COLOR = new Color(1f, 0.65f, 0f);
    private const float MAX_OPTION_WIDTH = 150f;
    private const float MAX_OPTION_HEIGHT = 100f;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/letterPerSecond);
        }
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableOptionSelector(bool enabled)
    {
        optionSelector.SetActive(enabled);

        // Collect each component type by child name
        var slots = optionSelector.GetComponentsInChildren<Transform>(true);

        var images = new List<RawImage>();
        var strings = new List<TMP_Text>();
        var captions = new List<TMP_Text>();
        var outlines = new List<Image>();

        foreach (Transform t in slots)
        {
            if (t.name == "Image")
            {
                RawImage ri = t.GetComponent<RawImage>();
                if (ri != null) images.Add(ri);
            }
            else if (t.name == "Text")
            {
                TMP_Text tmp = t.GetComponent<TMP_Text>();
                if (tmp != null) strings.Add(tmp);
            }
            else if (t.name == "CaptionText")
            {
                TMP_Text tmp = t.GetComponent<TMP_Text>();
                if (tmp != null) captions.Add(tmp);
            }
            else if (t.name == "Outline")
            {
                Image img = t.GetComponent<Image>();
                if (img != null) outlines.Add(img);
            }
        }

        optionImages = images.ToArray();
        optionStrings = strings.ToArray();
        optionCaptions = captions.ToArray();
        optionOutlines = outlines.ToArray();
    }

    public void UpdateChoiceSelection(int selectedChoice)
    {
        if (answerSelected)
            return;

        for (int i = 0; i < optionOutlines.Length; i++)
        {
            if (i == aiCurrentChoice)
                continue;

            bool selected = i == selectedChoice;
            Color color = selected ? SELECTED_COLOR : UNSELECTED_COLOR;
            optionOutlines[i].color = color;
        }
    }

    public void SetAnswers(Option[] answers)
    {
        // Check all options for images, not just answers[0]
        bool anyImage = false;
        for (int i = 0; i < answers.Length; i++)
        {
            if (answers[i].useImage && !string.IsNullOrEmpty(answers[i].imageLink))
            {
                anyImage = true;
                break;
            }
        }

        if (anyImage)
        {
            currentOptions = AnswersType.Image;

            // Collect image URLs and caption text separately
            // Each slot also shows RawImage and CaptionText (if text is present)
            string[] imagePaths = new string[answers.Length];
            string[] captionTexts = new string[answers.Length];

            for (int i = 0; i < answers.Length; i++)
            {
                if (answers[i].useImage && !string.IsNullOrEmpty(answers[i].imageLink))
                {
                    imagePaths[i] = answers[i].imageLink;
                    captionTexts[i] = answers[i].text;
                }
                else
                {
                    // Text-only option in an otherwise image question — no image, caption only
                    imagePaths[i] = null;
                    captionTexts[i] = answers[i].text;
                }
            }

            for (int i = 0; i < optionImages.Length; i++)
            {
                if (i < answers.Length)
                {
                    optionImages[i].transform.parent.gameObject.SetActive(true);
                    optionStrings[i].gameObject.SetActive(false);

                    bool hasCaption = !string.IsNullOrEmpty(captionTexts[i]);
                    if (i < optionCaptions.Length)
                    {
                        optionCaptions[i].gameObject.SetActive(hasCaption);
                        optionCaptions[i].text = hasCaption ? captionTexts[i] : "";
                    }
                }
                else
                {
                    optionImages[i].transform.parent.gameObject.SetActive(false);
                }
            }

            StartCoroutine(loadImages(imagePaths));
        }
        else
        {
            currentOptions = AnswersType.String;

            for (int i = 0; i < optionImages.Length; i++)
            {
                if (i < answers.Length)
                {
                    optionImages[i].transform.parent.gameObject.SetActive(true);
                    optionImages[i].gameObject.SetActive(false);
                    optionStrings[i].gameObject.SetActive(true);
                    if (i < optionCaptions.Length)
                        optionCaptions[i].gameObject.SetActive(false);
                }
                else
                {
                    optionImages[i].transform.parent.gameObject.SetActive(false);
                }
            }

            loadStrings(answers);
        }
    }

    void loadStrings(Option[] answers)
    {
        for (int i = 0; i < optionStrings.Length; i++)
        {
            if (i < answers.Length)
                optionStrings[i].text = answers[i].text;
        }
    }

    IEnumerator loadImages(string[] paths)
    {
        for (int i = 0; i < paths.Length; i++)
        {
            // Skip slots with no image (text-only option in an image question)
            if (string.IsNullOrEmpty(paths[i]))
            {
                optionImages[i].gameObject.SetActive(false);
                continue;
            }

            // Use UnityWebRequestTexture to download raw image bytes directly
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(paths[i]);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D newTexture = DownloadHandlerTexture.GetContent(request);
                float widthDividend = newTexture.width / MAX_OPTION_WIDTH;
                float heightDividend = newTexture.height / MAX_OPTION_HEIGHT;
                float maxSize = Mathf.Max(widthDividend, heightDividend);
                optionImages[i].gameObject.SetActive(true);
                optionImages[i].texture = newTexture;

                Vector2 size = new Vector2(newTexture.width, newTexture.height) / maxSize;
                size -= new Vector2(5, 5);
                optionImages[i].rectTransform.sizeDelta = size;
            }
            else
            {
                Debug.LogError("Image Load Error: " + request.error);
                Debug.LogError("Failed Image Path: " + paths[i]);
                optionImages[i].gameObject.SetActive(false);
            }
        }
    }

    public bool DisplayAnswer(int selectedChoiceIndex, int correctAnswerIndex)
    {
        answerSelected = true;
        optionOutlines[selectedChoiceIndex].color = INCORRECT_COLOR;
        optionOutlines[correctAnswerIndex].color = CORRECT_COLOR;
        return selectedChoiceIndex == correctAnswerIndex;
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            if (i == selectedAction)
                actionTexts[i].color = Color.cyan;
            else
                actionTexts[i].color = Color.black;
        }
    }

    public void ResetDalogBox()
    {
        answerSelected = false;
        aiCurrentChoice = -1;
        dialogText.text = "";
        currentOptions = AnswersType.None;
        EnableActionSelector(false);
        EnableOptionSelector(false);
    }

    public void ForceCompleteText()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogText.text = fullText;
        }
    }

    private IEnumerator TypeText(string dialog)
    {
        fullText = dialog;
        dialogText.text = "";
        foreach (char letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }
    }

    // ==== AI rival DialogBox (orange) ====

    public void ShowAIChoice(int aiAnswer)
    {
        aiCurrentChoice = aiAnswer;
        if (aiAnswer >= 0 && aiAnswer < optionOutlines.Length)
            optionOutlines[aiAnswer].color = AI_CHOICE_COLOR;
    }

    public void ClearAIChoice()
    {
        aiCurrentChoice = -1;
        for (int i = 0; i < optionOutlines.Length; i++)
            optionOutlines[i].color = UNSELECTED_COLOR;
    }
}