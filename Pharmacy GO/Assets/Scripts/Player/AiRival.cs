using UnityEngine;

public class SimpleFollowAnim : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 4f;
    public float stoppingDistance = 1.5f;
    public Animator animator;

    private Vector2 lastMoveDirection;

    void Update()
    {
        if (player == null) return;

        // Calculate direction and distance to player
        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        // Only move if farther than stoppingDistance
        bool shouldMove = distance > stoppingDistance;

        if (shouldMove)
        {
            // movement towards player
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

            // Update animator parameters for facing direction
            Vector2 moveDirection = new Vector2(direction.x, direction.y).normalized;
            animator.SetFloat("moveX", moveDirection.x);
            animator.SetFloat("moveY", moveDirection.y);
            lastMoveDirection = moveDirection;
        }
        else
        {
            // When stopped, maintain last facing direction
            animator.SetFloat("moveX", lastMoveDirection.x);
            animator.SetFloat("moveY", lastMoveDirection.y);
        }

        animator.SetBool("isMoving", shouldMove);
    }

    // If want the AI to stop (like battles or other event)
    public void StopMovement()
    {
        animator.SetBool("isMoving", false);
    }
}