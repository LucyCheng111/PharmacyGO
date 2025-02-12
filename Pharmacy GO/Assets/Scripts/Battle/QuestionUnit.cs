using UnityEngine;
using UnityEngine.UI;

public class QuestionUnit : MonoBehaviour
{
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetImage(QuestionBase question)
    {
        image.sprite = question.QuestionImg;
        image.preserveAspect = true;
    }
}
