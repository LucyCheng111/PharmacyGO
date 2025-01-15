using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<QuestionBase> randomQuestions;

    public QuestionBase GetRandomQuestion()
    {
        var randomQuestion = randomQuestions[Random.Range(0, randomQuestions.Count)];
        return randomQuestion;
    }
}
