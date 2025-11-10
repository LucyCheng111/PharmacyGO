using UnityEngine;
using UnityEngine.SceneManagement;

public class AiRival : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 4f;
    public float stoppingDistance = 1.5f;
    public Animator animator;

    private Vector2 lastMoveDirection;


    public static AiRival Instance { get; private set; }

    void Awake()
    {

        // Check if we're the first AI in the game
        if (Instance == null)
        {
            // First AI - set as singleton and persist
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else if (Instance != this)
        {
            // Duplicate AI - destroy this one
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        FindPlayer();

        // If we just loaded into a new scene, teleport to player
        if (player != null && Vector3.Distance(transform.position, player.position) > 10f)
        {
            TeleportToPlayer();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        // Wait for scene to fully load, then teleport to player
        StartCoroutine(TeleportToPlayerAfterDelay());
    }

    private System.Collections.IEnumerator TeleportToPlayerAfterDelay()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f); // Small delay for player to spawn

        FindPlayer();

        if (player != null)
        {
            TeleportToPlayer();
        }
    }

    private void TeleportToPlayer()
    {
        if (player == null) return;

        // Teleport to near the player (behind the player)
        Vector3 spawnOffset = new Vector3(1f, 0f, 0f);
        transform.position = player.position + spawnOffset;


    }



    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            if (player == null) return;
        }



        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        bool shouldMove = distance > stoppingDistance;

        if (shouldMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            Vector2 moveDirection = new Vector2(direction.x, direction.y).normalized;
            animator.SetFloat("moveX", moveDirection.x);
            animator.SetFloat("moveY", moveDirection.y);
            lastMoveDirection = moveDirection;
        }
        else
        {
            animator.SetFloat("moveX", lastMoveDirection.x);
            animator.SetFloat("moveY", lastMoveDirection.y);
        }

        animator.SetBool("isMoving", shouldMove);
    }

    private void FindPlayer()
    {


        if (PlayerControl.Instance != null)
        {
            player = PlayerControl.Instance.transform;
        }
        else
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("AI: Player not found!");
            }
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void StopMovement()
    {
        animator.SetBool("isMoving", false);
    }
}