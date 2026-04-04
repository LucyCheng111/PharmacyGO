using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public enum SlapjackCardType
{
    None,
    PlayerCard,
    CenterCard,
    OpponentCard,
    Other //anything else the player cant interact with
}
public class CardForSlapping : MonoBehaviour
{
    public int index; //index in whatever list it is in in minigame controller (centercards, playercards...)
    public bool shown = false; //is being revealed
    public bool isQuestion = false;
    public string info;
    public GameObject text;
    public RawImage image;
    public SlapjackMinigame controller;
    public Image outline;
    public SlapjackCardType type; 
    

    void Start()
    {
        controller = gameObject.GetComponentInParent<SlapjackMinigame>();
        //GetComponent<Button>().onClick.AddListener(Press);
        if(type == SlapjackCardType.None)
        {
            Debug.Log("WARNING- SLAPJACK CARD HAS TYPE NONE");
        }
    }
    
    public void Press()
    {
        if(type == SlapjackCardType.PlayerCard && controller.awaiting == SlapjackPlay.PlayerTurn)
        {
            controller.HaltOpponentAction();
            StartCoroutine(moveToCenter(true));
            Reveal(false);
            controller.CardDrawn(true, this);
        }
        else if(type == SlapjackCardType.CenterCard) //player has selected center card
        {
            controller.HaltOpponentAction();
            StartCoroutine(controller.PlayerSelectedCenter());
        }
        else if(type == SlapjackCardType.OpponentCard && controller.awaiting == SlapjackPlay.OpponentTurn) //this was called by the opponent
        {
            controller.StopAllActions();
            StartCoroutine(moveToCenter(false));
            Reveal(false); //even though the opponent is selecting this, I wont outline every card it touches, only when it selects the center
            controller.CardDrawn(false, this);
        }
    }


    public IEnumerator moveToCenter(bool isPlayer)
    {
        controller.cardisMoving = true;
        transform.SetParent(controller.centerpile.transform, true);
        this.type = SlapjackCardType.CenterCard;
        
        //this.transform.position = controller.centerpile.transform.position;
        if (isPlayer)
        {
            controller.playerCards.RemoveAt(index);
            controller.centerCards.Add(this);
            controller.reIndexCards(1); //reindex center
            controller.reIndexCards(0); //reindex player
        }
        else
        {
            controller.opponentCards.RemoveAt(index);
            controller.centerCards.Add(this);
            controller.reIndexCards(1); //reindex center
            controller.reIndexCards(2); //reindex opponent
        }
        controller.UpdateCardNumReaders();
        controller.cardisMoving = false;
        float totaltime = 0f;
        float duration = .2f;
        while(this.transform.position != controller.centerpile.transform.position)
        {
            totaltime += Time.deltaTime;
            float t = totaltime / duration;
            this.gameObject.transform.position = Vector3.Lerp(this.transform.position, controller.centerpile.transform.position, Mathf.Clamp01(t));
            
            yield return new WaitForSeconds(.017f);
        }
        yield return null;
    }
    public IEnumerator SwapSides(bool toPlayer) //gives the card to the player/ opp from the others deck
    {
        controller.cardisMoving = true;
        Hide();
        if (toPlayer)
        {
            
            //this.transform.position = controller.centerpile.transform.position;
            this.type = SlapjackCardType.PlayerCard;
            transform.SetParent(controller.playerpile.transform, true);

            controller.opponentCards.RemoveAt(index);
            index = controller.playerCards.Count;

            
            controller.playerCards.Add(this);
            this.gameObject.name = "Playercard_" + index;

            controller.reIndexCards(0); //reindex player
            controller.reIndexCards(2); //reindex opp
            controller.UpdateCardNumReaders();
            controller.cardisMoving = false;
            float totaltime = 0f;
            float duration = .2f;
            while(this.transform.position != controller.playerpile.transform.position)
            {
                totaltime += Time.deltaTime;
                float t = totaltime / duration;
                this.gameObject.transform.position = Vector3.Slerp(this.transform.position, controller.playerpile.transform.position, Mathf.Clamp01(t));
                yield return new WaitForSeconds(.017f);
            }
            yield return null;
        } 
        else
        {
            
            //this.transform.position = controller.centerpile.transform.position;
            this.type = SlapjackCardType.OpponentCard;
            transform.SetParent(controller.opponentpile.transform, true);

            controller.playerCards.RemoveAt(index);
            index = controller.opponentCards.Count;

            
            controller.opponentCards.Add(this);
            this.gameObject.name = "Opponentcard_" + index;

            controller.reIndexCards(0); //reindex player
            controller.reIndexCards(2); //reindex opp
            controller.UpdateCardNumReaders();
            controller.cardisMoving = false;
            float totaltime = 0f;
            float duration = .2f;
            while(this.transform.position != controller.opponentpile.transform.position)
            {
                totaltime += Time.deltaTime;
                float t = totaltime / duration;
                this.gameObject.transform.position = Vector3.Slerp(this.transform.position, controller.opponentpile.transform.position, Mathf.Clamp01(t));
                yield return new WaitForSeconds(.017f);
            }
            yield return null;
        }
    }
    public IEnumerator moveFromCenter(bool isPlayer)
    {
        controller.cardisMoving = true;
        Hide();
        if (isPlayer)
        {
            transform.SetParent(controller.playerpile.transform, true);

            controller.centerCards.RemoveAt(index);
            index = controller.playerCards.Count;

            
            controller.playerCards.Add(this);
            this.gameObject.name = "Playercard_" + index;

            controller.reIndexCards(1); //reindex center
            controller.UpdateCardNumReaders();

            this.type = SlapjackCardType.PlayerCard;
            float totaltime = 0f;
            float duration = .2f;
            while(this.transform.position != controller.playerpile.transform.position)
            {
                totaltime += Time.deltaTime;
                float t = totaltime / duration;
                this.gameObject.transform.position = Vector3.Lerp(this.transform.position, controller.playerpile.transform.position, Mathf.Clamp01(t));
                yield return new WaitForSeconds(.017f);
            }
            //this.transform.position = controller.centerpile.transform.position;
            
            
            yield return null;
            controller.cardisMoving = false;
        }
        else //isrival
        {
            this.type = SlapjackCardType.OpponentCard;
            
            transform.SetParent(controller.opponentpile.transform, true);

            controller.centerCards.RemoveAt(index);
            index = controller.opponentCards.Count;

            
            controller.opponentCards.Add(this);
            this.gameObject.name = "Opponentcard_" + index;

            controller.reIndexCards(1); //reindex center
            controller.UpdateCardNumReaders();
            controller.cardisMoving = false;

            float totaltime = 0f;
            float duration = .2f;
            while(this.transform.position != controller.opponentpile.transform.position)
            {
                totaltime += Time.deltaTime;
                float t = totaltime / duration;
                this.gameObject.transform.position = Vector3.Lerp(this.transform.position, controller.opponentpile.transform.position, Mathf.Clamp01(t));
                yield return new WaitForSeconds(.017f);
            }
            //this.transform.position = controller.centerpile.transform.position;
            
            yield return null;
        }
        controller.cardisMoving = false;
        yield return null;
    }

