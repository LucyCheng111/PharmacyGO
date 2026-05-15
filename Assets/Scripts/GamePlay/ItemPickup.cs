using UnityEngine;

public class ItemPickup : MonoBehaviour, Interactable
{

    // Allows player to pick up items they find and gets recorded as collected

    [SerializeField] private string itemName;
    [SerializeField] private GameObject InteractPrompt;

    void Start()
    {
        if (ProgressionState.Instance.HasItem(itemName))
        {
            // If the scene is reloaded and the player has already retrieved the item
            gameObject.SetActive(false);
        }
    }
    public void Interact(Transform initiator)
    {
        Debug.Log("Added item to inventory: " + itemName);

        ProgressionState.Instance.CollectItem(itemName);

        gameObject.SetActive(false);
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
