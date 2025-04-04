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
    [SerializeField] private GameObject levelCompletePanel;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;  // store user selected action
    int currentAnswer;  // store user selected answer
    IEnumerator chooseAction;
    Question question;

    bool isBossBattle = false; // if Boss battle, handle differently
    int currentBossQuestion = 0;
    int maxBossQuestions = 1;
    int bossQuestionsRight = 0;


    private Option[] shuffleAnswersList;
    private int shuffleAnswersIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartBattle()
    {
        isBossBattle = false;
        this.state = BattleState.START;
        this.question = mapData.GetRandomQuestion();
        Debug.Log(this.question.question);
        Debug.Log(this.question.options);
        currentAction = 0;
        currentAnswer = 0;      
        dialogBox.ResetDalogBox();
        hudController.TurnHudOff();
        StartCoroutine(SetupBattle());
    }

    public void BossBattle(int maxQuestions)
    {
        isBossBattle = true;
        maxBossQuestions = maxQuestions;
        this.state = BattleState.START;
        // Rework question selection once boss questions are implemented
        this.question = mapData.GetRandomQuestion();
        Debug.Log(this.question.question);
        Debug.Log(this.question.options);
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
        shuffleAnswersList = (Option[])question.options.ToArray().Clone();
        shuffleAnswersIndex = question.answerIndex;
        ShuffleAnswers(shuffleAnswersList, ref shuffleAnswersIndex);
        StartCoroutine(questionSection.TypeQuestion(question));
        if (shuffleAnswersList != null)
        {
            dialogBox.SetAnswers(shuffleAnswersList);
        }
        else
        {
            dialogBox.SetAnswers(new Option[0]); // Pass an empty array if null
        }
        // if (question.AnswersImg != null)
        // {
        //     Sprite[] clonedAnswerImages = (Sprite[])question.AnswersImg.Clone();
        //     dialogBox.SetAnswerImages(clonedAnswerImages);
        // }
        // else
        // {
        //     dialogBox.SetAnswerImages(null); // Pass null if there are no images
        // }
        questionUnit.SetImage(question);
        if (!isBossBattle)
        {
            yield return StartCoroutine(dialogBox.TypeDialog("A wild question appeared!"));
        }
        else if (currentBossQuestion == 0)
        {
            yield return StartCoroutine(dialogBox.TypeDialog("Time for the test!"));
        }
        else
        {
            yield return StartCoroutine(dialogBox.TypeDialog("Next question!"));
        }
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }

    public void SetMapData(MapArea newMapData)
    {
        this.mapData = newMapData;
    }

    public void SetHudController(HudController newHudController)
    {
        this.hudController = newHudController;
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
            dialogBox.EnableOptionSelector(false);
        }

        else if (state == BattleState.PLAYERACTION)
        {
            HandleAction();
        }
        else if (state == BattleState.PLAYERANSWER)
        {
            dialogBox.EnableActionSelector(false);
            dialogBox.EnableDialogText(false);
            dialogBox.EnableOptionSelector(true);
            HandleAnswer();
        }
        else if (state == BattleState.END)
        {
            dialogBox.EnableDialogText(true);
            dialogBox.EnableActionSelector(false);
            dialogBox.EnableOptionSelector(false);
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
    public void OnClickFightButton()
    {
        state = BattleState.PLAYERANSWER;
        dialogBox.UpdateActionSelection(0);
    }

    void HandleAnswer()
    {
        bool hasImageAnswers = dialogBox.currentOptions == DialogBox.AnswersType.Image;
        int maxAnswers = question.options.Count;

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

        // if (Input.GetKeyDown(KeyCode.S)) // Move Down
        // {
        //     if (currentAnswer < maxAnswers - 2)
        //         currentAnswer += 3;
        // }
        // else if (Input.GetKeyDown(KeyCode.W)) // Move Up
        // {
        //     if (currentAnswer > 2)
        //         currentAnswer -= 3;
        // }

        // Update selection based on answer type
        
        dialogBox.UpdateChoiceSelection(currentAnswer);

        if (Input.GetKeyDown(KeyCode.Z) && !dialogBox.GetAnswerSelected())
        {
            bool isCorrect;
            isCorrect = dialogBox.DisplayAnswer(currentAnswer, shuffleAnswersIndex);

            StartCoroutine(EndBattle(isCorrect));
        }

        if (!hasImageAnswers)
        {
            dialogBox.UpdateActionSelection(currentAnswer);
        }
    }
    public void OnClickAnswerButton(int answerIndex)
    {
        Debug.Log("Answer button clicked: " + answerIndex);
        currentAnswer = answerIndex;
        dialogBox.UpdateChoiceSelection(currentAnswer);
        bool hasImageAnswers = dialogBox.currentOptions == DialogBox.AnswersType.Image;
        
        bool isCorrect;
        isCorrect = dialogBox.DisplayAnswer(currentAnswer, shuffleAnswersIndex);
        StartCoroutine(EndBattle(isCorrect));

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
        dialogBox.EnableDialogText(true);
            if (answerCorrect){
                if (isBossBattle)
                {
                    bossQuestionsRight += 1;
                }
                else
                {
                    CoinManager.Instance.AddCoin(1); // Add a coin
                }
                ScoreManager.Instance.AddScore(mapData.GetCorrectStreak(), mapData.GetDifficulty()); // Increment score based on streak and difficulty
                mapData.CorrectAnswer(1); // Track question streak
                yield return StartCoroutine(dialogBox.TypeDialog("Correct!"));
            }
            else{
                mapData.CorrectAnswer(0);
                //Debug.Log("Should be typing incorrect");
                yield return StartCoroutine(dialogBox.TypeDialog("Incorrect!"));
                //Debug.Log("Returned from typing incorrect?");
        }
        if (isBossBattle)
        {
            yield return new WaitForSeconds(2.5f);
            currentBossQuestion += 1;
            if (currentBossQuestion < maxBossQuestions) 
            {
                BossBattle(maxBossQuestions);
                yield break; // Stop the iteration so the battle doesn't end until all questions are done
            }
            else
            {
                if (bossQuestionsRight == maxBossQuestions)
                {
                    yield return StartCoroutine(dialogBox.TypeDialog("You got them all right! You win!"));

                    dialogBox.ResetDalogBox();
                    if (levelCompletePanel != null)
                    {
                        levelCompletePanel.SetActive(true);
                        yield return new WaitForSeconds(3f); 
                        levelCompletePanel.SetActive(false);
                    }

                    OnBattleOver(answerCorrect);
                    hudController.TurnHudOn();
                    yield return null;
                }
                else
                {
                    yield return StartCoroutine(dialogBox.TypeDialog("You missed some questions. Better luck next time!"));
                }
                bossQuestionsRight = 0;
                currentBossQuestion = 0;
            }
        }
        yield return new WaitForSeconds(2.5f);
        dialogBox.ResetDalogBox();
        hudController.TurnHudOn();
        OnBattleOver(answerCorrect);
    }                                                                                                                                                                                                                            

    private void ShuffleAnswers(Option[] answerChoices, ref int correctAnswerIndex)
    {
        System.Random rng = new System.Random();
        for (int i = 0; i < answerChoices.Length; i++)
        {
            int randomIndex = rng.Next(i, answerChoices.Length);

            Option temp = answerChoices[i];
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
