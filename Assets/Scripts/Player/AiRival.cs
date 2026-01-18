using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class AiRival : MonoBehaviour, Interactable
{
    // Public
    public Transform player;
    public float moveSpeed = 4f;
    public float sprintMoveSpeed = 9f;      // when player sprint, AI runs faster
    public float stoppingDistance = 1.5f;
    public Animator animator;

    // For interacting with AI
    [SerializeField] Dialog dialog;

    

    // Private
    private PlayerControl playerControl;    
    private float currentMoveSpeed;     // To switch between sprint or normal
    private Vector2 lastMoveDirection;
    private bool isInteracting = false;
    private bool isShutdown = false;
    private bool AiRestarted = false;


    public static AiRival Instance { get; private set; }
    [SerializeField] private GameObject InteractPrompt;
    
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
        currentMoveSpeed = moveSpeed;

        // If we just loaded into a new scene, teleport to player
        if (player != null && Vector3.Distance(transform.position, player.position) > 10f)
        {
            TeleportToPlayer();
        }

        
    }

    
    void Update()
    {

        // Don't process movement if we're shutdown or interacting
        if (isShutdown || isInteracting) return;


        if (player == null)
        {
            FindPlayer();
            if (player == null) return;
        }

        // Check if player is sprinting and adjust AI speed
        CheckPlayerSprint();



        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        bool shouldMove = distance > stoppingDistance;

        if (shouldMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, currentMoveSpeed * Time.deltaTime);
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
    

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void StopMovement()
    {
        animator.SetBool("isMoving", false);
    }

    // For restart AI
    public bool IsShutdown()
    {
        return isShutdown;
    }

    // ========== HELPER FUNCTIONS ==========

    // For interacting with AI (to shut down AI rival)
    public void Interact()
    {
        if (!isInteracting && !isShutdown)
        {
            StartCoroutine(InteractWithPlayer());
        }
    }

    public void ShowPrompt()
    {
        if (InteractPrompt != null && !isShutdown)
        {
            InteractPrompt.SetActive(true);
        }
    }

    public void HidePrompt()
    {
        if (InteractPrompt != null)
        {
            InteractPrompt.SetActive(false);
        }
    }

    private IEnumerator InteractWithPlayer()
    {
        isInteracting = true;
        StopMovement();

        // When AI just restarted don't prompt interaction
        if(AiRestarted == true)
        {
            AiRestarted = false;
            isInteracting = false; 
            yield break; 
        }

        // Show initial dialog
        yield return DialogManager.Instance.ShowDialog(dialog);

        // Show shutdown confirmation dialog
        yield return ShowShutdownConfirmation();

        isInteracting = false;
    }

    private IEnumerator ShowShutdownConfirmation()
    {
        // Create choices for shutdown confirmation
        List<string> choices = new List<string>
        {
            "Yes, dismiss him",
            "No, he can stay"
        };

        // Show dialog with choices using ShowDialogText
        yield return DialogManager.Instance.ShowDialogText(
            "Would you like to dismiss your Rival?\n",
            waitForInput: false,
            autoClose: false,
            choices: choices,
            onChoiceSelected: (choiceIndex) =>
            {
                if (choiceIndex == 0) // Player chose "Yes, shut down"
                {
                    ShutdownAI();
                }
                else
                {
                    // Manually close the dialog after choice is made
                    DialogManager.Instance.CloseDialog();
                }
            }
        );

        
    }

    private void ShutdownAI()
    {
        // Stop all movement and interactions
        StopMovement();
        isInteracting = true;
        isShutdown = true;

        // Play shutdown animation if available
        if (animator != null)
        {
            animator.SetTrigger("Shutdown");
            animator.SetBool("isMoving", false);
        }

        // Hide interact prompt
        HidePrompt();
        
        // Disable the AI after a delay to allow animation to play
        StartCoroutine(ShowShutdownMessage());
    }

    private IEnumerator ShowShutdownMessage()
    {

        // Show shutdown message
        yield return DialogManager.Instance.ShowDialogText(
            "RIVAL: Fine, go at it alone. I didn't want to be here anyway.",
            waitForInput: true,
            autoClose: true
        );

        yield return DialogManager.Instance.ShowDialogText(
            "Go to pause menu to reopen AI.",
            waitForInput: true,
            autoClose: true
        );

        gameObject.SetActive(false);
    }

    public void RestartAI()
    {
        if (isShutdown)
        {
            // If the GameObject was disabled, re-enable it
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
            }

            isShutdown = false;
            isInteracting = false;
            AiRestarted = true;

            // Reset animations
            if (animator != null)
            {
                animator.SetTrigger("Restart");
                animator.ResetTrigger("Shutdown");
            }

            // Teleport to player when restarting
            if (player != null)
            {
                TeleportToPlayer();
            }

            Debug.Log("AI Rival has been restarted ");

            // Show restart message
            StartCoroutine(ShowRestartMessage());

        }
    }

    public IEnumerator ShowRestartMessage()
    {
        yield return new WaitForSeconds(0.5f);  // prevent conflict in pause menu

        yield return DialogManager.Instance.ShowDialogText(
            "Your rival has returned.",
            waitForInput: true,
            autoClose: true
        );
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

    private void CheckPlayerSprint()
    {
        // Get reference to player control if we don't have it
        if (playerControl == null && player != null)
        {
            playerControl = player.GetComponent<PlayerControl>();
        }

        // If we have player control, check sprint state
        if (playerControl != null)
        {
            // Get isSprinting in PlayerControl, if yes then currentmove speed is now sprintMoveSpeed
            currentMoveSpeed = GetPlayerIsSprinting() ? sprintMoveSpeed : moveSpeed;

            //Make animations faster when sprinting
            animator.speed = GetPlayerIsSprinting() ? 1.3f : 1f;
        }
    }

    private bool GetPlayerIsSprinting()
    {
        // Get isSprinting in PlayerControl
        return playerControl.isSprinting;
    }

    public bool IsActive
    {
        get
        {
            return !isShutdown && gameObject.activeInHierarchy;
        }
    }

}