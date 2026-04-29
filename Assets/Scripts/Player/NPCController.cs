using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class NPCController : MonoBehaviour, Interactable
{

    // Manager for NPC dialog and interactions

    [SerializeField] Dialog dialog;
    public GameObject InteractPrompt;
    
    NPCMovement npcMovement;
    NPCSceneTransitioner sceneTransitioner;
    NPCMinigamePlayer minigamePlayer;
    bool willCharge = true; //if transport to MM is free
    public bool seen = false; //primarily used in murder mystery to see if all objects have been interacted with


    //optional, for when npcs can teleport player between scenes
    [SerializeField] private bool showTravelChoice = false;
    [SerializeField] private List<String> travelChoices = new List<String> {"Yes", "No"};
    [SerializeField] private List<String> minigameChoices = new List<String>();
    [SerializeField] Dialog getProgressionItemDialog;
    [SerializeField] private int yesChoiceInt = 0;
    [SerializeField] Dialog cantaffordtravelDialog;
    [SerializeField] Dialog TravelIsFreeDialog; //for already completed levels
    
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
                if (ProgressionState.Instance.HasItem(sceneTransitioner.NecessaryProgressionItemName))
                {
                    if(sceneTransitioner.levelNumber == -1 && convertSpawnPointIDtoLevel(sceneTransitioner.targetSpawnPointID) < PlayerPrefs.GetInt("UnlockedLevel"))
                    {
                        //player has gotten past this MM level
                        willCharge = false;
                        StartCoroutine(DialogManager.Instance.ShowDialog(TravelIsFreeDialog, travelChoices, OnTravelChoiceSelected));
                    }
                    //dialogue if the npc is able to transport the player to other scenes
                    else if(CoinManager.Instance.GetCoinCount() < sceneTransitioner.price)
                    {
                        StartCoroutine( DialogManager.Instance.ShowDialog(cantaffordtravelDialog));
                    }
                    else //can afford to travel
                    {
                        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, travelChoices, OnTravelChoiceSelected));
                        
                    }
                }
                else //block player from playing MM until they have the progression item
                {
                    string statement = "";
                    if(ProgressionState.Instance.ReturnNameForItem(sceneTransitioner.NecessaryProgressionItemName) == "")
                    {
                        Debug.Log("MM NPC HAS INCORRECT PROGRESSION ITEM NAME");
                        statement = "Before I can take you to the mystery level, you need to go aquire the item for Dr Shepard.";
                    }
                    else
                    {
                        statement = "Before I can take you to the mystery level, you need to go aquire " + 
                            ProgressionState.Instance.ReturnNameForItem(sceneTransitioner.NecessaryProgressionItemName) + ".";
                    }
                    
                    
                    getProgressionItemDialog.Lines.Add(statement);

                    StartCoroutine( DialogManager.Instance.ShowDialog(getProgressionItemDialog));
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
            if (willCharge)
            {
                CoinManager.Instance.RemoveCoin(sceneTransitioner.price);   
            }
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

    public void SetDialog(Dialog newDialog)
    {
        // This allows other scripts to change what dialog this NPC says

        dialog = newDialog;
    }
    
    //gets the integer value from a string
    public int convertSpawnPointIDtoLevel(string input)
    {
        //got this from https://discussions.unity.com/t/extract-number-from-string/4361
        return Convert.ToInt32(Regex.Replace(input, "[^0-9]", ""));
    }
}

