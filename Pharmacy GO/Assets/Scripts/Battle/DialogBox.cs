using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
public class DialogBox : MonoBehaviour
{
    public int letterPerSecond = 30;
    public bool answerSelected = false;
    [SerializeField] private Color highlightedColor;
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private GameObject actionSelector;
    [SerializeField] private GameObject choiceSelector;
    [SerializeField] List<TMP_Text> actionTexts;
    [SerializeField] List<TMP_Text> choices;
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

    public void ResetDalogBox()
    {
        answerSelected = false;
        dialogText.text = "";
        EnableActionSelector(false);
        EnableChoiceSelector(false);
    }
    
}
