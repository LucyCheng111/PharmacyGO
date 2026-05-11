using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System;
using System.Timers;

public enum BattleState { START, PLAYERACTION, PLAYERANSWER, END}
public class BattleSystem : MonoBehaviour
{
    public static BattleSystem Instance { get; private set; }
    // Manager for the battle system
    // Handles flow through entire battle and calls to external battle components, e.g. boss manager

    [SerializeField] private MapArea mapData;
    [SerializeField] private QuestionSection questionSection;
    [SerializeField] private DialogBox dialogBox;
    [SerializeField] private QuestionUnit questionUnit;
    [SerializeField] private HudController hudController;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private float timeLimit = 30f;

    public event Action<bool> OnBattleOver;

    BattleState state;
    //int currentAction;  // store user selected action
    int currentAnswer;  // store user selected answer
    IEnumerator chooseAction;
    Question question;

    int currentQuestion = 0;
    int maxBattleQuestions = 1;
    int questionsRight = 0;

    enum BattleType
    {Wild, 
    Enemy,
    Boss};

    BattleType battleType;

    private Option[] shuffleAnswersList;
    private int shuffleAnswersIndex;

    // === AI RIVAL ===
    private bool aiEnabled;
    private List<int> aiTriedAnswers = new List<int>(); // Track which answers AI already tried
    private Coroutine aiAnswerRoutine;
    private bool battleLocked = false;  // if AI answer right then have a battle lock to avoid user choose something
    private bool timedOut = false;

    // Player timing
    private float questionStartTime;
    private List<float> recentAnswerTimes = new List<float>();

