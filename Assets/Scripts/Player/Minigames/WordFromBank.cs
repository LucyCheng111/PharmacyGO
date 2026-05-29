using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class WordFromBank : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public string word;
    public TextMeshProUGUI text;
    public Transform lastLocation;
    public GameObject Facade; //top image to recolor
    public int indexInSentence = -1; //if not in sentence, -1
    public int indexInWords = 0; //if in sentence, -1, else whatever index in WordBankMinigame.words
    public bool beingDragged;

    public IEnumerator shakeInHand()
    {
        yield return new WaitForSeconds(.1f);

    }
    //pick up the word (need to hold it down and drag)
    public void OnPointerDown(PointerEventData eventData)
    {
        lastLocation = this.transform;
        beingDragged = true;
        GetComponentInParent<WordBankMinigame>().WordInHand = this.gameObject.GetComponent<RectTransform>();
        transform.SetParent(GetComponentInParent<WordBankMinigame>().GameBoard.transform, false);

        GetComponent<RectTransform>().anchorMin = new Vector2(.5f,.5f);
        GetComponent<RectTransform>().anchorMax = new Vector2(.5f,.5f);
    }

    //drop the word
    public void OnPointerUp(PointerEventData eventData)
    {
        beingDragged = false;
        GetComponentInParent<WordBankMinigame>().WordInHand = null;
        GetComponentInParent<WordBankMinigame>().exchangeBoardandSentence(this.gameObject.transform);
        
    }
}
