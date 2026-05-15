using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class NPCEnemy : MonoBehaviour
{

    public float moveSpeed =2f;
    [SerializeField] GameObject exclaimation;
    [SerializeField] Dialog dialog;
    [SerializeField] GameObject FoV;
    [SerializeField] private int maxQuestions = 1; 
    [SerializeField] bool enemyDefeated;
    private Animator animator;
    private bool battleTriggered = false;

    private void Awake()
    { 
        animator = GetComponent<Animator>();
    }

    public IEnumerator TriggerBattle(PlayerControl player)
    {
        if (enemyDefeated || battleTriggered)
        {
            yield break;
        }

        battleTriggered = true;

        //flash the exclaimation on screen briefly when 
        exclaimation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclaimation.SetActive(false);
        
        //make the enemy walk towards the player
        float stopDistance = 1.0f; // tweak this
        Vector2 dirToPlayer = (player.transform.position - transform.position).normalized;
        Vector2 targetPosition = (Vector2)player.transform.position - dirToPlayer * stopDistance;
        yield return MoveToPlayer(targetPosition);

        //show Dialog
        yield return StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
        GameController.Instance.StartBattle(false, true, maxQuestions);

        Debug.Log("NPC waiting for battle end...");
        yield return StartCoroutine(WaitForBattleEnd());
        Debug.Log("NPC detected battle end!");
        
        gameObject.SetActive(false);
        Debug.Log("NPC Enemy Deactivated");
        //enemyDefeated = true;

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

    private IEnumerator WaitForBattleEnd()
    {
        //added this to prevent an occassional softlock when ending a battle caused by a race condition

        bool finished = false;
        
        void Handler()
        {
            finished = true;
        }

        //subscribe to the OnBattleOver event from the Battle System class
        GameController.Instance.OnBattleFinished += Handler;
        yield return new WaitUntil(()=> finished);
        GameController.Instance.OnBattleFinished -= Handler;
    }

    public void MarkDefeated()
    {
        enemyDefeated = true;
        
        if(FoV != null)
        {
            FoV.SetActive(false);
        }

        Collider2D collider = GetComponent<Collider2D>();
        if(collider != null)
        {
            collider.enabled = false;
        }

    }
}