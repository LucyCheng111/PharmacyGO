using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class QuestionSection : MonoBehaviour
{
    [SerializeField] TMP_Text qurestionText;
    [SerializeField] Sprite questionImage;

    public void SetQuestion(QuestionBase question)
    {
        qurestionText.text = question.Question;
        // questionImage = question.QuestionImg;
    }

    public IEnumerator TypeQuestion(QuestionBase question)
    {
        qurestionText.text = "";
        foreach (var letter in question.Question.ToCharArray())
        {
            qurestionText.text += letter;
            yield return new WaitForSeconds(1f/40);
        }
    }
}
