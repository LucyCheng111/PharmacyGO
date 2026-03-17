using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    // Handles map encounter logic and question selection

    [SerializeField] List<Question> randomQuestions;
    [SerializeField] bool dangerous; // should questions be encountered randomly
    [SerializeField] int correctAnswer; // how much to increase when supplying a correct answer
    [SerializeField] int wrongAnswer; // how much to decrease when supplying a wrong answer
    [SerializeField] int module = 0;

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
        // more questions will allow for more dynamic difficulty selection
        
        // Instead of enum logic, used int logic (1-5)
        int questionDifficulty = 1; 
        
        if (difficulty <= 20) { questionDifficulty = 1; }
        else if (difficulty <= 40) { questionDifficulty = 2; }
        else if (difficulty <= 60) { questionDifficulty = 3; }
        else if (difficulty <= 80) { questionDifficulty = 4; }
        else if (difficulty <= 100) { questionDifficulty = 5; }

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