using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DetectiveController : MonoBehaviour, Interactable
{
    private bool isInteracting = false;
    [SerializeField] private GameObject InteractPrompt;
    public int level;
    public int caseID;
    MurderCase murderCase;
    public List<string> options = new List<string>(); //multiple choice of answers for murder 
    public int correctOption; //index in options thats correct
    public string dialogGiven;
    string dectordoc = "";

    public void Interact(Transform initiator)
    {
        murderCase = MurderMystery.Instance.murders[level].cases[caseID];

        if(murderCase.type == CaseType.LivePatient)
        {
            dectordoc = "Doctor:";
        }
        else
        {
            dectordoc = "Detective:";
        }
        if (!isInteracting)
        {   if(murderCase.type == CaseType.Murder)
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
            else if(murderCase.type == CaseType.LivePatient)
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
                //check if seen all info from patient
                if (murderCase.patient.responsesSeen.Contains(false))
                {
                    StartCoroutine(NotCheckedAllEvidenceMessage());
                    return;
                }
                StartCoroutine(ShowSolvingChoices());
                return;
            }
            
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
        for(int i = 0; i < options.Count; i++)
        {
            choices.Add(options[i]);
        }
        choices.Add("I need more time");

        // Show dialog with choices using ShowDialogText
        yield return DialogManager.Instance.ShowDialogText(
            dialogGiven,
            waitForInput: false,
            autoClose: false,
            choices: choices,
            onChoiceSelected: async (choiceIndex) =>
            { //these options can be expanded upon once we know what else is wanted for the win/lose condition
                if (choiceIndex == correctOption) //correct
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
            dectordoc + " I think that makes a lot of sense.",
            waitForInput: true,
            autoClose: false
        ));

        yield return StartCoroutine(DialogManager.Instance.ShowDialogText(
            "You completed the Murder Mystery Challenge!",
            waitForInput: true,
            autoClose: false
        ));

        //show unique dialogue when you complete the last murder mystery level
        if((murderCase.level + 1) >= 5)
        {
            yield return StartCoroutine(DialogManager.Instance.ShowDialogText(
                "You have completed all of our challenges for you! Congratulations!",
                waitForInput: true,
                autoClose: false
            ));

            yield return StartCoroutine(DialogManager.Instance.ShowDialogText(
                "Dr. Shepard is thrilled to see how far you have come! You should be proud of yourself.",
                waitForInput: true,
                autoClose: false
            ));

            yield return StartCoroutine(DialogManager.Instance.ShowDialogText(
                "You unlocked the bonus Organ Levels!",
                waitForInput: true,
                autoClose: true
            ));
        }
        else
        {
            yield return StartCoroutine(DialogManager.Instance.ShowDialogText(
                "You unlocked Level " + (murderCase.level + 1) + "!",
                waitForInput: true,
                autoClose: true
            ));
        }


        LevelManager.Instance.UnlockNextLevel();

        //AsyncOperation operation = SceneManager.LoadSceneAsync(7);
        PlayerPrefs.SetString("SpawnPointID", MurderMystery.Instance.GetSpawnpointFromLevel());

        AsyncOperation operation = LevelManager.Instance.LoadLevel(0); // load hub
        yield return new WaitUntil(() => operation.isDone);

        yield return new WaitForSeconds(0.1f); // Delay for scene initialization
    }

    private IEnumerator NotCheckedAllEvidenceMessage()
    {
        yield return StartCoroutine(DialogManager.Instance.ShowDialogText(
            dectordoc + " You haven't looked at all of the evidence, come back when you have.",
            waitForInput: true,
            autoClose: true
        ));
    }

    private IEnumerator ShowIncorrectMessage()
    {
        yield return StartCoroutine(DialogManager.Instance.ShowDialogText(
            dectordoc + " I don't think thats correct...",
            waitForInput: true,
            autoClose: false
        ));
        yield return StartCoroutine(DialogManager.Instance.ShowDialogText(
            "You failed the Murder Mystery Challenge :(",
            waitForInput: true,
            autoClose: true
        ));

        //AsyncOperation operation = SceneManager.LoadSceneAsync(7);
        PlayerPrefs.SetString("SpawnPointID", MurderMystery.Instance.GetSpawnpointFromLevel());

        AsyncOperation operation = LevelManager.Instance.LoadLevel(0); // load hub
        yield return new WaitUntil(() => operation.isDone);

        yield return new WaitForSeconds(0.1f); // Delay for scene initialization
    }

    private IEnumerator ShowLeaveMessage()
    {
        yield return StartCoroutine(DialogManager.Instance.ShowDialogText(
            dectordoc + " Let me know if you have a guess.",
            waitForInput: true,
            autoClose: true
        ));

    }
}
