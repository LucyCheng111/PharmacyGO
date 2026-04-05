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
    NPCMinigamePlayer minigamePlayer;
    public bool seen = false; //primarily used in murder mystery to see if all objects have been interacted with


    //optional, for when npcs can teleport player between scenes
    [SerializeField] private bool showTravelChoice = false;
    [SerializeField] private List<String> travelChoices = new List<String> {"Yes", "No"};
    [SerializeField] private List<String> minigameChoices = new List<String>();
    [SerializeField] private int yesChoiceInt = 0;
    [SerializeField] Dialog cantaffordtravelDialog;
    
    //optional, for when npcs give access to the minigame
    private bool minigameconfirmation = true;

    void Awake()
    {   
        minigameChoices.Clear();
        npcMovement = GetComponent<NPCMovement>();
        sceneTransitioner = GetComponent<NPCSceneTransitioner>();
        minigamePlayer = GetComponent<NPCMinigamePlayer>();

        if (minigamePlayer)
        {
            if(minigamePlayer.type == MinigameType.CardMatching)
            {
                minigameChoices.Add("Play Cards " + "(Lvl." + minigamePlayer.difficulty + ")");
            }
            else if(minigamePlayer.type == MinigameType.WordBank)
            {
                minigameChoices.Add("Play Pharmacy Word Scramble" + "(Lvl." + minigamePlayer.difficulty + ")");
            }
            else if(minigamePlayer.type == MinigameType.Slapjack)
            {
                minigameChoices.Add("Play Slapjack" + "(Lvl." + minigamePlayer.difficulty + ")");
            }
            minigameChoices.Add("Leave");
        }
        
    }

    public void Interact(Transform initiator)
    {
        
        //npcMovement.LookTowards(initiator.position);  
        seen = true;
        if(minigamePlayer != null)
        {
            minigameconfirmation = false;
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, minigameChoices, OnMinigameChoiceSelected));
        }
        if (minigameconfirmation)
        {
            if(showTravelChoice && sceneTransitioner != null && travelChoices != null & travelChoices.Count > 1)
            {
                //dialogue if the npc is able to transport the player to other scenes
                if(CoinManager.Instance.GetCoinCount() < sceneTransitioner.price)
                {
                    StartCoroutine( DialogManager.Instance.ShowDialog(cantaffordtravelDialog));
                }
                else //can afford to travel
                {
                    StartCoroutine(DialogManager.Instance.ShowDialog(dialog, travelChoices, OnTravelChoiceSelected));
                    
                }
            }
            else
            {
                //normal NPC dialog
                StartCoroutine( DialogManager.Instance.ShowDialog(dialog) );
            }
        }
    
    }

    private void OnTravelChoiceSelected(int choiceIdx)
    {
        if(choiceIdx == yesChoiceInt && sceneTransitioner != null)
        {
            sceneTransitioner.Travel();
            CoinManager.Instance.RemoveCoin(sceneTransitioner.price);
        }
    }

    private void OnMinigameChoiceSelected(int choiceIdx)
    {
        if(choiceIdx == yesChoiceInt)
        {
            minigamePlayer.StartMinigame();
        }
        minigameconfirmation = true;
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

