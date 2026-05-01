using UnityEngine;
using UnityEngine.Tilemaps;

public class LockedDoor : MonoBehaviour, Interactable
{

    // Locked door that the player must unlock by finding a switch

    [SerializeField] private string itemName;
    [SerializeField] private GameObject InteractPrompt;
    [SerializeField] private Dialog lockedDialog;
    [SerializeField] private Dialog unlockedDialog;
    [SerializeField] private Tilemap doorTilemap;

    void Start()
    {
        if (ProgressionState.Instance.HasItem(itemName))
        {
            // If the scene is reloaded and the player has already unlocked the door

            Unlock();
        }
    }

    public void Interact(Transform initiator)
    {
        if (ProgressionState.Instance.HasItem(itemName))
        {
            // If the player has retrieved the hidden item, unlock

            Unlock();
        }
        else
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(lockedDialog));
        }
    }

    private void Unlock()
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(unlockedDialog));

        GetComponent<Collider2D>().enabled = false;

        doorTilemap.gameObject.SetActive(false);
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
