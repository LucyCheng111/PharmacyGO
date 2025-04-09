using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Text;
using System;

public class DialogBox : MonoBehaviour
{

    public enum AnswersType
    {
        None,
        String,
        Image
    };
    public int letterPerSecond = 30;
    private bool answerSelected = false;
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
    private RawImage[] optionImages;
    private TMP_Text[] optionStrings;
    private Image[] optionOutlines;
    public AnswersType currentOptions = AnswersType.None;

    private readonly Color SELECTED_COLOR = Color.black;
    private readonly Color UNSELECTED_COLOR = Color.clear;
    private readonly Color CORRECT_COLOR = Color.green;
    private readonly Color INCORRECT_COLOR = Color.red;
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
    //
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
        optionImages = optionSelector.GetComponentsInChildren<RawImage>(true);
        optionStrings = optionSelector.GetComponentsInChildren<TMP_Text>(true);
        optionOutlines = optionSelector.GetComponentsInChildren<Image>(true);
    }

    public void UpdateChoiceSelection(int selectedChoice)
    {
        if (answerSelected)
            return;

        for (int i = 0; i < optionOutlines.Length; i++)
        {
            bool selected = i == selectedChoice;
            Color color = selected ? SELECTED_COLOR : UNSELECTED_COLOR;
            optionOutlines[i].color = color;
        }
    }

    public void SetAnswers(Option[] answers)
    {
        // Check type of answer 1
        (Option.OptionType, string) firstIndex = answers[0].grabOption();
        Debug.Log(firstIndex.Item1);
        string[] values = new string[answers.Length];
        for (int i = 0; i < answers.Length; i++)
        {
            values[i] = answers[i].grabOption().Item2;
        }

        switch (firstIndex.Item1)
        {
            case Option.OptionType.None:
                break;
            case Option.OptionType.String:
                currentOptions = AnswersType.String;
                for (int i = 0; i < optionImages.Length; i++)
                {
                    optionImages[i].gameObject.SetActive(false);
                    optionStrings[i].gameObject.SetActive(true);
                }
                loadStrings(values);
                break;
            case Option.OptionType.Image:
                currentOptions = AnswersType.Image;
                for (int i = 0; i < optionImages.Length; i++)
                {
                    optionImages[i].gameObject.SetActive(true);
                    optionStrings[i].gameObject.SetActive(false);
                }
                StartCoroutine(loadImages(values));
                break;
            default:
                break;
        }
    }

    void loadStrings(string[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            optionStrings[i].text = values[i];
        }
    }

    IEnumerator loadImages(string[] paths)
    {
        for (int i = 0; i < paths.Length; i++)
        {
            UnityWebRequest request = UnityWebRequest.Get (paths[i]);
            request.SetRequestHeader ("authorization", Database.MAIN_AUTH_TOKEN);

            yield return request.SendWebRequest ();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                JObject responseJson = JObject.Parse (responseText);                    // Convert text to json object
                string base64Content = responseJson.GetValue ("content").ToString ();   // Get base64 from json
                byte[] byteArrayContent = Convert.FromBase64String (base64Content);     // Convert base64 to byte array
                string databaseContent = Encoding.UTF8.GetString (byteArrayContent);    // Convert byte array to string
                databaseContent = databaseContent.Replace ("\"", "");
                byteArrayContent = Convert.FromBase64String (databaseContent);     // Convert base64 to byte array


                Texture2D newTexture = new Texture2D (1, 1);
                newTexture.LoadImage (byteArrayContent);
                newTexture.Apply ();
                float widthDividend = newTexture.width / MAX_OPTION_WIDTH;
                float heightDividend = newTexture.height / MAX_OPTION_HEIGHT;
                float maxSize = Mathf.Max (widthDividend, heightDividend);
                optionImages[i].texture = newTexture;

                Vector2 size = new Vector2 (newTexture.width, newTexture.height) / maxSize;
                size -= new Vector2 (5, 5);
                optionImages[i].rectTransform.sizeDelta = size;
            }
            else
            {
                Debug.LogError ("Image Load Error: " + request.error);
            }
        }
    }

    // Display the Correct or Wrong answer
    public bool DisplayAnswer(int selectedChoiceIndex, int correctAnswerIndex)
    {
        answerSelected = true;

        // If the selected choice was correct, it will override the incorrect color with the correct color
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
        dialogText.text = "";
        currentOptions = AnswersType.None;
        EnableActionSelector(false);
        EnableOptionSelector(false);
    }
    
}
