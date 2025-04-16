using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct LevelEntry
{
    public int levelNumber;
    public int buildIndex;
}

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance { get; private set; }
    public int UnlockedLevel { get; private set; } = 1;

    [SerializeField] List<LevelEntry> levelMap = new List<LevelEntry>();


    // 5 total levels
    // [SerializeField] private int maxLevel = 5;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            UnlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 2);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void UnlockNextLevel()
    {
        if (UnlockedLevel < levelMap.Count)
        {
            UnlockedLevel++;
            PlayerPrefs.SetInt("UnlockedLevel", UnlockedLevel);
            PlayerPrefs.Save();
        }
        
        foreach (var portal in FindObjectsOfType<Portal>())
            portal.RefreshPortalEffect();
    }

    public AsyncOperation LoadLevel(int levelNumber)
    {
        if (levelMap == null || levelMap.Count == 0)
        {
            Debug.LogError("LevelManager: levelMap is empty! Populate it in the Inspector.");
            return null;
        }

        var phgoMap = levelMap.Find(e => e.levelNumber == levelNumber);
        if (phgoMap.buildIndex >= 0)
            return SceneManager.LoadSceneAsync(phgoMap.buildIndex);
        else
            Debug.LogError($"No buildIndex mapped for level {levelNumber}");

        return null;
    }
}
