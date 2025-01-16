using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System;

public enum BattleState { START, PLAYERACTION, PLAYERANSWER, END}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] private QuestionBase question;
    [SerializeField] private QuestionSection questionSection;
    [SerializeField] private DialogBox dialogBox;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentAnswer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartBattle()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    // Update is called once per frame
    public IEnumerator SetupBattle()
    {
        StartCoroutine(questionSection.TypeQuestion(question));
        dialogBox.SetAnswerTexts(question.Answers);
        yield return StartCoroutine(dialogBox.TypeDialog("A wild question appeared!"));
        yield return new WaitForSeconds(1f);
        
        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PLAYERACTION;
        StartCoroutine(dialogBox.TypeDialog("Choose an action!"));
        dialogBox.EnableActionSelector(true);
    }

    public void HandleUpdate()
    {
        if (state == BattleState.START)
        {
            dialogBox.EnableDialogText(true);
            dialogBox.EnableActionSelector(false);
            dialogBox.EnableChoiceSelector(false);
        }
        
        else if (state == BattleState.PLAYERACTION)
        {
            HandleAction();
        }
        else if (state == BattleState.PLAYERANSWER)
        {
            dialogBox.EnableActionSelector(false);
            dialogBox.EnableDialogText(false);
            dialogBox.EnableChoiceSelector(true);
            HandleAnswer();
        }
        else if (state == BattleState.END)
        {
            dialogBox.EnableDialogText(true);
            dialogBox.EnableActionSelector(false);
            dialogBox.EnableChoiceSelector(false);
        }

    }

    void HandleAction()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (currentAction < 1)
                ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentAction > 0)
                --currentAction;
        }
        
       if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                // Answer the question
                state = BattleState.PLAYERANSWER;
            }
            else if (currentAction == 1)
            {
                // Run
            }
        }
        dialogBox.UpdateActionSelection(currentAction);

    }

    void HandleAnswer()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (currentAnswer < question.Answers.Length - 1)
                ++currentAnswer;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentAnswer > 0)
                --currentAnswer;
        }
       
       if (Input.GetKeyDown(KeyCode.S))
       {
              if (currentAnswer < question.Answers.Length - 2)
                currentAnswer += 3;
         }
         else if (Input.GetKeyDown(KeyCode.W))
         {
              if (currentAnswer > 2)
                currentAnswer -= 3;
       }

       dialogBox.UpdateChoiceSelection(currentAnswer);

       if (Input.GetKeyDown(KeyCode.Z))
        {
            state = BattleState.END;
            Debug.Log("Current action: " + currentAnswer);
            StartCoroutine(EndBattle(currentAnswer == question.CorrectAnswerIndex));
        }
        dialogBox.UpdateActionSelection(currentAnswer);
    }
    IEnumerator EndBattle(bool answerCorrect)
    {
        Debug.Log("answerCorrect: " + answerCorrect);
            if (answerCorrect){
                dialogBox.EnableDialogText(true);   
                yield return StartCoroutine(dialogBox.TypeDialog("Correct!"));
            }
            else{
                dialogBox.EnableDialogText(true); 
                yield return StartCoroutine(dialogBox.TypeDialog("Incorrect!"));
            }
        yield return new WaitForSeconds(2.5f);
        OnBattleOver(answerCorrect);
    }                                                                                                                                                                                                                            

}
