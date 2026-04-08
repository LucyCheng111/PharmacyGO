using UnityEngine;

public class SceneBarrier : MonoBehaviour
{
    // This barrier will unlock and disappear once the player has obtained a specific item
    [SerializeField] private string itemName;

    private void Start()
    {
        if (ProgressionState.Instance.HasItem(itemName))
        {
            Unlock();
        }
    }

    void Update()
    {
        if (ProgressionState.Instance.HasItem(itemName))
        {
            Unlock();
        }
    }

    private void Unlock()
    {
        // Disable the collider so the player can pass

        Collider2D collider = GetComponent<Collider2D>();

        if (collider!= null)
        {
            collider.enabled = false;
        }
    }
}
