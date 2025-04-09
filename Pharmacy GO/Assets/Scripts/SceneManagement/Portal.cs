using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class Portal : MonoBehaviour
{
    public int sceneBuildIndex; // The target scene's build index
    public string targetSpawnPointID; // The ID of the target spawn point

    private bool isTransitioning = false;
    private Collider2D portalCollider;

    private void Awake()
    {
        portalCollider = GetComponent<Collider2D>();
    }

    private IEnumerator Start()
    {
        portalCollider.enabled = false;
        yield return new WaitForSeconds(2f);
        portalCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(TransitionToScene());
        }
    }

    private IEnumerator TransitionToScene()
    {
        // Save the target spawn point ID before scene transition
        PlayerPrefs.SetString("SpawnPointID", targetSpawnPointID);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneBuildIndex);
        yield return new WaitUntil(() => operation.isDone);

        yield return new WaitForSeconds(0.1f); // Delay for scene initialization

        // Fix Cinemachine
        StartCoroutine(UpdateCinemachineTarget());

        isTransitioning = false;
    }

    private IEnumerator UpdateCinemachineTarget()
    {
        yield return new WaitForSeconds(0.2f); // Small delay to ensure the player loads

        CinemachineCamera cam = FindFirstObjectByType<CinemachineCamera>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (cam != null && player != null)
        {
            cam.Follow = player.transform;

            CinemachineConfiner2D confiner = cam.GetComponent<CinemachineConfiner2D>();
            if (confiner != null)
            {
                PolygonCollider2D mapBounds = FindFirstObjectByType<PolygonCollider2D>();
                if (mapBounds != null)
                {
                    confiner.BoundingShape2D = mapBounds;
                    confiner.InvalidateBoundingShapeCache();
                }
            }
        }
    }

    private SpawnPoint FindTargetSpawnPoint()
    {
        SpawnPoint[] spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint.spawnPointID == targetSpawnPointID)
            {
                return spawnPoint;
            }
        }
        return null;
    }
}
