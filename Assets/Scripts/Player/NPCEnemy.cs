using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class NPCEnemy : MonoBehaviour
{

    public float moveSpeed =2f;
    [SerializeField] GameObject exclaimation;
    [SerializeField] Dialog dialog;
    [SerializeField] private int maxQuestions = 1; 
    [SerializeField] private bool enemyDefeated;
    private Animator animator;
    private bool battleTriggered = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public IEnumerator TriggerBattle(PlayerControl player)
    {
        if (battleTriggered)
        {
            yield break;
        }

        battleTriggered = true;

        //flash the exclaimation on screen briefly when 
        exclaimation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclaimation.SetActive(false);
        
        //make the enemy walk towards the player
        float stopDistance = 0.8f; // tweak this
        Vector2 dirToPlayer = (player.transform.position - transform.position).normalized;
        Vector2 targetPosition = (Vector2)player.transform.position - dirToPlayer * stopDistance;
        yield return MoveToPlayer(targetPosition);

        //show Dialog
        yield return StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
        GameController.Instance.StartBattle(false, true, maxQuestions);
    }

    private IEnumerator MoveToPlayer(Vector2 targetPosition)
    {
        animator.SetBool("isMoving", true);

        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

            animator.SetFloat("moveX", direction.x);
            animator.SetFloat("moveY", direction.y);

            transform.position = Vector2.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        animator.SetBool("isMoving", false);
    }
}