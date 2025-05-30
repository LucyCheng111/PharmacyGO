using UnityEngine;

public class MapTriggerZone : MonoBehaviour
{

    // Detects when a player has entered a map

    public int areaIndex;  // Index represent the area to trigger

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))  // Ensure the player has the correct tag
        {
            other.GetComponent<PlayerControl>().SetAreaTracker(areaIndex);
            Debug.Log("Player Triggered the map!" + areaIndex);
            Destroy(gameObject);  // Remove trigger after activation (optional)
        }
    }

}

