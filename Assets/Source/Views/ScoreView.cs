using System;
using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    public TextMeshProUGUI Text;

    private ScoreCounter _score;

    public void Initialize(ScoreCounter score)
    {
        _score = score;
        _score.ScoreUpdated += OnScoreUpdated;
    }

    private void OnScoreUpdated(object sender, EventArgs e)
    {
        Text.text = _score.TotalScore.ToString();
    }
}