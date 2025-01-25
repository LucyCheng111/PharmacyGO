using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System;

public enum BattleState { START, PLAYERACTION, PLAYERANSWER, END}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] private MapArea questionSelector;
    [SerializeField] private QuestionSection questionSection;
    [SerializeField] private DialogBox dialogBox;
    [SerializeField] private QuestionUnit questionUnit;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;  // store user selected action
    int currentAnswer;  // store user selected answer
    IEnumerator chooseAction;
    QuestionBase question;

    private string[] shuffleAnswersList;
    private int shuffleAnswersIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartBattle()
    {

        this.state = BattleState.START;
        this.question = questionSelector.GetRandomQuestion();
        currentAction = 0;
        currentAnswer = 0;      
        dialogBox.ResetDalogBox();
        StartCoroutine(SetupBattle());
    }

    // Update is called once per frame
    // filling the question and answer texts
    public IEnumerator SetupBattle()
    {
        shuffleAnswersList = question.Answers;
        shuffleAnswersIndex = question.CorrectAnswerIndex;
        ShuffleAnswers(shuffleAnswersList, ref shuffleAnswersIndex);
        StartCoroutine(questionSection.TypeQuestion(question));
        dialogBox.SetAnswerTexts(shuffleAnswersList);
        questionUnit.SetImage(question);
        yield return StartCoroutine(dialogBox.TypeDialog("A wild question appeared!"));
        yield return new WaitForSeconds(1f);
        
        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PLAYERACTION;
        chooseAction = dialogBox.TypeDialog("Choose an action!");
        StartCoroutine(chooseAction);
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
            StopCoroutine(chooseAction);
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
            if (currentAnswer < shuffleAnswersList.Length - 1)
                ++currentAnswer;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentAnswer > 0)
                --currentAnswer;
        }
       
       if (Input.GetKeyDown(KeyCode.S))
       {
              if (currentAnswer < shuffleAnswersList.Length - 2)
                currentAnswer += 3;
         }
         else if (Input.GetKeyDown(KeyCode.W))
         {
              if (currentAnswer > 2)
                currentAnswer -= 3;
       }

       dialogBox.UpdateChoiceSelection(currentAnswer);

       if (Input.GetKeyDown(KeyCode.Z) && !dialogBox.GetAnswerSelected())
        {
            bool isCorrect = dialogBox.DisplayAnswer(currentAnswer, shuffleAnswersIndex);
            StartCoroutine(EndBattle(isCorrect));
        }
        dialogBox.UpdateActionSelection(currentAnswer);
    }
    IEnumerator EndBattle(bool answerCorrect)
    {
        yield return new WaitForSeconds(2.5f);
        state = BattleState.END;
        Debug.Log("answerCorrect: " + answerCorrect);
            if (answerCorrect){
                dialogBox.EnableDialogText(true);   
                CoinManager.Instance.AddCoin(1); // Add a coin
                yield return StartCoroutine(dialogBox.TypeDialog("Correct!"));
            }
            else{
                dialogBox.EnableDialogText(true); 
                yield return StartCoroutine(dialogBox.TypeDialog("Incorrect!"));
            }
        yield return new WaitForSeconds(2.5f);
        dialogBox.ResetDalogBox();
        OnBattleOver(answerCorrect);
    }                                                                                                                                                                                                                            

    private void ShuffleAnswers(string[] answerChoices, ref int correctAnswerIndex)
    {
        System.Random rng = new System.Random();
        for (int i = 0; i < answerChoices.Length; i++)
        {
            int randomIndex = rng.Next(i, answerChoices.Length);

            string temp = answerChoices[i];
            answerChoices[i] = answerChoices[randomIndex];
            answerChoices[randomIndex] = temp;

            if (correctAnswerIndex == i)
            {
                correctAnswerIndex = randomIndex;
            }
            else if (correctAnswerIndex == randomIndex)
            {
                correctAnswerIndex = i;
            }
        }

    }
}
