using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class WordFromBank : MonoBehaviour, IPointerClickHandler
{
    public string word;
    public TextMeshProUGUI text;
    public GameObject Facade; //top image to recolor
    public int indexInSentence = -1; //if not in sentence, -1
    public int indexInWords = 0; //if in sentence, -1, else whatever index in WordBankMinigame.words
    public bool beingDragged;

    public IEnumerator shakeInHand()
    {
        yield return new WaitForSeconds(.1f);

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
