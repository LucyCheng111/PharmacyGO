using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ChoiceText : MonoBehaviour, IPointerClickHandler
{

    // Handles displaying the choices available to the player

    Text text;
    private System.Action<int> onChoiceClicked;
    private int choiceIndex;


    private void Awake()
    {
        text = GetComponent<Text>();
        text.raycastTarget = true;

    }

    public void Initialize(int index, System.Action<int> clickCallback)
    {
        choiceIndex = index;
        onChoiceClicked = clickCallback;
    }

    public void SetSelected(bool selected)
    {
        text.color = (selected) ? Color.red : Color.black;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"CLICKED choice {choiceIndex}!"); 
        onChoiceClicked?.Invoke(choiceIndex);
    }

    public Text TextField => text;

}
