using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardForMatching : MonoBehaviour
{
    public int index; //in the question and answer list in minigame controller, q[0] = a[0], etc. however, each cards location will be randomized
    public bool shown = false; //is being revealed
    public bool isQuestion = false;
    public string info;
    public GameObject text;
    MinigameController controller;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponentInParent<MinigameController>();

        text = GetComponentInChildren<TextMeshProUGUI>().gameObject;
        GetComponent<Button>().onClick.AddListener(Select);
    }


    public void Hide()
    {
        if (isQuestion)
        {
            text.GetComponent<TextMeshProUGUI>().text = "";
        }
        else
        {
            text.GetComponent<TextMeshProUGUI>().text = "";   
        }
        shown = false;
    }

    public void Reveal()
    {
        text.GetComponent<TextMeshProUGUI>().text = info;
        shown = true;
    }
    void Select()
    {
        if(controller.awaiting == CardMatchingPlay.PlayerQuestion && isQuestion && shown == false)
        {
            controller.questionCard = this;
            Reveal();
            controller.awaiting = CardMatchingPlay.PlayerAnswer;
            controller.playerPlayReader.text = "Selecting an Answer Card...";
            
        }
        else if(controller.awaiting == CardMatchingPlay.PlayerAnswer && !isQuestion  && shown == false)
        {
            controller.answerCard = this;
            Reveal();
            controller.awaiting = CardMatchingPlay.PlayerAnswer;
            controller.CalculatePlay("Player");
        }
        else
        {
            //do nothing, either not player turn, or wrong card type
        }

    }

}
