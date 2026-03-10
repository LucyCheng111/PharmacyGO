using System;
using System.Collections.Generic;
using UnityEngine;


public class NPCController : MonoBehaviour, Interactable
{

    // Manager for NPC dialog and interactions

    [SerializeField] Dialog dialog;
    [SerializeField] private GameObject InteractPrompt;
    
    NPCMovement npcMovement;
    NPCSceneTransitioner sceneTransitioner;
    public bool seen = false; //primarily used in murder mystery to see if all objects have been interacted with


    //optional, for when npcs can teleport player between scenes
    [SerializeField] private bool showTravelChoice = false;
    [SerializeField] private List<String> travelChoices = new List<String> {"Yes", "No"};
    [SerializeField] private int yesChoiceInt = 0;



    void Awake()
    {
        npcMovement = GetComponent<NPCMovement>();
        sceneTransitioner = GetComponent<NPCSceneTransitioner>();
    }

    public void Interact(Transform initiator)
    {
        
        //npcMovement.LookTowards(initiator.position);  
        seen = true;

        if(showTravelChoice && sceneTransitioner != null && travelChoices != null & travelChoices.Count > 1)
        {
            //dialogue if the npc is able to transport the player to other scenes
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, travelChoices, OnTravelChoiceSelected));
        }
        else
        {
            //normal NPC dialog
            StartCoroutine( DialogManager.Instance.ShowDialog(dialog) );
        }

    }

    private void OnTravelChoiceSelected(int choiceIdx)
    {
        if(choiceIdx == yesChoiceInt && sceneTransitioner != null)
        {
            sceneTransitioner.Travel();
        }
    }

    public void ShowPrompt()
    {
        InteractPrompt.SetActive(true);
    }

    public void HidePrompt()
    {
        InteractPrompt.SetActive(false);
    }

}

