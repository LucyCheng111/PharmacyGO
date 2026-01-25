using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{

    // Handles score display game object

    [SerializeField] private TMP_Text scoreText;

    private void Update()
    {
        scoreText.text = "Score: " + ScoreManager.Instance.GetScoreCount();
    }
}
