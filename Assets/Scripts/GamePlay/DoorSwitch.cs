using UnityEngine;
using UnityEngine.Rendering; // Required namespace for Volume and VolumeProfile
using UnityEngine.Rendering.Universal;
using System;
using System.Collections;

public class DoorSwitch : MonoBehaviour, Interactable
{

    // Allows player to interact with a door switch, unlocking some door

    [SerializeField] private string itemName;
    [SerializeField] private GameObject InteractPrompt;
    [SerializeField] private Dialog flipDialog;
    [SerializeField] private GameObject VisualEffect;
    
     void Start()
    {
        if (ProgressionState.Instance.HasItem(itemName))
        {
            // If the scene is reloaded and the player has already flipped the switch

            InteractPrompt.SetActive(false);

            GetComponent<Collider2D>().enabled = false;
        }
    }

    public void Interact(Transform initiator)
    {
        Debug.Log("Added item to inventory: " + itemName);

        ProgressionState.Instance.CollectItem(itemName);

        StartCoroutine(DialogManager.Instance.ShowDialog(flipDialog));

        InteractPrompt.SetActive(false);

        if(VisualEffect != null)
        {
            VisualEffect.SetActive(!VisualEffect.activeSelf);
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
