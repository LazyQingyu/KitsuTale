using TMPro;
using UnityEngine;

public class CoinPanel : MonoBehaviour
{
    void Start()
    {
        Level.current_level.player.OnScoreChanged.AddListener(UpdateScore);
        GetComponent<TextMeshProUGUI>().text = GameManager.instance.scoreSaved.ToString();

    }

    
    public void UpdateScore( int score)
    {
        GetComponent<TextMeshProUGUI>().text = score.ToString();
    }
}
