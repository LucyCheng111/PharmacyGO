using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public enum GameState {FreeRoam, Battle, Dialogue, Encounter, Minigame}
public class GameController : MonoBehaviour
{

    // Main controller for the game state
    // Acts as a hub for multiple intersecting components

    [SerializeField] PlayerControl playerControl;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;


    public GameState state;

    public static GameController Instance { get; private set; }
    private HashSet<string> defeatedBossLevels = new HashSet<string>();

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

        SceneManager.sceneLoaded += OnSceneLoaded;

        if(LevelManager.Instance.continuing == true)
        {
            UnityEngine.Debug.Log("Continuing game, loading save");
            GetComponent<saveManager>().LoadGame();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // grab the unlocked level and buildIndex from LevelManager
        int lvl = LevelManager.Instance.UnlockedLevel;
        int idx = LevelManager.Instance.GetCurrentBuildIndex();
        Debug.Log($"[GameController] Scene \"{scene.name}\" loaded �� LevelNumber = {lvl}, BuildIndex = {idx}");
    }


private void Start()
    {
        //playerControl.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;
        // playerControl.OnEnterDialogue += StartDialogue;
        // playerControl.OnEndDialogue += EndDialogue;

        playerControl.OnEnemyEncountered += (Collider2D enemyCollider) =>
        {
            var enemy = enemyCollider.GetComponentInParent<NPCEnemy>();
            if(enemy != null)
            {
                state = GameState.Encounter;
                StartCoroutine(enemy.TriggerBattle(playerControl));
            }
        };

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

    public void StartBattle(bool isBoss = false, bool isEnemy=false, int maxQuestions = 1)
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
        else if (isEnemy)
        {
            battleSystem.EnemyBattle(maxQuestions);
        }
        else
        {
            battleSystem.StartBattle();
        }  
    }

    void EndBattle(bool playerWin)
    {
        StartCoroutine(EndBattleRoutine());

        // state = GameState.FreeRoam;
        // Debug.Log("GameState == " + state);
        // //yield return null;
        // battleSystem.gameObject.SetActive(false);
        // playerControl.gameObject.SetActive(true);
        // worldCamera.gameObject.SetActive(true);

        // NPCEnemy enemy = FindFirstObjectByType<NPCEnemy>();
        // if (enemy!= null){
        //     enemy.MarkDefeated();
        // }        
    }

    IEnumerator EndBattleRoutine()
    {
        state = GameState.FreeRoam;
        Debug.Log("GameState == " + state);
        yield return null;
        battleSystem.gameObject.SetActive(false);
        playerControl.gameObject.SetActive(true);
        //reset the player's encounter timer
        PlayerControl.Instance.RestartEncounterCooldown();
        worldCamera.gameObject.SetActive(true);
    }

    // checks if boss is defeated
    public void MarkBossDefeated()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        defeatedBossLevels.Add(sceneName);
        Debug.Log($"Boss in {sceneName} marked as defeated");
    }

    public bool IsCurrentLevelBossDefeated()
    {
        return defeatedBossLevels.Contains(SceneManager.GetActiveScene().name);
    }

    public void StartMinigame()
    {
        state = GameState.Minigame;
    }

    public void EndMinigame()
    {
        state = GameState.FreeRoam;
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
        else if (state == GameState.Minigame)
        {
            //no update for it
        }

        saveManager.Instance.HandleUpdate(); //used for autosave

    }
}
