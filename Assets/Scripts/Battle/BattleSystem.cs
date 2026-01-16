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

    // New 2026 
    // === AI RIVAL ===
    private bool aiEnabled;
    private List<int> aiTriedAnswers = new List<int>(); // Track which answers AI already tried
    //private bool aiHasAnswered;
    //private bool aiAnswerCorrect;
    private Coroutine aiAnswerRoutine;
    private bool battleLocked = false;
    private bool timedOut = false;

    // Player timing
    private float questionStartTime;
    private List<float> recentAnswerTimes = new List<float>();

    private AnswerSource battleAnswerSource;
    private enum AnswerSource
    {
        Player,
        AI,
        Timeout
    }
    // New 2026

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartBattle()
    {

        // New 2026
        battleLocked = false;
        aiEnabled = AiRival.Instance != null && AiRival.Instance.IsActive;
        //aiHasAnswered = false;
        aiTriedAnswers.Clear();
        timedOut = false;
        // New 2026

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
        // New 2026
        battleLocked = false;
        aiEnabled = AiRival.Instance != null && AiRival.Instance.IsActive;
        //aiHasAnswered = false;
        aiTriedAnswers.Clear();
        timedOut = false;
        // New 2026

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

        // new 2026
        // AI clock starts

        state = BattleState.PLAYERANSWER;
        questionStartTime = Time.time;
        

        if (aiEnabled && !isBossBattle)
        {
            Debug.Log("Starting AI Answer Routine - AI is enabled!");
            aiAnswerRoutine = StartCoroutine(AIAnswerRoutine());
        }
        else
        {
            Debug.Log($"AI NOT starting - aiEnabled: {aiEnabled}, isBossBattle: {isBossBattle}");
        }
        // new 2026

        //StartCoroutine(dialogBox.TypeDialog("Pick the choice!"));
        //yield return new WaitForSeconds(1f);
        //state = BattleState.PLAYERANSWER;

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

            // Start the rival timer - For timeout usage
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

        // new2026 - stop accepting answer
        if (state != BattleState.PLAYERANSWER || battleLocked)
            return;
        // new2026

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
            // new 2026
            float playerAnswerTime = Time.time - questionStartTime;
            RecordPlayerAnswerTime(playerAnswerTime);

            battleAnswerSource = AnswerSource.Player; // player answered

            if (aiAnswerRoutine != null)
            {
                StopCoroutine(aiAnswerRoutine);
            }
            // new 2026

            bool isCorrect;
            isCorrect = dialogBox.DisplayAnswer(currentAnswer, shuffleAnswersIndex);

            // new 2026
            StartCoroutine(EndBattle(isCorrect, AnswerSource.Player));
        }

        if (!hasImageAnswers)
        {
            dialogBox.UpdateActionSelection(currentAnswer);
        }
    }
    public void OnClickAnswerButton(int answerIndex)
    {
        // new2026
        if (battleLocked || dialogBox.GetAnswerSelected())
            return; // Prevent answering if AI already finished
        // new 2026

        if (dialogBox.GetAnswerSelected())
        {
            return; // Prevents multiple clicks on the answer button
        }
        else
        {
            Debug.Log("Answer button clicked: " + answerIndex);
            currentAnswer = answerIndex;
            dialogBox.UpdateChoiceSelection(currentAnswer);
            bool hasImageAnswers = dialogBox.currentOptions == DialogBox.AnswersType.Image;

            // new 2026
            float playerAnswerTime = Time.time - questionStartTime;
            RecordPlayerAnswerTime(playerAnswerTime);

            battleAnswerSource = AnswerSource.Player; // player answered

            if (aiAnswerRoutine != null)
            {
                StopCoroutine(aiAnswerRoutine);
            }

            // new 2026

            bool isCorrect;
            isCorrect = dialogBox.DisplayAnswer(currentAnswer, shuffleAnswersIndex);
            // new 2026
            StartCoroutine(EndBattle(isCorrect, AnswerSource.Player));

            if (!hasImageAnswers)
            {
                dialogBox.UpdateActionSelection(currentAnswer);
            }
        }
        
    }
    IEnumerator EndBattle(bool answerCorrect, AnswerSource source)
    {
        // new 2026
        bool playerWon =
        (source == AnswerSource.Player && answerCorrect);

        // Clear any previous dialog - to avoid "pick the choice appear when AI wins" 
        if (source == AnswerSource.AI || source == AnswerSource.Timeout)
        {
            dialogBox.ResetDalogBox();
        }
        // new 2026

        yield return new WaitForSeconds(1.5f);
        state = BattleState.END;
        Debug.Log("answerCorrect: " + answerCorrect);

        

        dialogBox.EnableDialogText(true);

        int previousScore = ScoreManager.Instance.GetScoreCount();


        //if (answerCorrect)
        //{
        //    ScoreManager.Instance.AddScore(true); // Increment score
        //    int pointsEarned = ScoreManager.Instance.GetScoreCount() - previousScore;
        //    mapData.CorrectAnswer(1); // Track question streak
        //    string rewardText;
        //    if (isBossBattle)
        //    {
        //        rewardText = $"Correct! Rewards: {pointsEarned} points";
        //        bossQuestionsRight += 1;
        //    }
        //    else
        //    {
        //        rewardText = $"Correct! Rewards: +1 coin, +{pointsEarned} points";
        //        CoinManager.Instance.AddCoin(1); // Add a coin
        //    }


        //    yield return StartCoroutine(dialogBox.TypeDialog(rewardText));
        //}
        //else
        //{
        //    mapData.CorrectAnswer(0);
        //    yield return StartCoroutine(dialogBox.TypeDialog("Incorrect!"));
        //}

        // new 2026
        if (playerWon)
        {
            // Player won
            ScoreManager.Instance.AddScore(true);
            int pointsEarned = ScoreManager.Instance.GetScoreCount() - previousScore;
            mapData.CorrectAnswer(1);

            string rewardText;
            if (isBossBattle)
            {
                rewardText = $"You win! Rewards: {pointsEarned} points";
                bossQuestionsRight += 1;
            }
            else
            {
                rewardText = $"You win! Rewards: +1 coin, +{pointsEarned} points";
                CoinManager.Instance.AddCoin(1);
            }

            yield return StartCoroutine(dialogBox.TypeDialog(rewardText));
        }
        else
        {
            mapData.CorrectAnswer(0);

            if (source == AnswerSource.AI)
            {
                yield return StartCoroutine(dialogBox.TypeDialog("Your Rival answered first and won!"));
            }
            else if (source == AnswerSource.Timeout)
            {
                yield return StartCoroutine(dialogBox.TypeDialog("Time's up, you failed!"));
            }
            else
            {
                yield return StartCoroutine(dialogBox.TypeDialog("Incorrect!"));
            }
        }
        // new 2026

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
            timedOut = true;
            // Stop AI routine if it's still running
            if (aiAnswerRoutine != null)
            {
                StopCoroutine(aiAnswerRoutine);
            }
            StartCoroutine(EndBattle(false, AnswerSource.Timeout));  // Auto lose
        }

        rivalTimer = null;  // Reset for next battle
    }

    // new 2026
    // ================= AI RIVAL LOGIC =================

    //private IEnumerator AIAnswerRoutine()
    //{
    //    float playerAvg = GetAveragePlayerAnswerTime();

    //    // Rival difficulty tuning
    //    float difficultyModifier = UnityEngine.Random.Range(0.85f, 1.1f);
    //    float aiDelay = Mathf.Clamp(playerAvg * difficultyModifier + UnityEngine.Random.Range(0.2f, 0.8f), 5, timeLimit - 0.5f);

    //    yield return new WaitForSeconds(aiDelay);

    //    if (state != BattleState.PLAYERANSWER)
    //        yield break;

    //    battleAnswerSource = AnswerSource.AI;   // Ai answered

    //    aiHasAnswered = true;

    //    // Accuracy model
    //    float accuracy = 0.75f; // tune per rival / difficulty
    //    aiAnswerCorrect = UnityEngine.Random.value < accuracy;

    //    Debug.Log($"AI answered in {aiDelay:F2}s | Correct: {aiAnswerCorrect}");

    //    ResolveBattleIfNeeded();
    //}

    private IEnumerator AIAnswerRoutine()
    {
        Debug.Log("AI Answer Routine Started!");

        while (state == BattleState.PLAYERANSWER && !battleLocked)
        {
            float playerAvg = GetAveragePlayerAnswerTime();

            // Rival difficulty tuning - time between attempts
            // Make AI faster: use 60% of player average time, with some randomness
            float difficultyModifier = UnityEngine.Random.Range(0.5f, 0.8f);
            float aiDelay = Mathf.Clamp(playerAvg * difficultyModifier, 1.5f, 8f);

            Debug.Log($"AI waiting {aiDelay:F2} seconds before answering... (playerAvg: {playerAvg:F2})");
            yield return new WaitForSeconds(aiDelay);

            if (state != BattleState.PLAYERANSWER || battleLocked)
            {
                Debug.Log("AI stopped - battle state changed or locked");
                yield break;
            }

            // Choose an answer AI hasn't tried yet
            int aiChosenAnswer = ChooseAIAnswer();

            if (aiChosenAnswer == -1)
            {
                Debug.Log("AI ran out of answers to try");
                yield break;
            }

            Debug.Log($"AI chose answer index: {aiChosenAnswer}");

            // Show AI's choice with orange outline
            dialogBox.ShowAIChoice(aiChosenAnswer);

            yield return new WaitForSeconds(2f); // Brief pause to show selection

            // Check if AI got it right
            bool aiCorrect = (aiChosenAnswer == shuffleAnswersIndex);

            if (aiCorrect)
            {
                // AI got it right! AI wins
                Debug.Log($"AI answered correctly: {aiChosenAnswer}");
                battleAnswerSource = AnswerSource.AI;
                ResolveBattleWin();
                yield break;
            }
            else
            {
                // AI got it wrong, clear and try again
                Debug.Log($"AI answered wrong: {aiChosenAnswer}, correct was {shuffleAnswersIndex}, will try again");
                dialogBox.ClearAIChoice();
                aiTriedAnswers.Add(aiChosenAnswer);
                // Loop continues to try another answer
            }
        }
    }


    private int ChooseAIAnswer()
    {
        // Get list of answers AI hasn't tried yet
        List<int> availableAnswers = new List<int>();
        for (int i = 0; i < question.options.Count; i++)
        {
            if (!aiTriedAnswers.Contains(i))
            {
                availableAnswers.Add(i);
            }
        }

        if (availableAnswers.Count == 0)
            return -1; // No more answers to try

        // Randomly choose from available answers
        return availableAnswers[UnityEngine.Random.Range(0, availableAnswers.Count)];
    }

    // Ai helper function, get player answering info
    private void RecordPlayerAnswerTime(float time)
    {
        recentAnswerTimes.Add(time);
        if (recentAnswerTimes.Count > 5)
            recentAnswerTimes.RemoveAt(0);
    }

    private float GetAveragePlayerAnswerTime()
    {
        if (recentAnswerTimes.Count == 0)
            return 5f; // Start with 5 seconds default instead of 9

        float sum = 0;
        foreach (float t in recentAnswerTimes)
            sum += t;

        return sum / recentAnswerTimes.Count;
    }

    private void ResolveBattleWin()
    {
        if (battleLocked) return; // Already resolved
        battleLocked = true;      // Lock immediately

        // AI answered correctly and wins
        StopPlayerInput();
        StartCoroutine(EndBattle(true, AnswerSource.AI));
    }

    private void StopPlayerInput()
    {
        state = BattleState.END;
        dialogBox.EnableOptionSelector(false);
    }

    // new 2026


}

