using UnityEngine;

[CreateAssetMenu(fileName = "QuestionBase", menuName = "Question/Create new question")]
public class QuestionBase : ScriptableObject
{
    [SerializeField] private string question; // image or text
    [SerializeField] private Sprite questionImg;
    [SerializeField] private string[] answers;
    [SerializeField] private Sprite[] answersImg;
    [SerializeField] private int correctAnswerIndex;

    [SerializeField] private QuestionType difficulty;

    public string Question
    {
        get {return question;}
    }

    public Sprite QuestionImg
    {
        get {return questionImg;}
    }

    public string[] Answers
    {
        get {return answers;}
    }

    public Sprite[] AnswersImg
    {
        get {return answersImg;}
    }

    public int CorrectAnswerIndex
    {
        get {return correctAnswerIndex;}
    }

    public QuestionType Difficulty
    {
        get {return difficulty;}
    }
}


public enum QuestionType
{
    easy,
    medium,
    hard
}