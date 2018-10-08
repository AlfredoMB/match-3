using System;
using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI MultiplierText;

    private ScoreCounter _score;

    public void Initialize(ScoreCounter score)
    {
        _score = score;
        _score.ScoreUpdated += OnScoreUpdated;
    }

    private void OnEnable()
    {
        OnScoreUpdated(null, null);
    }

    private void OnScoreUpdated(object sender, EventArgs e)
    {
        if (ScoreText != null)
        {
            ScoreText.text = _score.TotalScore.ToString();
        }
        if (MultiplierText != null)
        {
            MultiplierText.text = _score.TotalMultiplier.ToString();
        }
    }
}