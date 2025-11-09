using UnityEngine;


public class NPCController : MonoBehaviour, Interactable
{

    // Manager for NPC dialog and interactions

    [SerializeField] Dialog dialog;
    [SerializeField] private GameObject InteractPrompt;
    public void Interact()
    {
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

