using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss: MonoBehaviour, Interactable
{
    // Currently useless class
    // Will be expanded to initiate a boss battle
    public void Interact()
    {
        StartCoroutine(Quiz());
    }

    public void ShowPrompt()
    {
        BossController.i.ShowPrompt();
    }

    public void HidePrompt()
    {
        BossController.i.HidePrompt();
    }

    public IEnumerator Quiz()
    {
        yield return BossController.i.StartQuiz();
    }
    
}
