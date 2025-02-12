using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System;

public enum BattleState { START, PLAYERACTION, PLAYERANSWER, END}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] private MapArea mapData;
    [SerializeField] private QuestionSection questionSection;
    [SerializeField] private DialogBox dialogBox;
    [SerializeField] private QuestionUnit questionUnit;
    [SerializeField] private HudController hudController;

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
        this.question = mapData.GetRandomQuestion();
        currentAction = 0;
        currentAnswer = 0;      
        dialogBox.ResetDalogBox();
        hudController.TurnHudOff();
        StartCoroutine(SetupBattle());
    }

    // Update is called once per frame
    // filling the question and answer texts
    public IEnumerator SetupBattle()
    {
        shuffleAnswersList = (string[])question.Answers.Clone();
        shuffleAnswersIndex = question.CorrectAnswerIndex;
        ShuffleAnswers(shuffleAnswersList, ref shuffleAnswersIndex);
        StartCoroutine(questionSection.TypeQuestion(question));
        if (shuffleAnswersList != null)
        {
            dialogBox.SetAnswerTexts(shuffleAnswersList);
        }
        else
        {
            dialogBox.SetAnswerTexts(new string[0]); // Pass an empty array if null
        }
        if (question.AnswersImg != null)
        {
            Sprite[] clonedAnswerImages = (Sprite[])question.AnswersImg.Clone();
            dialogBox.SetAnswerImages(clonedAnswerImages);
        }
        else
        {
            dialogBox.SetAnswerImages(null); // Pass null if there are no images
        }
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
            dialogBox.EnableImageChoiceSelector(false);
        }

        else if (state == BattleState.PLAYERACTION)
        {
            HandleAction();
        }
        else if (state == BattleState.PLAYERANSWER)
        {
            dialogBox.EnableActionSelector(false);
            dialogBox.EnableDialogText(false);
            if (question.AnswersImg != null && question.AnswersImg.Length > 0)
            {
                dialogBox.EnableImageChoiceSelector(true);
            }
            else
            {
                dialogBox.EnableChoiceSelector(true);
            }
            HandleAnswer();
        }
        else if (state == BattleState.END)
        {
            dialogBox.EnableDialogText(true);
            dialogBox.EnableActionSelector(false);
            dialogBox.EnableChoiceSelector(false);
            dialogBox.EnableImageChoiceSelector(false);
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
        bool hasImageAnswers = question.AnswersImg != null && question.AnswersImg.Length > 0;
        int maxAnswers = hasImageAnswers ? question.AnswersImg.Length : shuffleAnswersList.Length;

        if (Input.GetKeyDown(KeyCode.D)) // Move Right
        {
            if (currentAnswer < maxAnswers - 1)
                ++currentAnswer;
        }
        else if (Input.GetKeyDown(KeyCode.A)) // Move Left
        {
            if (currentAnswer > 0)
                --currentAnswer;
        }

        if (Input.GetKeyDown(KeyCode.S)) // Move Down
        {
            if (currentAnswer < maxAnswers - 2)
                currentAnswer += 3;
        }
        else if (Input.GetKeyDown(KeyCode.W)) // Move Up
        {
            if (currentAnswer > 2)
                currentAnswer -= 3;
        }

        // Update selection based on answer type
        if (hasImageAnswers)
        {
            dialogBox.UpdateImageChoiceSelection(currentAnswer);
        }
        else
        {
            dialogBox.UpdateChoiceSelection(currentAnswer);
        }

        if (Input.GetKeyDown(KeyCode.Z) && !dialogBox.GetAnswerSelected())
        {
            bool isCorrect;
            if (hasImageAnswers)
            {
                isCorrect = dialogBox.DisplayImageAnswer(currentAnswer, shuffleAnswersIndex);

            }
            else
            {
                isCorrect = dialogBox.DisplayAnswer(currentAnswer, shuffleAnswersIndex);
            }

            StartCoroutine(EndBattle(isCorrect));
        }

        if (!hasImageAnswers)
        {
            dialogBox.UpdateActionSelection(currentAnswer);
        }
    }
    IEnumerator EndBattle(bool answerCorrect)
    {
        yield return new WaitForSeconds(2.5f);
        state = BattleState.END;
        Debug.Log("answerCorrect: " + answerCorrect);
            if (answerCorrect){
                dialogBox.EnableDialogText(true);
                CoinManager.Instance.AddCoin(1); // Add a coin
                ScoreManager.Instance.AddScore(mapData.GetCorrectStreak(), mapData.GetDifficulty()); // Increment score based on streak and difficulty
                mapData.CorrectAnswer(1);
                yield return StartCoroutine(dialogBox.TypeDialog("Correct!"));
            }
            else{
                dialogBox.EnableDialogText(true);
                mapData.CorrectAnswer(0);
                yield return StartCoroutine(dialogBox.TypeDialog("Incorrect!"));
            }
        yield return new WaitForSeconds(2.5f);
        dialogBox.ResetDalogBox();
        hudController.TurnHudOn();
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
