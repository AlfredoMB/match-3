using TMPro;
using UnityEngine;

public class TimerView : MonoBehaviour
{
    public TextMeshProUGUI Text;

    private GameTimer _gameTimer;

    public void Initialize(GameTimer gameTimer)
    {
        _gameTimer = gameTimer;
    }

    private void Update()
    {
        Text.text = string.Format("{0:0}", _gameTimer.RemainingTime);
    }
}
