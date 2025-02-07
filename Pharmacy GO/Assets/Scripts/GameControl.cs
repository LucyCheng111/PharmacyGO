using UnityEngine;
using UnityEngine.SceneManagement;
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

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialogue;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if(state == GameState.Dialogue)
                state = GameState.FreeRoam;
        };

        if(CoinManager.Instance == null)
        {
            GameObject coinManager = new GameObject("CoinManager");
            coinManager.AddComponent<CoinManager>();
        }

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

        //Debug.Log($"Game State: {state}");

        if (state == GameState.FreeRoam)
        {
            playerControl.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialogue)
        {
            DialogManager.Instance.HandleUpdate();
        }


    }
}
