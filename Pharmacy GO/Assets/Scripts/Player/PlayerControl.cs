using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private Joystick joystick;

    public static PlayerControl Instance { get; private set; }

    public float moveSpeed;
    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;
    public LayerMask grassLayer;

    //public event Action OnEncountered;

    public int numberOfAreas = 4;  // Adjust based on the number of areas

    private Coroutine moveCoroutine;

    private bool isInEncounter = false;

    private bool isMoving;
    private Vector2 input;

    private Animator animator;

    private List<bool> areaTracker; // List of areas the player has triggered
    
    [SerializeField] private GameObject ExclamationMark;


    private void Awake()
    {
        areaTracker = new List<bool>(new bool[numberOfAreas]);  // Initializes all to false
   
        animator = GetComponent<Animator>();

        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate Player found! Destroying extra instance.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // DontDestroyOnLoad(gameObject);
        if (ExclamationMark != null)
        {
            ExclamationMark.SetActive(false);
        }
    }

    public void HandleUpdate()
    {
        if (!isMoving && !isInEncounter){ 
            // Get the input from the player
            float h = joystick.Horizontal;
            float v = joystick.Vertical;

            // If joystick is “centered,” read keyboard
            input.x = Mathf.Abs(h) > 0.1f ? h : Input.GetAxisRaw("Horizontal");
            input.y = Mathf.Abs(v) > 0.1f ? v : Input.GetAxisRaw("Vertical");


            if (input != Vector2.zero)
            {   
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                var targetPos = transform.position; // current position of the player
                targetPos.x += input.x;
                targetPos.y += input.y;
                
                if (IsWalkable(targetPos))
                    StartCoroutine(Move(targetPos));
                
            }
            animator.SetBool("isMoving", isMoving);

            if(Input.GetKeyDown(KeyCode.Z))
            {
                Interact();

            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                CoinManager.Instance.AddCoin(10);
                Debug.Log("Cheat activated: 10 coins added.");
            }
        }
    }

    void Interact()
    {
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPos = transform.position + facingDir; // the direction of the player facing

        // Debug.DrawLine(transform.position, interactPos, Color.red, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        moveCoroutine = StartCoroutine(MoveCoroutine(targetPos)); // Store the coroutine reference
        yield return moveCoroutine; // Wait for the coroutine to finish
    }

    private IEnumerator MoveCoroutine(Vector3 targetPos)
    {
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) 
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;

        CheckForEncounters();
        
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.0f, solidObjectsLayer | interactableLayer) != null)
        {
            return false;
        }
        return true;
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.0f, grassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 50)
            {
                animator.SetBool("isMoving", false);
                StartCoroutine(ShowExclamationAndEncounter());

            }
        }
        else if (MapArea.i.IsDangerous())
        {
            if (UnityEngine.Random.Range(1, 101) <= 5)
            {
                animator.SetBool("isMoving", false);
                StartCoroutine(ShowExclamationAndEncounter());
            }
        }
    }


    private IEnumerator ShowExclamationAndEncounter()
    {
        isInEncounter = true;  

        ExclamationMark.SetActive(true); 

        // Flashing effect
        float flashDuration = 1.0f;  
        float flashInterval = 0.1f;  
        float timer = 0f; 

        while (timer < flashDuration)
        {
            ExclamationMark.SetActive(!ExclamationMark.activeSelf); 
            timer += flashInterval;
            yield return new WaitForSeconds(flashInterval);
        }


        ExclamationMark.SetActive(true); 
        yield return new WaitForSeconds(0.5f);  

        Debug.Log("Hiding Exclamation Mark"); 
        ExclamationMark.SetActive(false);  
        GameController.Instance.StartBattle();  
        isInEncounter = false;  
    }

    // New-map
    public void StopMovement()
    {
        // Stop any running movement coroutine
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);  // Stop the specific movement coroutine
            moveCoroutine = null;  // Reset the coroutine reference
        }

        isMoving = false;
        input = Vector2.zero;

        // Force Unity's Input System to Clear
        Input.ResetInputAxes();

        // Force Stop the Animator
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", 0);
        animator.SetBool("isMoving", false);
    }

    public void SetAreaTracker(int areaIndex)
    {
        if (areaIndex >= 0 && areaIndex < areaTracker.Count)
        {
            areaTracker[areaIndex] = true;
            Debug.Log("Player triggered area: " + areaIndex);
        }
    }

    public bool HasTriggeredArea(int areaIndex)
    {
        return areaIndex >= 0 && areaIndex < areaTracker.Count && areaTracker[areaIndex];
    }




}