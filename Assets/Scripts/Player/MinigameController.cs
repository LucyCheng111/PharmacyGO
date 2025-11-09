using UnityEngine;

public class MinigameController : MonoBehaviour, Interactable
{

    [SerializeField] private GameObject InteractPrompt;
    

    public void Interact()
    {
       
       
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
