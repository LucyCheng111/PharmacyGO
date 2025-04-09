using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class QuestionSection : MonoBehaviour
{
    [SerializeField] TMP_Text questionText;
    [SerializeField] TMP_Text questionID;

    public void SetQuestion(QuestionBase question)
    {
        questionText.text = question.Question;
        questionID.text = "ID: 9999";
    }

    public IEnumerator TypeQuestion(Question question)
    {
        StartCoroutine(TypeID("ID: 9999"));
        questionText.text = "";
        foreach (var letter in question.question.ToCharArray())
        {
            questionText.text += letter;
            yield return new WaitForSeconds(1f/40);
        }
    }

    public IEnumerator TypeID(string id)
    {
        questionID.text = "";
        foreach (var letter in id.ToCharArray())
        {
            questionID.text += letter;
            yield return new WaitForSeconds(1f / 20);
        }
    }
}
