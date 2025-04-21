using UnityEngine;
using UnityEngine.SceneManagement;
public enum GameState {FreeRoam, Battle, Dialogue}
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerControl playerControl;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;


    GameState state;

    public static GameController Instance { get; private set; }

    public void Awake()
    {
        // Check if there already is an existing GameController
        // Especially when returning to the Sample Scene
        if(Instance != null & Instance != this )
        {
            Destroy( gameObject );
            return;
        }
        Instance = this;
        DontDestroyOnLoad( gameObject );
    }

    private void Start()
    {
        //playerControl.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
        // playerControl.OnEnterDialogue += StartDialogue;
        // playerControl.OnEndDialogue += EndDialogue;

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialogue;
        };

        DialogManager.Instance.OnDialogFinished += () =>
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

    public void StartBattle(bool isBoss = false, int maxQuestions = 1)
    {
        MapArea localMapArea = FindFirstObjectByType<MapArea>();
        if(localMapArea != null)
        {
            battleSystem.SetMapData(localMapArea);
        }

        HudController localHud = FindFirstObjectByType<HudController>();
        if (localHud != null)
        {
            battleSystem.SetHudController(localHud);
        }

        Camera localCamera = Camera.main;
        if(localCamera != null)
        {
            worldCamera = localCamera;
        }

        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        playerControl.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(false);
        if (isBoss)
        {
            battleSystem.BossBattle(maxQuestions);
        }
        else
        {
            battleSystem.StartBattle();
        }  
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