    // Track who answered 
    private AnswerSource battleAnswerSource;
    private enum AnswerSource
    {
        Player,
        AI,
        Timeout
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartBattle()
    {

        battleLocked = false;
        aiEnabled = AiRival.Instance != null && AiRival.Instance.IsActive;  // check if AI rival is on
        // clear the AI answer choice from last battle
        aiTriedAnswers.Clear();     
        timedOut = false;

        battleType = BattleType.Wild;
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

    public void EnemyBattle(int maxQuestions)
    {
        battleType = BattleType.Enemy;
        maxBattleQuestions = maxQuestions;
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

    public void BossBattle(int maxQuestions)
    {

        battleType = BattleType.Boss;
        maxBattleQuestions = maxQuestions;
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

        questionUnit.SetImage(question);

        Coroutine currentDialog = null;

        if (battleType == BattleType.Wild)
        {
            currentDialog = StartCoroutine(dialogBox.TypeDialog("A wild question appeared!"));
            yield return WaitForSpaceOrComplete(currentDialog, 2.0f); // Wait max 1.5 sec or until space
        }
        else if (battleType == BattleType.Boss && currentQuestion == 0)
        {
            currentDialog = StartCoroutine(dialogBox.TypeDialog("Time for the test!"));
            yield return WaitForSpaceOrComplete(currentDialog, 2.0f);
        }
        else if(battleType == BattleType.Enemy && currentQuestion == 0)
        {
            currentDialog = StartCoroutine(dialogBox.TypeDialog("You are challenged by an opponent!"));
            yield return WaitForSpaceOrComplete(currentDialog, 2.0f);
        }
        else
        {
            currentDialog = StartCoroutine(dialogBox.TypeDialog("Next question!"));
            yield return WaitForSpaceOrComplete(currentDialog, 1.5f);
        }

        currentDialog = StartCoroutine(dialogBox.TypeDialog("Pick the choice!"));
        yield return WaitForSpaceOrComplete(currentDialog, 1f); // Wait max 1 sec or until space


        //yield return new WaitForSeconds(1f);

        // AI clock starts
        state = BattleState.PLAYERANSWER;
        questionStartTime = Time.time;
        
        // AI is on and not in boss battle (AI don't participate in boss battle)
        if (aiEnabled && battleType == BattleType.Wild)
        {
            aiAnswerRoutine = StartCoroutine(AIAnswerRoutine());
        }
        else
        {
            Debug.Log($"AI NOT starting - aiEnabled: {aiEnabled}, battle type: {battleType}");
        }

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
                Debug.Log("time limit == " + timeLimit);
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

        // stop accepting answer
        if (state != BattleState.PLAYERANSWER || battleLocked)
            return;


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

        // Update selection based on answer type
        
        dialogBox.UpdateChoiceSelection(currentAnswer);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) && !dialogBox.GetAnswerSelected())
        {
            // Record player answer time
            float playerAnswerTime = Time.time - questionStartTime;

            battleAnswerSource = AnswerSource.Player; // player answered

            // Stop AI from answering
            if (aiAnswerRoutine != null)
            {
                StopCoroutine(aiAnswerRoutine);
            }

            bool isCorrect;
            isCorrect = dialogBox.DisplayAnswer(currentAnswer, shuffleAnswersIndex);

            // AI only record if CORRECT 
            if (isCorrect)
            {
                RecordPlayerAnswerTime(playerAnswerTime);
            }
            else
            {
                RecordPlayerAnswerTime(playerAnswerTime + 3f); // Add 3 second to slow AI down
            }

            // pass if player's answer is correct and its source is from player
            StartCoroutine(EndBattle(isCorrect, AnswerSource.Player));
        }

        if (!hasImageAnswers)
        {
            dialogBox.UpdateActionSelection(currentAnswer);
        }
    }
    public void OnClickAnswerButton(int answerIndex)
    {

        if (battleLocked || dialogBox.GetAnswerSelected())
            return; // Prevent player from answering when AI already finished
  

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

            // same as handleAnswer()
            float playerAnswerTime = Time.time - questionStartTime;
            RecordPlayerAnswerTime(playerAnswerTime);

            battleAnswerSource = AnswerSource.Player; // player answered

            if (aiAnswerRoutine != null)
            {
                StopCoroutine(aiAnswerRoutine);
            }



            bool isCorrect;
            isCorrect = dialogBox.DisplayAnswer(currentAnswer, shuffleAnswersIndex);

            StartCoroutine(EndBattle(isCorrect, AnswerSource.Player));

            if (!hasImageAnswers)
            {
                dialogBox.UpdateActionSelection(currentAnswer);
            }
        }
        
    }
    IEnumerator EndBattle(bool answerCorrect, AnswerSource source)
    {
        // source is from player and answer is correct
        bool playerWon =
        (source == AnswerSource.Player && answerCorrect);

        // Clear any previous dialog - to avoid "pick the choice appear when AI wins" 
        if (source == AnswerSource.AI || source == AnswerSource.Timeout)
        {
            dialogBox.ResetDalogBox();
        }

        yield return new WaitForSeconds(1.5f);
        state = BattleState.END;
        Debug.Log("answerCorrect: " + answerCorrect);

        dialogBox.EnableDialogText(true);

        int previousScore = ScoreManager.Instance.GetScoreCount();

        if (playerWon)
        {
            // Player won
            ScoreManager.Instance.AddScore(true);
            int pointsEarned = ScoreManager.Instance.GetScoreCount() - previousScore;
            mapData.CorrectAnswer(1);

            string rewardText;
            if (battleType == BattleType.Boss)
            {
                rewardText = $"You win! Rewards: {pointsEarned} points";
                questionsRight += 1;
            }
            else if(battleType == BattleType.Enemy)
            {
                rewardText = $"Correct! +{pointsEarned} points";
                questionsRight += 1;
            }
            else
            {
                rewardText = $"You win! Rewards: +1 coin, +{pointsEarned} points";
                CoinManager.Instance.AddCoin(1);
            }

            yield return StartCoroutine(dialogBox.TypeDialog(rewardText));
        }
        // Timeout, Answer is from AI, player answered wrong
        else
        {
            mapData.CorrectAnswer(0);

            if (source == AnswerSource.AI)
            {
                int aiPointsEarned = ScoreManager.Instance.AddAiScore();
                yield return StartCoroutine(dialogBox.TypeDialog($"Your Rival answered first! AI gains +{aiPointsEarned} points!"));
            }
            else if (source == AnswerSource.Timeout)
            {
                yield return StartCoroutine(dialogBox.TypeDialog("Time's up, better luck next time!"));
            }
            else
            {
                yield return StartCoroutine(dialogBox.TypeDialog("Incorrect!"));
            }
        }

        if (battleType == BattleType.Boss)
        {
            yield return new WaitForSeconds(2.5f);
            currentQuestion += 1;
            if (currentQuestion < maxBattleQuestions) 
            {
                BossBattle(maxBattleQuestions);
                yield break; // Stop the iteration so the battle doesn't end until all questions are done
            }
            else
            {
                Debug.Log("questions right =- " + questionsRight);
                if (questionsRight == maxBattleQuestions)
                {
                    questionsRight = 0;
                    currentQuestion = 0;
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
                questionsRight = 0;
                currentQuestion = 0;
            }
        }
        else if (battleType == BattleType.Enemy)
        {
            yield return new WaitForSeconds(2.5f);
            currentQuestion += 1;
            if (currentQuestion < maxBattleQuestions) 
            {
                EnemyBattle(maxBattleQuestions);
                yield break; // Stop the iteration so the battle doesn't end until all questions are done
            }
            else
            {
                Debug.Log("questions right =- " + questionsRight);
                if (questionsRight == maxBattleQuestions)
                {
                    questionsRight = 0;
                    currentQuestion = 0;
                    yield return StartCoroutine(dialogBox.TypeDialog("You got them all right! You win!"));
                    CoinManager.Instance.AddCoin(maxBattleQuestions);
                    yield return StartCoroutine(dialogBox.TypeDialog($"You are given {maxBattleQuestions} coins as a reward!"));


                    //GameController.Instance.MarkBossDefeated();

                    dialogBox.ResetDalogBox();

                    OnBattleOver(answerCorrect);
                    //hudController.TurnHudOn();
                    hudController.ExitingBattle();
                    yield break;
                }
                else
                {
                    yield return StartCoroutine(dialogBox.TypeDialog("You missed some questions. Better luck next time!"));
                    CoinManager.Instance.RemoveCoin(3);

                    if(CoinManager.Instance.GetCoinCount() >= 3)
                    {
                        yield return StartCoroutine(dialogBox.TypeDialog("You hand over 3 coins to the victor as a reward."));
                    }
                    else
                    {
                        yield return StartCoroutine(dialogBox.TypeDialog("You hand over your remaining coins to the victor as a reward."));

                    }


                }
                questionsRight = 0;
                currentQuestion = 0;
            }
        }

        yield return new WaitForSeconds(2.5f);
        dialogBox.ResetDalogBox();
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
        //Debug.Log("duration 1 == " + duration); 
        float elaspedTime = 0f;
        while (elaspedTime < duration && state == BattleState.PLAYERANSWER)
        {            
            //Debug.Log("elapsed time == " + elaspedTime);
            //Debug.Log("duration == " + duration); 
            elaspedTime += Time.deltaTime;
            yield return null;
        }

        // If the player fails to make an answer when the timer reaches 0 (15 sec)
        if (state == BattleState.PLAYERANSWER)
        {
            Debug.Log("Times up, you lose!");
            Debug.Log("elapsed time == " + elaspedTime);
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

    // ====AI RIVAL LOGIC====
    private IEnumerator AIAnswerRoutine()
    {
        Debug.Log("AI Answer Routine Started!");

        while (state == BattleState.PLAYERANSWER && !battleLocked)
        {
            // get player answering avg time
            float playerAvg = GetAveragePlayerAnswerTime();

            // AI waits 50�80% of player�s average time
            // max = 8 sec, min = 1.5 sec
            float difficultyModifier = UnityEngine.Random.Range(0.5f, 0.8f);
            float aiDelay = Mathf.Clamp(playerAvg * difficultyModifier, 1.5f, 8f);

            Debug.Log($"AI waiting {aiDelay:F2} seconds before answering... (playerAvg: {playerAvg:F2})");
            yield return new WaitForSeconds(aiDelay);

            if (state != BattleState.PLAYERANSWER || battleLocked)
            {
                Debug.Log("AI stopped - battle state changed or locked");
                yield break;
            }

            // AI hoose an answer 
            int aiChosenAnswer = ChooseAIAnswer();

            if (aiChosenAnswer == -1)
            {
                Debug.Log("AI ran out of answers to try");
                yield break;
            }

            Debug.Log($"AI chose answer index: {aiChosenAnswer}");

            // Show AI's choice with orange outline
            dialogBox.ShowAIChoice(aiChosenAnswer);

            yield return new WaitForSeconds(1.5f); // Brief pause to show selection

            // Check if AI got it right
            bool aiCorrect = (aiChosenAnswer == shuffleAnswersIndex);

            if (aiCorrect)
            {
                // AI got it right, AI wins
                Debug.Log($"AI answered correctly: {aiChosenAnswer}");
                battleAnswerSource = AnswerSource.AI;
                ResolveBattleWin();
                yield break;
            }
            else
            {
                // AI got it wrong, clear and try again
                Debug.Log($"AI answered wrong: {aiChosenAnswer}, correct was {shuffleAnswersIndex}, will try again");
                // clear AI orange outline
                dialogBox.ClearAIChoice();
                // Save the AI's answer so it doesn't choose again
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
        // only keep 5 times to count the avg
        if (recentAnswerTimes.Count > 5)
            recentAnswerTimes.RemoveAt(0);
    }

    // count player avg time
    private float GetAveragePlayerAnswerTime()
    {
        if (recentAnswerTimes.Count == 0)
            return 5f; // Start with 5 seconds default 

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




}