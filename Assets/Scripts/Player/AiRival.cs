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
    
    //make sure the ai rival leaves enough space between itself and the player so the player
    //does not get stuck between a wall and the ai rival
    public float stoppingDistance = 2.5f;
    [SerializeField] private float minDistanceFromPlayer = 2.1f;
    
    public Animator animator;

    // For interacting with AI
    [SerializeField] Dialog dialog;

    // Avoid collision
    [SerializeField] private LayerMask solidObjectsLayer;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float collisionCheckRadius = 0.2f;



    // Private
    private PlayerControl playerControl;
    private NPCMinigamePlayer minigamePlayer;   // minigame
    private float currentMoveSpeed;     // To switch between sprint or normal
    private Vector2 lastMoveDirection;
    private bool isInteracting = false;
    private bool isShutdown = false;
    private bool AiRestarted = false;
    enum AiPointState
    {
        Win,
        Even,
        Lose
    };
    AiPointState aiPointState;







    public static AiRival Instance { get; private set; }
    [SerializeField] private GameObject InteractPrompt;
    
    void Awake()
    {
        minigamePlayer = GetComponent<NPCMinigamePlayer>();

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
            Vector2 moveDirection = direction.normalized;
            Vector3 nextPos = Vector3.MoveTowards(
                transform.position,
                player.position,
                currentMoveSpeed * Time.deltaTime
            );

            //Keep the AI a comfortable distance from the player to prevent softlock
            float nextDistanceToPlayer = Vector3.Distance(nextPos,player.position);
            if(nextDistanceToPlayer < minDistanceFromPlayer)
            {
                nextPos = transform.position;
                shouldMove=false;
            }
            // Avoid solid objects
            else if (IsWalkable(nextPos))
            {
                transform.position = nextPos;
            }
            else
            {
                // Try sliding along obstacles
                // Try moving only in X direction
                Vector3 slideX = new Vector3(nextPos.x, transform.position.y, transform.position.z);
                if (IsWalkable(slideX))
                {
                    transform.position = slideX;
                }
                else
                {
                    // Try moving only in Y direction
                    Vector3 slideY = new Vector3(transform.position.x, nextPos.y, transform.position.z);
                    if (IsWalkable(slideY))
                    {
                        transform.position = slideY;
                    }
                }
            }

            animator.SetFloat("moveX", moveDirection.x);
            animator.SetFloat("moveY", moveDirection.y);
            lastMoveDirection = moveDirection;
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
    public void Interact(Transform initiator)
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

        // Show  confirmation dialog
        yield return ShowConfirmation();

        isInteracting = false;
    }

    private IEnumerator ShowConfirmation()
    {
        int aiScore = ScoreManager.Instance.GetAiRivalScore();
        int playerScore = ScoreManager.Instance.GetScoreCount();

        Debug.Log($"AI score: {aiScore}, player score: { playerScore}");

        SetAiState(aiScore, playerScore);

        bool showingChat = false;
        bool showingMiniGame = false;

        // Create choices for shutdown confirmation
        List<string> choices = new List<string>
        {
            "Chat",
            "Mini Game",
            "Close the AI"
            
        };

        // Show dialog with choices using ShowDialogText
        yield return DialogManager.Instance.ShowDialogText(
            "RIVAL: What do you want?\n",
            waitForInput: false,
            autoClose: false,
            choices: choices,
            onChoiceSelected: (choiceIndex) =>
            {
                if (choiceIndex == 0)   // chat 
                {
                    showingChat = true;
                }
                else if (choiceIndex == 1)
                {
                    showingMiniGame = true;
                }
           
            }
        );
        if (showingChat)
        {
            if (aiPointState == AiPointState.Lose)
            {
                yield return ShowLoseMessage();
            }
            else if (aiPointState == AiPointState.Win)
            {
                yield return ShowWinMessage();
            }
            else if (aiPointState == AiPointState.Even)
            {
                yield return ShowEvenMessage();
            }
        }
        else if (showingMiniGame)
        {
            yield return showMiniGameOptions();
        }
        else
        {
            // Show shutdown confirmation, choose close AI
            yield return ShowShutdownConfirmation();
        }
    }

    private IEnumerator ShowShutdownConfirmation()
    {
        // Create choices for shutdown confirmation
        List<string> choices = new List<string>
        {
            "Yes, dismiss them",
            "No, they can stay"
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

    // New
    private IEnumerator showMiniGameOptions()
    {
        // Create choices for mini games
        List<string> choices = new List<string>
        {
            "PlayCards",
            "option 2 (not yet)",
            "Nah"
        };

        // Show dialog with choices using ShowDialogText
        yield return DialogManager.Instance.ShowDialogText(
            "What game would you like to play?\n",
            waitForInput: false,
            autoClose: false,
            choices: choices,
            onChoiceSelected: (choiceIndex) =>
            {
                if (choiceIndex == 0) // Player chose "playcards
                {
                    // Open the minigame
                    if (minigamePlayer != null)
                        minigamePlayer.StartMinigame();
                    else
                        Debug.LogWarning("AiRival: No NPCMinigamePlayer component found!");
                }
                // Can add other minigames later
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
        Vector3 spawnOffset = new Vector3(2.5f, 0f, 0f);
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

    // collision checking function
    private bool IsWalkable(Vector3 targetPos)
    {
        if (isShutdown) return false;

        return Physics2D.OverlapCircle(
            targetPos,
            collisionCheckRadius,
            solidObjectsLayer | interactableLayer
        ) == null;
    }

    void SetAiState(int AIScore, int PlayerScore)
    {
        if (AIScore > PlayerScore)
        {
            aiPointState = AiPointState.Win;
        }

        if (AIScore == PlayerScore)
        {
            aiPointState = AiPointState.Even;
        }

        if (AIScore <  PlayerScore)
        {
            aiPointState = AiPointState.Lose;
        }
    }

    private IEnumerator ShowWinMessage()
    {

        // Show shutdown message
        yield return DialogManager.Instance.ShowDialogText(
            "RIVAL: Haha, I'm better than you!",
            waitForInput: true,
            autoClose: true
        );

        yield return DialogManager.Instance.ShowDialogText(
            "RIVAL: Better Luck next time, pal",
            waitForInput: true,
            autoClose: true
        );
    }

    private IEnumerator ShowEvenMessage()
    {

        // Show shutdown message
        yield return DialogManager.Instance.ShowDialogText(
            "RIVAL: We're a tie right now, but soon, I'll beat you",
            waitForInput: true,
            autoClose: true
        );
    }

    private IEnumerator ShowLoseMessage()
    {

        // Show shutdown message
        yield return DialogManager.Instance.ShowDialogText(
            "RIVAL: Are you making fun of me just because I'm a little bit behind you?",
            waitForInput: true,
            autoClose: true
        );

        yield return DialogManager.Instance.ShowDialogText(
            "RIVAL: Laugh now, before you can't anymore",
            waitForInput: true,
            autoClose: true
        );
    }



}