﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{

    // Controller for the player object
    // Also handles checks when moving into objects

    [SerializeField] private Joystick joystick;

    public static PlayerControl Instance { get; private set; }

    public float moveSpeed =5f;
    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;
    public LayerMask grassLayer;

    //public event Action OnEncountered;

    public int numberOfAreas = 4;  // Adjust based on the number of areas

    private Coroutine moveCoroutine;

    private bool isInEncounter = false;
    private bool noClipEnabled = false;

    private bool isMoving;
    private bool isSprinting;
    private Vector2 input;

    private Animator animator;

    private Collider2D activePrompt;

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
        if (isInEncounter) return;

            // Get the input from the player
            float h = joystick.Horizontal;
            float v = joystick.Vertical;

            // If joystick is “centered,” read keyboard
            input.x = Mathf.Abs(h) > 0.1f ? h : Input.GetAxisRaw("Horizontal");
            input.y = Mathf.Abs(v) > 0.1f ? v : Input.GetAxisRaw("Vertical");

            //normalize diagonal movement to prevent faster movement when moving at an angle
            if (input.magnitude > 1f){
                input.Normalize();
            }

            //check if the player is sprinting

            isSprinting = Input.GetKey(KeyCode.LeftShift);
            float currentSpeed = isSprinting  ? moveSpeed * 2.0f: moveSpeed;
            //note to self: this is the ternary conditional operator, basically:
            // condition ? ifTrue : ifFalse;

            //apply movement transformation
            Vector3 movementVector = new Vector3(input.x,input.y,0f) * currentSpeed * Time.deltaTime;


            //when we add a sprinting animation we will add the logic for it here

            if(movementVector != Vector3.zero && IsWalkable(transform.position + movementVector)){
                transform.position += movementVector;
                isMoving = true;
                PromptCheck();

            }

            if(input.magnitude > 0.1f){
                            
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                animator.SetBool("isMoving",true);
            }else{
                animator.SetBool("isMoving",false);

            }

            //isMoving = false;
            //animator.SetBool("isMoving",isMoving);
            CheckForEncounters();
            PromptCheck();


            /*
            if (input != Vector2.zero)
            {   
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                var targetPos = transform.position; // current position of the player

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    isSprinting = true;
                }
                else
                {
                    isSprinting = false;
                }

                targetPos.x += input.x;
                targetPos.y += input.y;
                PromptCheck();

                if (IsWalkable(targetPos))
                    StartCoroutine(Move(targetPos));
                
            }
            */
            //animator.SetBool("isMoving", isMoving);

            if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                Interact();

            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.L))
            {
                CoinManager.Instance.AddCoin(10);
                Debug.Log("Cheat activated: 10 coins added.");
            }


            //noclip cheat for debug purposes
            if(Input.GetKeyDown(KeyCode.N)){
                if(noClipEnabled = true){
                    Debug.Log("Cheat deactivated: NoClip Disabled");
                    noClipEnabled = false;
                }else{
                    Debug.Log("Cheat activated: NoClip Enabled");
                    noClipEnabled = true;
                }

            }
#endif
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

    void PromptCheck()
    {
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPos = transform.position + facingDir; // the direction of the player facing

        // Debug.DrawLine(transform.position, interactPos, Color.red, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.ShowPrompt();
            activePrompt = collider;
        }
    }

    void HidePrompt()
    {
        if (activePrompt != null)
        {
            activePrompt.GetComponent<Interactable>()?.HidePrompt();
            activePrompt = null;
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        HidePrompt();
        moveCoroutine = StartCoroutine(MoveCoroutine(targetPos)); // Store the coroutine reference
        yield return moveCoroutine; // Wait for the coroutine to finish
    }

    private IEnumerator MoveCoroutine(Vector3 targetPos)
    {
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) 
        {
            float currentSpeed = isSprinting ? moveSpeed * 2.0f : moveSpeed;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, currentSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;

        CheckForEncounters();
        PromptCheck();

    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if(noClipEnabled){
            return true;
        }

        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer | interactableLayer) != null)
        {
            return false;
        }
        return true;
    } 

    private void CheckForEncounters()
    {
        // skip battle if no question in this area
        var mapArea = FindFirstObjectByType<MapArea>();
        if (mapArea == null || !mapArea.HasQuestions())
        {
            return;
        }
        if (Physics2D.OverlapCircle(transform.position, 0.0f, grassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 50)
            {
                animator.SetBool("isMoving", false);
                StartCoroutine(ShowExclamationAndEncounter());

            }
        }
        else if (MapArea.i.IsDangerous() && !GameController.Instance.IsCurrentLevelBossDefeated())
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