    //move this card from question pile to the left
    public IEnumerator selectThisQuestionCard()
    {
        controller.cardisMoving = true;
        controller.currentQuestion = controller.questions[index];
        
        //this.transform.position = controller.centerpile.transform.position;
        this.type = SlapjackCardType.Other;
        Reveal(false);
        controller.cardisMoving = false;
        float totaltime = 0f;
        float duration = .2f;
        while(this.transform.position != controller.currentquestioncardlocation.transform.position)
        {
            totaltime += Time.deltaTime;
            float t = totaltime / duration;
            this.gameObject.transform.position = Vector3.Lerp(this.transform.position, controller.currentquestioncardlocation.transform.position, Mathf.Clamp01(t));
            yield return new WaitForSeconds(.017f);
        }
        yield return null;
    }
    public IEnumerator resetThisQuestionCard()
    {
        controller.cardisMoving = true;
        float totaltime = 0f;
        float duration = .2f;
        while(this.transform.position != controller.questionpile.transform.position)
        {
            totaltime += Time.deltaTime;
            float t = totaltime / duration;
            this.gameObject.transform.position = Vector3.Lerp(this.transform.position, controller.questionpile.transform.position, Mathf.Clamp01(t));
            yield return new WaitForSeconds(.017f);
        }
        //this.transform.position = controller.centerpile.transform.position;
        this.type = SlapjackCardType.Other;
        controller.cardisMoving = false;
        yield return null;
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
            gameObject.GetComponent<Image>().sprite = controller.AnswerCardBack;
            
        }
        shown = false;
        outline.color = new Color(1,0.65f,0,0f);
    }

    public void Reveal(bool outlineCard)
    {
        text.GetComponent<TextMeshProUGUI>().text = info;
        gameObject.GetComponent<Image>().sprite = controller.CardFront;
        if(image.texture != null)
        {
            image.gameObject.SetActive(true);
        }
        shown = true;

        if (outlineCard)
        {
            outline.color = new Color(1,0.65f,0,1f);
        }
    }

}
