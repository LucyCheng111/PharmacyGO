using UnityEngine;
using UnityEngine.UI;

public class AnswerUnit : MonoBehaviour
{
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetImage(QuestionBase question, int answerIndex)
    {
        if (question.AnswersImg.Length > answerIndex && question.AnswersImg[answerIndex] != null)
        {
            image.sprite = question.AnswersImg[answerIndex];
            image.enabled = true;  // Ensure the image is visible

        }
        else
        {
            image.sprite = null;
            image.enabled = false; // Hide if no image is available
        }
    }
}