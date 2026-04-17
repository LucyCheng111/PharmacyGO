using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class PatientController : MonoBehaviour, Interactable
{
    public List<Dialog> responses = new List<Dialog>(); //responses to questions
    public List<string> questions = new List<string>(); //list of questions to ask patient
    public List<bool> responsesSeen = new List<bool>();

    public string dialogGiven;

    private bool isInteracting = false;
    [SerializeField] private GameObject InteractPrompt;
    MurderCase murderCase;
    public void Interact(Transform initiator)
    {
        //init responsesSeen
        if(responsesSeen.Count == 0)
        {
            for(int i = 0; i < responses.Count; i++)
            {
                responsesSeen.Add(false);
            }
        }
        murderCase = MurderMystery.Instance.murders[MurderMystery.Instance.currentLevel].cases[MurderMystery.Instance.currentCase];
        if (!isInteracting)
        {
            StartCoroutine(ShowDialogeOptions());
        }
    }
    
    private IEnumerator ShowDialogeOptions()
    {
        List<string> choices = new List<string>();
        for(int i = 0; i < questions.Count; i++)
        {
            choices.Add(questions[i]);
        }
        choices.Add("I'll be right back");
        yield return DialogManager.Instance.ShowDialogText(
            dialogGiven,
            waitForInput: true,
            autoClose: true,
            choices: choices,
            onChoiceSelected: async (choiceIndex) =>
            { 
                if(choiceIndex != questions.Count) // "ill be right back"
                {
                    StartCoroutine(ShowResponse(choiceIndex));   
                }
            }
        );
    }

    

    private IEnumerator ShowResponse(int index)
    {
        responsesSeen[index] = true;
        StartCoroutine( DialogManager.Instance.ShowDialog(responses[index]));
        //StartCoroutine(ShowDialogeOptions());
        yield return null;
    }


    public void ShowPrompt()
    {
        if (InteractPrompt != null)
        {
            InteractPrompt.SetActive(true);
        }
    }

    public void HidePrompt()
    {
        if (InteractPrompt != null)
        {
            InteractPrompt.SetActive(false);
        }
    }


}
