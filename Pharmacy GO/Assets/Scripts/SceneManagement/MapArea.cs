using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<QuestionBase> randomQuestions;
    [SerializeField] int correctAnswer; // how much to increase when supplying a correct answer
    [SerializeField] int wrongAnswer; // how much to decrease when supplying a wrong answer

    private int correctStreak;
    private int difficulty;
    private bool validQuestion;

    public QuestionBase GetRandomQuestion()
    {

        // difficulty based selection will need to be re-worked once we have more questions
        // more questions will be added once the database is operational
        // this is also very un-optimized
        validQuestion = false;
        var randomQuestion = randomQuestions[Random.Range(0, randomQuestions.Count)];

        while (!validQuestion)
        {
            if ((difficulty <= 30) && (randomQuestion.Difficulty == QuestionType.easy)) { validQuestion = true;}
            else if ((difficulty > 30) && (difficulty < 70) && (randomQuestion.Difficulty == QuestionType.medium)) { validQuestion = true; }
            else if ((difficulty >= 70) && (randomQuestion.Difficulty == QuestionType.hard)) { validQuestion = true; }
            else { randomQuestion = randomQuestions[Random.Range(0, randomQuestions.Count)]; }
        }
        return randomQuestion;
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
}
