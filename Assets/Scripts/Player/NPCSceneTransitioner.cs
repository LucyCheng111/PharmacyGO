using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using Unity.VisualScripting;

public class NPCSceneTransitioner : MonoBehaviour
{
    public int sceneBuildIndex;
    public int levelNumber;
    public string targetSpawnPointID;
    private bool isTransitioning = false;       

    //call this from a dialogue choice from an NPC set up to transport the player
    public void Travel()
    {
        if(!gameObject.activeInHierarchy) return;
        if(isTransitioning) return;

        StartCoroutine(TransitionToScene());
    }

    private IEnumerator TransitionToScene()
    {
        // Save the target spawn point ID before scene transition
        Debug.Log("player should be teleported right now");
        PlayerPrefs.SetString("SpawnPointID",targetSpawnPointID);

        AsyncOperation operation = LevelManager.Instance.LoadLevel(levelNumber);
        yield return new WaitUntil(()=>operation.isDone);
        yield return new WaitForSeconds(0.1f);

        //fix cinemachine
        StartCoroutine(UpdateCinemachineTarget());
    }

    private IEnumerator UpdateCinemachineTarget()
    {
        yield return new WaitForSeconds(0.5f); //small delay to ensure the player loads

        CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(cam != null && player != null)
        {
            cam.Follow = player.transform;

            CinemachineConfiner2D confiner = cam.GetComponent<CinemachineConfiner2D>();
            if(confiner != null)
            {
                PolygonCollider2D mapBounds = FindFirstObjectByType<PolygonCollider2D>();
                if(mapBounds != null)
                {
                    confiner.BoundingShape2D = mapBounds;
                    confiner.InvalidateBoundingShapeCache();
                }
            }
        }
    }
}