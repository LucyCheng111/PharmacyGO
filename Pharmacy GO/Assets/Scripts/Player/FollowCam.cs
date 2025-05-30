using UnityEngine;

public class FollowCam : MonoBehaviour
{

    // Attaches the camera to the player

    public GameObject player;
    public Vector3 offset = new Vector3(0, 0, -10);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Automatically find the player in the scene
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");

            if (player == null)
            {
                Debug.LogError("FollowCam: Player NOT found in the scene!");
            }
            else
            {
                Debug.Log("FollowCam: Player found and assigned.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Fix the bug that the player stuck when return to the town (Samplescene)
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
                return;
        }

        transform.position = player.transform.position + offset;
    }
}
