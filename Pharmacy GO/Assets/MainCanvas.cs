using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    [SerializeField] private GameObject menu;    
    private static MainCanvas instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
