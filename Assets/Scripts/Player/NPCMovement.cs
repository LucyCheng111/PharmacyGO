using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;

public class NPCMovement : MonoBehaviour
{
    public Transform WaypointParent;
    public float moveSpeed = 2f; //will be tweaked in time
    public float waitTime = 2f; //how long the npc remains in place when reaching a waypoint
    public bool repeatMovement = true; //whether the npc will repeat the movement

    private Transform[] waypoints;
    private int currentWaypointIdx;
    private bool isWaiting;

    private Animator animator;

    void Start()
    {
        //get an array of waypoints
        waypoints = new Transform[WaypointParent.childCount];
        animator = GetComponent<Animator>();

        //determine how many child waypoints the waypoint parent has
        for(int i = 0; i < WaypointParent.childCount; i++)
        {
            waypoints[i] = WaypointParent.GetChild(i);
        }
    }

    void Update()
    {
        if (isWaiting)
        {
            //if the game is paused, or the npc is waiting at a waypoint, they do nothing
            return;        
        }

        MoveToWaypoint();
    }

    void MoveToWaypoint()
    {
        Transform target = waypoints[currentWaypointIdx];
        transform.position = Vector2.MoveTowards(transform.position,target.position, moveSpeed * Time.deltaTime);

        //convert world coordinates to movement directions on a unit circle
        Vector2 movementDirection = (target.position - transform.position).normalized;
 
        animator.SetFloat("moveX", movementDirection.x);
        animator.SetFloat("moveY", movementDirection.y);
        animator.SetBool("isMoving", true);

        if(Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        //handles when the npc reaches a waypoint and waits for some time before resuming their course if they are set to repeat their movements

        animator.SetBool("isMoving", false);
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);

        currentWaypointIdx = repeatMovement ? (currentWaypointIdx + 1) % waypoints.Length : Mathf.Min(currentWaypointIdx+1, waypoints.Length-1);
        isWaiting = false;
    }

}