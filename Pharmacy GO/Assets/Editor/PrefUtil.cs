using UnityEditor;
using UnityEngine;

public static class PrefUtils
{
    [MenuItem("Tools/Clear UnlockedLevel")]
    public static void ClearUnlockedLevel()
    {
        PlayerPrefs.DeleteKey("UnlockedLevel");
        Debug.Log("[DEBUG] PlayerPrefs key 'UnlockedLevel' deleted.");
    }
}
