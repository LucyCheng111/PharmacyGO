using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

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
    public AnswersType currentOptions = AnswersType.None;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        //Debug.Log("This should go between the incorrect statements.");
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
    }

    public void UpdateChoiceSelection(int selectedChoice)
    {
        if (answerSelected)
            return;
        else
        {
            for (int i = 0; i < optionImages.Length; i++)
            {
                if (i == selectedChoice)
                {
                    if (currentOptions == AnswersType.String)
                    {
                        optionStrings[i].color = Color.cyan;
                    }
                    else if (currentOptions == AnswersType.Image)
                    {
                        optionImages[i].gameObject.GetComponent<Outline>().effectColor = new Color(0, 0, 1, 1);
                    }
                }
                else
                {
                    if (currentOptions == AnswersType.String)
                    {
                        optionStrings[i].color = Color.black;
                    }
                    else if (currentOptions == AnswersType.Image)
                    {
                        optionImages[i].gameObject.GetComponent<Outline>().effectColor = new Color(0, 0, 0, 0);
                    }
                }
            }
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
            UnityWebRequest texture = UnityWebRequestTexture.GetTexture ("https://tornquisterik.github.io/Webpage/images/" + paths[i]);
            yield return texture.SendWebRequest ();
            Texture2D option = DownloadHandlerTexture.GetContent (texture);
            optionImages[i].texture = option;
        }
    }

    // Display the Correct or Wrong answer
    public bool DisplayAnswer(int selectedChoiceIndex, int correctAnswerIndex)
    {
        answerSelected = true;
        //Debug.Log("Selected choice : " + choices[selectedChoiceIndex].text);
        if (currentOptions == AnswersType.String)
        {
            optionStrings[correctAnswerIndex].color = Color.green;
        }
        else if (currentOptions == AnswersType.Image)
        {
            optionImages[correctAnswerIndex].gameObject.GetComponent<Outline>().effectColor = new Color(0, 1, 0, 1);
        }

        if (selectedChoiceIndex != correctAnswerIndex)
        {
            if (currentOptions == AnswersType.String)
            {
                optionStrings[selectedChoiceIndex].color = Color.red;
            }
            else if (currentOptions == AnswersType.Image)
            {
                optionImages[selectedChoiceIndex].gameObject.GetComponent<Outline>().effectColor = new Color(1, 0, 0, 1);
            }
            return false;
        }
        return true;
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
