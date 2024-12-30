using TMPro;
using UnityEngine;

public class CoinDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;

    private void Update()
    {
        coinText.text = "Coins: " + CoinManager.Instance.GetCoinCount();
    }
}
