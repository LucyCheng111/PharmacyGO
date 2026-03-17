using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

public class CardForMatching : MonoBehaviour
{
    public int index; //in the question and answer list in minigame controller, q[0] = a[0], etc. however, each cards location will be randomized
    public bool shown = false; //is being revealed
    public bool isQuestion = false;
    public string info;
    public GameObject text;
    public RawImage image;
    CardMatchingMinigame controller;
    public Image outline;
    public GameObject indexidentifier;


    void Awake()
    {
        controller = gameObject.GetComponentInParent<CardMatchingMinigame>();

        text = GetComponentInChildren<TextMeshProUGUI>().gameObject;
        GetComponent<Button>().onClick.AddListener(Select);
    }


    public void Hide()
    {
        if (isQuestion)
        {
            text.GetComponent<TextMeshProUGUI>().text = "";
            gameObject.GetComponent<Image>().sprite = controller.QuestionCardBack;
            image.gameObject.SetActive(false);
        }
        else
        {
            image.gameObject.SetActive(false);
            text.GetComponent<TextMeshProUGUI>().text = "";   
            Debug.Log("Card: " + index + " " + isQuestion);
            gameObject.GetComponent<Image>().sprite = controller.AnswerCardBack;
            
        }
        shown = false;
        outline.color = new Color(1,0.65f,0,0f);
        indexidentifier.SetActive(false);
    }

    public void Reveal(bool isOpponent)
    {
        text.GetComponent<TextMeshProUGUI>().text = info;
        gameObject.GetComponent<Image>().sprite = controller.CardFront;
        if(image.texture != null)
        {
            image.gameObject.SetActive(true);
        }
        shown = true;

        if (isOpponent)
        {
            outline.color = new Color(1,0.65f,0,1f);
        }
    }
    void Select()
    {
        if(controller.awaiting == CardMatchingPlay.PlayerQuestion && isQuestion && shown == false && controller.questionCard == null)
        {
            controller.questionCard = this;
            Reveal(false);
            controller.awaiting = CardMatchingPlay.PlayerAnswer;
            controller.playerPlayReader.text = "Selecting an Answer Card...";
            
        }
        else if(controller.awaiting == CardMatchingPlay.PlayerAnswer && !isQuestion && shown == false && controller.answerCard == null)
        {
            controller.answerCard = this;
            Reveal(false);
            controller.awaiting = CardMatchingPlay.PlayerAnswer;
            controller.CalculatePlay("Player");
        }
        else
        {
            //do nothing, either not player turn, or wrong card type
        }

    }

}
