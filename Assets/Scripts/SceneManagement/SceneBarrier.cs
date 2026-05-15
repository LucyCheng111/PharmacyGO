using UnityEngine;

public class SceneBarrier : MonoBehaviour
{
    // This barrier will unlock and disappear once the player has obtained a specific item
    [SerializeField] private string itemName;

    private void Start()
    {
        if (ProgressionState.Instance.HasItem(itemName))
        {
            if(LevelManager.Instance.UnlockedLevel >= ProgressionState.Instance.ReturnLevelForItem(itemName))
            {
                Unlock();
            }
        }
    }

    void Update()
    {
        //if (ProgressionState.Instance.HasItem(itemName))
        {
            //Unlock();
        }
    }

    private void Unlock()
    {
        // Disable the collider so the player can pass

        gameObject.SetActive(false);
    }
}
