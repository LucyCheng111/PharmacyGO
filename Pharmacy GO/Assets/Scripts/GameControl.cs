using UnityEngine;
public enum GameState {FreeRoam, Battle, Dialogue}
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerControl playerControl;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

    GameState state;
    private void Start()
    {
        playerControl.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
        // playerControl.OnEnterDialogue += StartDialogue;
        // playerControl.OnEndDialogue += EndDialogue;
        
    }
    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        playerControl.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(false);
        battleSystem.StartBattle();
    }
    void EndBattle(bool playerWin)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        playerControl.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerControl.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
    }
}
