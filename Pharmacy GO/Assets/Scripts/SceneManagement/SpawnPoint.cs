using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class SpawnPoint : MonoBehaviour
{
    public string spawnPointID;

    private void Start()
    {
        //if (!PlayerPrefs.HasKey("SpawnPointID"))
        //    return;

        string lastSpawnPointID = PlayerPrefs.GetString("SpawnPointID", "");

        if (lastSpawnPointID == spawnPointID)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                // place player according to recorded spawn point
                // rounding and math is to ensure player is aligned on the grid
                player.transform.position = new Vector2((Mathf.Round(transform.position.x) * 2 + 1) / 2f, (Mathf.Round(transform.position.y) * 2 + 1) / 2f + 0.3f);

                // STOP MOVEMENT IMMEDIATELY AFTER SPAWNING
                PlayerControl playerControl = player.GetComponent<PlayerControl>();
                if (playerControl != null)
                {
                    playerControl.StopMovement();
                }

                StartCoroutine(SetCameraTargetWithDelay(player));

                // REASSIGN Cinemachine Camera Target
                CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
                if (cam != null)
                {
                    cam.Follow = player.transform;
                }
            }
        }

        // PlayerPrefs.DeleteKey("SpawnPointID");
    }

    IEnumerator SetCameraTargetWithDelay(GameObject player)
    {
        yield return new WaitForEndOfFrame();
        CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
        if (cam != null)
        {
            cam.Follow = player.transform;
        }
    }

}
