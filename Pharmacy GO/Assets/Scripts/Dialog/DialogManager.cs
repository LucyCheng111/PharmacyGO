using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{

    [SerializeField] GameObject dialogBox;
    [SerializeField] ChoiceBox choiceBox;
    [SerializeField] Text dialogText;
    [SerializeField] int letterPerSecond;
    
    public event Action OnShowDialog;
    public event Action OnDialogFinished;

    public static DialogManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;

    }


    public bool IsShowing { get; private set; }

    
    // For direct strings
    public IEnumerator ShowDialogText(string text, bool waitForInput=true, bool autoClose=true,
        List<string> choices=null, Action<int> onChoiceSelected=null)
    {
        OnShowDialog?.Invoke();
        IsShowing = true;
        dialogBox.SetActive(true);

        yield return TypeDialog(text);
        if (waitForInput)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }

        if (choices != null && choices.Count > 1)
        {
            Debug.Log("Choices:" + choices[0]);
            Debug.Log("onChoiceSelected: " + onChoiceSelected);
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        if (autoClose)
        {
            CloseDialog();
        }

        OnDialogFinished?.Invoke();
        /*
        dialogBox.SetActive(false);
        IsShowing = false;
        */
    }

    // Standardized Close dialog
    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
    }


    // For Dialog objects
    public IEnumerator ShowDialog(Dialog dialog, List<string> choices=null,
        Action<int> onChoiceSelected=null)
    {
        
        yield return new WaitForEndOfFrame(); // Avoid Race Condition


        /* The DialogManager triggers the event when the dialog box is shown
         * By using OnShowDialog?.Invoke();
         * Other scripts (like a PlayerController) can listen to this event to pause player movement or take some other action
         * using ?. to check if it's null
         * -- Aic
         */

        OnShowDialog?.Invoke();
        IsShowing = true;
        

        dialogBox.SetActive(true);

        foreach(var line in dialog.Lines)
        {
            yield return TypeDialog(line);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }

        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        dialogBox.SetActive(false);
        IsShowing = false;
        OnDialogFinished?.Invoke();
    }


    /* Updates the dialog system. Should be called in Update() in GameControl.cs
     * 
     * 
     * 
     */
    public void HandleUpdate()
    {
        // Handled in each dialog option now
    }


    public IEnumerator TypeDialog(string line)
    {
        // TypeDialog calls now wait until the call returns to before resuming other functions
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }
        
    }


}
