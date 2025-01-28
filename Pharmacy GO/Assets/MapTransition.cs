using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class MapTransition : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D mapBoundry;
    [SerializeField] private Direction direction;

    private CinemachineConfiner2D confiner;
    public CinemachineCamera vcam;

    private enum Direction { Up, Down, Left, Right }

    private bool isTransitioning = false; // Prevent multiple triggers

    private void Awake()
    {
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
        vcam = FindAnyObjectByType<CinemachineCamera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTransitioning) // Prevent multiple triggers
        {
            StartCoroutine(TransitionPlayer(collision.gameObject));
        }
    }

    private IEnumerator TransitionPlayer(GameObject player)
    {
        isTransitioning = true; // Prevent re-triggering

        if (confiner != null)
        {
            confiner.BoundingShape2D = mapBoundry;
            confiner.InvalidateBoundingShapeCache();
        }

        if (vcam != null)
        {
            vcam.Follow = player.transform;
            vcam.ForceCameraPosition(player.transform.position, Quaternion.identity);
        }

        // Disable player's collider temporarily
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        // Move the player
        UpdatePlayerPosition(player);

        yield return new WaitForSeconds(0.1f); // Small delay

        //  STOP MOVEMENT IMMEDIATELY
        PlayerControl playerControl = player.GetComponent<PlayerControl>();
        if (playerControl != null)
        {
            playerControl.StopMovement();
        }

        yield return new WaitForSeconds(0.5f); // Small delay before re-triger the EnterPoint

        // Re-enable the player's collider
        if (playerCollider != null)
        {
            playerCollider.enabled = true;
        }

        isTransitioning = false; // Allow future transitions
    }


    private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 newPos = player.transform.position;

        switch (direction)
        {
            case Direction.Up:
                newPos.y += 5;
                break;
            case Direction.Down:
                newPos.y -= 5;
                break;
            case Direction.Left:
                newPos.x -= 5;
                break;
            case Direction.Right:
                newPos.x += 5;
                break;
        }
        player.transform.position = newPos;
    }
}
