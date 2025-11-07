using UnityEngine;
using UnityEngine.AI;

public class AIFollow : MonoBehaviour
{
    public Transform player;          // drag your Player object here in Unity
    private NavMeshAgent agent;       // the AI’s walking 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Constantly tell the AI to go where the player is
        agent.SetDestination(player.position);
    }
}