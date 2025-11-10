using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System;
using System.Timers;

public enum BattleState { START, PLAYERACTION, PLAYERANSWER, END}
public class BattleSystem : MonoBehaviour
{

    // Manager for the battle system
    // Handles flow through entire battle and calls to external battle components, e.g. boss manager

    [SerializeField] private MapArea mapData;
    [SerializeField] private QuestionSection questionSection;
    [SerializeField] private DialogBox dialogBox;
    [SerializeField] private QuestionUnit questionUnit;
    [SerializeField] private HudController hudController;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private float timeLimit = 15f;

    public event Action<bool> OnBattleOver;

    BattleState state;
    //int currentAction;  // store user selected action
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

        if (!mapData.HasQuestions())
        {
            Debug.Log("No questions in the database");
            OnBattleOver(false);
            return;
        }

        this.question = mapData.GetRandomQuestion();
        Debug.Log(this.question.question);
        Debug.Log(this.question.options);
        //currentAction = 0;
        currentAnswer = 0;      
        dialogBox.ResetDalogBox();
        //hudController.TurnHudOff();
        hudController.EnteringBattle();
        StartCoroutine(SetupBattle());
    }

    public void BossBattle(int maxQuestions)
    {
        isBossBattle = true;
        maxBossQuestions = maxQuestions;
        this.state = BattleState.START;

        if (!mapData.HasQuestions())
        {
            Debug.Log("No questions in the database -- Boss Battle");
            OnBattleOver(false);
            return;
        }

        // Rework question selection once boss questions are implemented
        this.question = mapData.GetRandomQuestion();
        Debug.Log(this.question.question);
        Debug.Log(this.question.options);
        //currentAction = 0;
        currentAnswer = 0;
        dialogBox.ResetDalogBox();
        //hudController.TurnHudOff();
        hudController.EnteringBattle();
        StartCoroutine(SetupBattle());
    }

    // Update is called once per frame
    // filling the question and answer texts
    public IEnumerator SetupBattle()
    {
        shuffleAnswersList = (Option[])question.options.ToArray().Clone();
        shuffleAnswersIndex = question.answerIndex;
        ShuffleAnswers(shuffleAnswersList, ref shuffleAnswersIndex);
        StartCoroutine(questionSection.TypeQuestion(question, mapData));
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

        Coroutine currentDialog = null;

        if (!isBossBattle)
        {
            currentDialog = StartCoroutine(dialogBox.TypeDialog("A wild question appeared!"));
            yield return WaitForSpaceOrComplete(currentDialog, 2.0f); // Wait max 1.5 sec or until space
            // yield return StartCoroutine(dialogBox.TypeDialog("A wild question appeared!"));
        }
        else if (currentBossQuestion == 0)
        {
            currentDialog = StartCoroutine(dialogBox.TypeDialog("Time for the test!"));
            yield return WaitForSpaceOrComplete(currentDialog, 2.0f);
            // yield return StartCoroutine(dialogBox.TypeDialog("Time for the test!"));
        }
        else
        {
            currentDialog = StartCoroutine(dialogBox.TypeDialog("Next question!"));
            yield return WaitForSpaceOrComplete(currentDialog, 1.5f);
            // yield return StartCoroutine(dialogBox.TypeDialog("Next question!"));
        }

        currentDialog = StartCoroutine(dialogBox.TypeDialog("Pick the choice!"));
        yield return WaitForSpaceOrComplete(currentDialog, 1f); // Wait max 1 sec or until space

        //yield return new WaitForSeconds(1f);

        //StartCoroutine(dialogBox.TypeDialog("Pick the choice!"));
        //yield return new WaitForSeconds(1f);
        state = BattleState.PLAYERANSWER;
    }


    private IEnumerator WaitForSpaceOrComplete(Coroutine typingCoroutine, float maxWaitTime)
    {
        float elapsed = 0;
        bool skipped = false;

        while (elapsed < maxWaitTime && !skipped)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                skipped = true;
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                    dialogBox.ForceCompleteText(); // Implement this in DialogBox
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
    }


    public void SetMapData(MapArea newMapData)
    {
        this.mapData = newMapData;
    }

    public void SetHudController(HudController newHudController)
    {
        this.hudController = newHudController;
    }

    public void HandleUpdate()
    {
        if (state == BattleState.START)
        {
            dialogBox.EnableDialogText(true);
            dialogBox.EnableOptionSelector(false);
        }
        else if (state == BattleState.PLAYERANSWER)
        {
            dialogBox.EnableDialogText(false);
            dialogBox.EnableOptionSelector(true);
            HandleAnswer();

            // Start the rival timer
            if (rivalTimer == null)
            {
                rivalTimer = StartCoroutine(BattleTimer(timeLimit));
            }
        }
        else if (state == BattleState.END)
        {
            dialogBox.EnableDialogText(true);
            dialogBox.EnableOptionSelector(false);
        }

    }

    void HandleAnswer()
    {
        bool hasImageAnswers = dialogBox.currentOptions == DialogBox.AnswersType.Image;
        int maxAnswers = question.options.Count;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) // Move Right
        {
            if (currentAnswer < maxAnswers - 1)
                ++currentAnswer;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) // Move Left
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

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) && !dialogBox.GetAnswerSelected())
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
        if(dialogBox.GetAnswerSelected())
        {
            return; // Prevents multiple clicks on the answer button
        }
        else
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
        
    }
    IEnumerator EndBattle(bool answerCorrect)
    {
        yield return new WaitForSeconds(1.5f);
        state = BattleState.END;
        Debug.Log("answerCorrect: " + answerCorrect);
        dialogBox.EnableDialogText(true);

        int previousScore = ScoreManager.Instance.GetScoreCount();


        if (answerCorrect)
        {
            ScoreManager.Instance.AddScore(true); // Increment score
            int pointsEarned = ScoreManager.Instance.GetScoreCount() - previousScore;
            mapData.CorrectAnswer(1); // Track question streak
            string rewardText;
            if (isBossBattle)
            {
                rewardText = $"Correct! Rewards: {pointsEarned} points";
                bossQuestionsRight += 1;
            }
            else
            {
                rewardText = $"Correct! Rewards: +1 coin, +{pointsEarned} points";
                CoinManager.Instance.AddCoin(1); // Add a coin
            }


            yield return StartCoroutine(dialogBox.TypeDialog(rewardText));
        }
        else
        {
            mapData.CorrectAnswer(0);
            yield return StartCoroutine(dialogBox.TypeDialog("Incorrect!"));
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
                    bossQuestionsRight = 0;
                    currentBossQuestion = 0;
                    yield return StartCoroutine(dialogBox.TypeDialog("You got them all right! You win!"));

                    GameController.Instance.MarkBossDefeated();

                    LevelManager.Instance.UnlockNextLevel();

                    dialogBox.ResetDalogBox();
                    if (levelCompletePanel != null)
                    {
                        ScoreManager.Instance.AddScore(false, 10000);
                        TimerManager.Instance.StopTimer();
                        levelCompletePanel.SetActive(true);
                        yield return new WaitForSeconds(3f); 
                        levelCompletePanel.SetActive(false);
                    }

                    OnBattleOver(answerCorrect);
                    //hudController.TurnHudOn();
                    hudController.ExitingBattle();
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
        //hudController.TurnHudOn();
        hudController.ExitingBattle();
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

    private Coroutine rivalTimer;
    private IEnumerator BattleTimer(float duration)
    {
        float elaspedTime = 0f;
        while (elaspedTime < duration && state == BattleState.PLAYERANSWER)
        {
            elaspedTime += Time.deltaTime;
            yield return null;
        }

        // If the player fails to make an answer when the timer reaches 0
        if (state == BattleState.PLAYERANSWER)
        {
            Debug.Log("Times up, you lose!");
            StartCoroutine(EndBattle(false));  // Auto lose
        }

        rivalTimer = null;  // Reset for next battle
    }
}