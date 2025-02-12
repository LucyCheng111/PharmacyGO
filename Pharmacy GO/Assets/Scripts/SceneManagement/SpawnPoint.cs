using UnityEngine;
using Unity.Cinemachine;

public class SpawnPoint : MonoBehaviour
{
    public string spawnPointID;

    private void Start()
    {
        string lastSpawnPointID = PlayerPrefs.GetString("SpawnPointID", "");

        if (lastSpawnPointID == spawnPointID)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                player.transform.position = transform.position;

                // STOP MOVEMENT IMMEDIATELY AFTER SPAWNING
                PlayerControl playerControl = player.GetComponent<PlayerControl>();
                if (playerControl != null)
                {
                    playerControl.StopMovement();
                }

                // REASSIGN Cinemachine Camera Target
                CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
                if (cam != null)
                {
                    cam.Follow = player.transform;
                }
            }
        }
    }
}
