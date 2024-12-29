using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{

    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int letterPerSecond;
    
    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    private Dialog dialog; 
    private int currentLine = 0; 
    private bool isTyping = false; 

    public static DialogManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;

    }

    public IEnumerator ShowDialog(Dialog dialog)
    {
        
        yield return new WaitForEndOfFrame(); // Avoid Race Condition


        /* The DialogManager triggers the event when the dialog box is shown
         * By using OnShowDialog?.Invoke();
         * Other scripts (like a PlayerController) can listen to this event to pause player movement or take some other action
         * using ?. to check if it's null
         * -- Aic
         */
        OnShowDialog?.Invoke();

        this.dialog = dialog;
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }


    /* Updates the dialog system. Should be called in Update() in GameControl.cs
     * 
     * 
     * 
     */
    public void HandleUpdate()
    {
        if(Input.GetKeyUp(KeyCode.Z) && !isTyping) 
        {
            ++currentLine;
            if(currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            else
            {
                currentLine = 0;
                dialogBox.SetActive(false);
                OnCloseDialog?.Invoke();
            }
        }
    }


    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }
        isTyping = false;
    }


}
