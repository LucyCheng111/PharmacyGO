using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class QuestionSection : MonoBehaviour
{
    [SerializeField] TMP_Text questionText;
    //[SerializeField] Sprite questionImage;

    public void SetQuestion(QuestionBase question)
    {
        questionText.text = question.Question;
        // questionImage = question.QuestionImg;
    }

    public IEnumerator TypeQuestion(Question question)
    {
        questionText.text = "";
        foreach (var letter in question.question.ToCharArray())
        {
            questionText.text += letter;
            yield return new WaitForSeconds(1f/40);
        }
    }
}
