using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class Portal : MonoBehaviour
{

    // Controls portal object and collider
    // Calls appropriate functions when player steps into portal

    public int sceneBuildIndex; // The target scene's build index
    public int levelNumber;
    public string targetSpawnPointID; // The ID of the target spawn point

    private bool isTransitioning = false;
    private Collider2D portalCollider;
    // private ParticleSystem portalEffect;
    [SerializeField] private ParticleSystem portalEffect;
    


    // run time, it makes newly unlocked portal immediately able to use
    // without reloading the Hub scene
    public void RefreshPortalEffect()
    {
        var emission = portalEffect.emission;
        emission.enabled = levelNumber <= LevelManager.Instance.UnlockedLevel;
    }

    private void Awake()
    {
        portalCollider = GetComponent<Collider2D>();
        portalEffect = GetComponentInChildren<ParticleSystem>(true);
        if (portalEffect == null)
        {
            Debug.LogError($"[{name}] No child ParticleSystem found on this portal!");
        }
            
    }

    private IEnumerator Start()
    {
        

        portalCollider.enabled = false;
        yield return new WaitForSeconds(2f);
        portalCollider.enabled = true;
        RefreshPortalEffect();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            // locked levels not able to teleport
            if (levelNumber > LevelManager.Instance.UnlockedLevel)
            {
                Debug.Log($"Level {levelNumber} is locked");
                return;
            }

            isTransitioning = true;
            StartCoroutine(TransitionToScene());

            
            if (levelNumber == LevelManager.Instance.UnlockedLevel)
            {
                if (!TimerManager.Instance.IsLevelStarted())
                    TimerManager.Instance.ResetTimer();
                TimerManager.Instance.StartTimer();
            }
        }
    }

    private IEnumerator TransitionToScene()
    {
        // Save the target spawn point ID before scene transition
        PlayerPrefs.SetString("SpawnPointID", targetSpawnPointID);

        // AsyncOperation operation = SceneManager.LoadSceneAsync(sceneBuildIndex);
        AsyncOperation operation = LevelManager.Instance.LoadLevel(levelNumber);
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
