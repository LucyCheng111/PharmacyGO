using UnityEngine;


public class NPCController : MonoBehaviour, Interactable
{

    // Manager for NPC dialog and interactions

    [SerializeField] Dialog dialog;
    [SerializeField] private GameObject InteractPrompt;
    
    NPCMovement npcMovement;

    void Awake()
    {
        npcMovement = GetComponent<NPCMovement>();
    }

    public void Interact(Transform initiator)
    {
        
        npcMovement.LookTowards(initiator.position);
        StartCoroutine( DialogManager.Instance.ShowDialog(dialog) );

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

