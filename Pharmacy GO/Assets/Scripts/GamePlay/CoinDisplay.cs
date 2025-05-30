using TMPro;
using UnityEngine;

public class CoinDisplay : MonoBehaviour
{

    // Handles coin display game object
    [SerializeField] private TMP_Text coinText;

    private void Update()
    {
        coinText.text = "Coins: " + CoinManager.Instance.GetCoinCount();
    }
}
