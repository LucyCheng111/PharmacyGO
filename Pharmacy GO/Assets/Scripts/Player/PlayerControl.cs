using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{

    public static PlayerControl Instance { get; private set; }

    public float moveSpeed;
    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;
    public LayerMask grassLayer;

    public event Action OnEncountered;

    private bool isMoving;
    private Vector2 input;

    private Animator animator;

    private void Awake()
    {
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
    }

    public void HandleUpdate()
    {
        if (!isMoving){ 
            // Get the input from the player
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

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
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) // while the distance between the player and the target position is greater than 0
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
                OnEncountered();
            }
        }
    }


    // New-map
    public void StopMovement()
    {
        // Stop any running movement coroutine
        StopAllCoroutines();

        isMoving = false;
        input = Vector2.zero;

        // Force Unity's Input System to Clear
        Input.ResetInputAxes();

        // Force Stop the Animator
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", 0);
        animator.SetBool("isMoving", false);
    }

    



}