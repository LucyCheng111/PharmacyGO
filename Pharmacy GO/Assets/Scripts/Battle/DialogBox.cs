using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class DialogBox : MonoBehaviour
{
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
    [SerializeField] private GameObject choiceSelector;
    [SerializeField] private GameObject ImageChoiceSelector;
    [SerializeField] List<TMP_Text> actionTexts;
    [SerializeField] List<TMP_Text> choices;
    [SerializeField] private List<Image> imageChoices;
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

    public void EnableChoiceSelector(bool enabled)
    {
        choiceSelector.SetActive(enabled);
    }

    public void EnableImageChoiceSelector(bool enabled)
    {
        ImageChoiceSelector.SetActive(enabled);
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
    public void UpdateChoiceSelection(int selectedChoice)
    {
        if (answerSelected)
            return;
        else
        {
            for (int i = 0; i < choices.Count; i++)
            {
                if (i == selectedChoice)
                    choices[i].color = Color.cyan;
                else
                    choices[i].color = Color.black;
            }
        }
    }

    public void UpdateImageChoiceSelection(int selectedChoice)
    {
        if (answerSelected) return;

        for (int i = 0; i < imageChoices.Count; i++)
        {
            Outline outline = imageChoices[i].GetComponent<Outline>();

            if (outline != null)
            {
                if (i == selectedChoice)
                {
                    outline.effectColor = new Color(0, 1, 1, 1); // Blue highlight
                }
                else
                {
                    outline.effectColor = new Color(0, 0, 0, 0); // Fully transparent
                }
            }
        }
    }
    // fill in the answer texts into choices container
    public void SetAnswerTexts(string[] answers)
    {
        for (int i = 0; i < choices.Count; i++)
        {
            if (i < answers.Length)
                choices[i].text = answers[i];
            else
                choices[i].text = "";
        }
    }

    public void SetAnswerImages(Sprite[] answerImages)
    {
        EnableChoiceSelector(false); // Use image choice selector
        EnableImageChoiceSelector(true);

        for (int i = 0; i < imageChoices.Count; i++)
        {
            if (i < answerImages.Length && answerImages[i] != null)
            {
                imageChoices[i].sprite = answerImages[i];
                imageChoices[i].preserveAspect = true;
                imageChoices[i].enabled = true;
            }
            else
            {
                imageChoices[i].sprite = null;
                imageChoices[i].enabled = false;
            }
        }
    }

    // Display the Correct or Wrong answer
    public bool DisplayAnswer(int selectedChoiceIndex, int correctAnswerIndex)
    {
        answerSelected = true;
        Debug.Log("Selected choice : " + choices[selectedChoiceIndex].text);
        if (selectedChoiceIndex == correctAnswerIndex){
            choices[selectedChoiceIndex].color = Color.green;
            return true;
        }
        else{
            choices[selectedChoiceIndex].color = Color.red;
            choices[correctAnswerIndex].color = Color.green;
            return false;
        }
    }

    public bool DisplayImageAnswer(int selectedChoiceIndex, int correctAnswerIndex)
    {
        answerSelected = true;
        Debug.Log("Selected choice index: " + selectedChoiceIndex);

        for (int i = 0; i < imageChoices.Count; i++)
        {
            Outline outline = imageChoices[i].GetComponent<Outline>();

            if (outline != null)
            {
                if (i == selectedChoiceIndex)
                {
                    if (i == correctAnswerIndex)
                    {
                        outline.effectColor = Color.green; // Correct answer -> Green outline
                    }
                    else
                    {
                        outline.effectColor = Color.red; // Wrong answer -> Red outline
                    }
                }
                else if (i == correctAnswerIndex)
                {
                    outline.effectColor = Color.green; // Ensure correct answer is green

                }
                else
                {
                    outline.effectColor = new Color(0, 0, 0, 0); // Hide outlines for unselected
                }
            }
        }

        return selectedChoiceIndex == correctAnswerIndex;
    }


    public void ResetDalogBox()
    {
        answerSelected = false;
        dialogText.text = "";
        EnableActionSelector(false);
        EnableChoiceSelector(false);
    }
    
}
