using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour, Interactable
{

    // Manages the boss on the game map
    // Starts the quiz if the player is ready

    [SerializeField] private int maxQuestions;
    [SerializeField] private GameObject InteractPrompt;

    public static BossController i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public void Interact()
    {
        StartCoroutine(StartQuiz());
    }

    public void ShowPrompt()
    {
        InteractPrompt.SetActive(true);
    }

    public void HidePrompt()
    {
        InteractPrompt.SetActive(false);
    }

    public IEnumerator StartQuiz()
    {

        int selectedChoice = 0;
        yield return DialogManager.Instance.ShowDialogText("Are you ready to take the quiz?",
            waitForInput: false,
            choices: new List<string>() { "Yes", "No" },
            onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        if (selectedChoice == 0)
        {
            //Yes
            if (CoinManager.Instance.GetCoinCount() >= 10)
            {
                CoinManager.Instance.RemoveCoin(10);
                // TODO: Send a call to Boss.cs to initiate a boss battle
                GameController.Instance.StartBattle(true, maxQuestions);
            }
            else
            {
                StartCoroutine(DialogManager.Instance.ShowDialogText("You don't have enough coins! It takes 10 to try."));
            }
        }
        else
        {
            //No
            StartCoroutine(DialogManager.Instance.ShowDialogText("Come back when you're ready to take the test."));
        }
    }
}
