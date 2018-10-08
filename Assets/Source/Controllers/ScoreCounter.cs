using System;

public class ScoreCounter
{
    public event EventHandler ScoreUpdated;

    private Board _board;

    public int TotalScore { get; private set; }

    public ScoreCounter(Board board)
    {
        _board = board;
        _board.MatchResolved += OnMatchResolved;
    }

    ~ScoreCounter()
    {
        if (_board != null)
        {
            _board.MatchResolved -= OnMatchResolved;
        }
    }

    private void OnMatchResolved(object sender, MatchResolvedEventArgs e)
    {
        TotalScore += e.Match.Count;
        if (ScoreUpdated != null)
        {
            ScoreUpdated(this, EventArgs.Empty);
        }
    }
}