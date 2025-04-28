using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Question> randomQuestions;
    [SerializeField] bool dangerous; // should questions be encountered randomly
    [SerializeField] int correctAnswer; // how much to increase when supplying a correct answer
    [SerializeField] int wrongAnswer; // how much to decrease when supplying a wrong answer

    Database database;
    Module moduleManager;

    private int correctStreak;
    private int difficulty;
    private bool validQuestion;

    public static MapArea i { get; private set; }

    public bool HasQuestions()
    {
        return randomQuestions != null && randomQuestions.Count > 0;
    }

    private void Awake()
    {
        i = this;
    }

    void Start()
    {
        StartCoroutine(load());
    }

    IEnumerator load()
    {   
        database = new Database();
        StartCoroutine(database.load());
        yield return new WaitUntil(() => database.loaded);
        randomQuestions = database.questionSet.questions;
        moduleManager = new Module(randomQuestions);
    }

    public Question GetRandomQuestion()
    {
        // difficulty based selection will need to be re-worked once we have more questions
        // more questions will be added once the database is operational
        // this is also very un-optimized
        // validQuestion = false;
        // var randomQuestion = randomQuestions[Random.Range(0, randomQuestions.Count)];
        // int wrongTries = 0;

        // while (!validQuestion)
        // {
        //     if ((difficulty <= 30) && (randomQuestion.difficulty == Question.DifficultyIndex.easy)) { validQuestion = true;}
        //     else if ((difficulty > 30) && (difficulty < 70) && (randomQuestion.difficulty == Question.DifficultyIndex.medium)) { validQuestion = true; }
        //     else if ((difficulty >= 70) && (randomQuestion.difficulty == Question.DifficultyIndex.hard)) { validQuestion = true; }
        //     else if (wrongTries >= 10) { validQuestion = true; }
        //     else { randomQuestion = randomQuestions[Random.Range(0, randomQuestions.Count)]; }
        //     wrongTries++;
        // }
        // return randomQuestion;
        int module = 1;
        Question.DifficultyIndex questionDifficulty = Question.DifficultyIndex.None;
        if (difficulty <= 20) { questionDifficulty = Question.DifficultyIndex.Beginner;}
        else if (difficulty <= 40) { questionDifficulty = Question.DifficultyIndex.Novice;}
        else if (difficulty <= 60) { questionDifficulty = Question.DifficultyIndex.Intermediate;}
        else if (difficulty <= 80) { questionDifficulty = Question.DifficultyIndex.Advanced;}
        else if (difficulty <= 100) { questionDifficulty = Question.DifficultyIndex.Expert;}
        return moduleManager.GetRandomQuestion(module, questionDifficulty);
    }

    public int GetCorrectStreak() { return correctStreak; }
    public int GetDifficulty() { return difficulty; }

    

    public void CorrectAnswer(int correct)
    {
        if (correct == 0)
        {
            correctStreak = 0;
            difficulty += wrongAnswer;
            if (difficulty < 0)
            {
                difficulty = 0;
            }
        }
        else
        {
            correctStreak += 1;
            difficulty += correctAnswer;
            if (difficulty > 100)
            {
                difficulty = 100;
            }
        }
        Debug.Log("Difficulty: " + difficulty);
    }

    public bool IsDangerous()
    {
        return dangerous;
    }

    public int getQuestionID(Question question)
    {
        return database.questionSet.questions.IndexOf(question);
    }
}
