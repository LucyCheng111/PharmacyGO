using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DetectiveController : MonoBehaviour, Interactable
{
    private bool isInteracting = false;
    [SerializeField] private GameObject InteractPrompt;
    MurderCase murderCase;
    public void Interact(Transform initiator)
    {
        murderCase = MurderMystery.Instance.murders[MurderMystery.Instance.currentLevel].cases[MurderMystery.Instance.currentCase];
        if (!isInteracting)
        {
            //first check if all evidence seen
            for(int i = 0; i < murderCase.evidenceObjects.Count; i++)
            {
                if(murderCase.evidenceObjects[i].GetComponent<NPCController>().seen == false)
                {
                    StartCoroutine(NotCheckedAllEvidenceMessage());
                    return;
                }
            }
            //seen all evidence
            StartCoroutine(ShowSolvingChoices());
        }
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

    private IEnumerator ShowSolvingChoices()
    {

        // Create choices for shutdown confirmation

        List<string> choices = new List<string>();
        for(int i = 0; i < murderCase.options.Count; i++)
        {
            choices.Add(murderCase.options[i]);
        }
        choices.Add("I need more time");

        // Show dialog with choices using ShowDialogText
        yield return DialogManager.Instance.ShowDialogText(
            "What do you think happened here?\n",
            waitForInput: false,
            autoClose: false,
            choices: choices,
            onChoiceSelected: async (choiceIndex) =>
            { //these options can be expanded upon once we know what else is wanted for the win/lose condition
                if (choiceIndex == murderCase.correctOption) //correct
                {
                    StartCoroutine(ShowCorrectMessage());
                }
                else if(choiceIndex == choices.Count - 1) //last option is leave
                {
                    StartCoroutine(ShowLeaveMessage());
                }
                else
                {
                    StartCoroutine(ShowIncorrectMessage());
                }
            }
        );

        
    }

    private IEnumerator ShowCorrectMessage()
    {
        yield return StartCoroutine(DialogManager.Instance.ShowDialogText(
            "Detective: I think that makes a lot of sense.",
            waitForInput: true,
            autoClose: false
        ));

        yield return StartCoroutine(DialogManager.Instance.ShowDialogText(
            "You completed the Murder Mystery Challenge!",
            waitForInput: true,
            autoClose: true
        ));
    }

    private IEnumerator NotCheckedAllEvidenceMessage()
    {
        yield return StartCoroutine(DialogManager.Instance.ShowDialogText(
            "Detective: You haven't looked at all of the evidence, come back when you have.",
            waitForInput: true,
            autoClose: true
        ));
    }

    private IEnumerator ShowIncorrectMessage()
    {
        yield return StartCoroutine(DialogManager.Instance.ShowDialogText(
            "Detective: I don't think thats correct...",
            waitForInput: true,
            autoClose: true
        ));
    }

    private IEnumerator ShowLeaveMessage()
    {
        yield return StartCoroutine(DialogManager.Instance.ShowDialogText(
            "Detective: Let me know if you have a guess.",
            waitForInput: true,
            autoClose: true
        ));

    }
}
