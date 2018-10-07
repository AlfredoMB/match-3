using System;

public class ScoreCounter
{
    public event EventHandler ScoreUpdated;

    private Board _board;

    public int TotalScore { get; private set; }

    public ScoreCounter(Board board)
    {
        _board = board;
        _board.MatchesFound += OnMatchesFound;
    }

    ~ScoreCounter()
    {
        if (_board != null)
        {
            _board.MatchesFound -= OnMatchesFound;
        }
    }

    private void OnMatchesFound(object sender, MatchesFoundEventArgs e)
    {
        foreach(var match in e.Matches)
        {
            TotalScore += match.Count;
        }
        if (ScoreUpdated != null)
        {
            ScoreUpdated(this, EventArgs.Empty);
        }
    }
}