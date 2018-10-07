using System;
using UnityEngine;

public class Match3GameController : MonoBehaviour
{
    public int Width = 8;
    public int Height = 8;
    public int MinMatchSize = 3;

    public BoardView BoardView;
    public ScoreView ScoreView;
    public TimerView TimerView;
    public GameOverView GameOverView;

    private Board _board;
    private Match3Game _game;
    private GameTimer _gameTimer;
    private ScoreCounter _score;

    private void Awake()
    {
        _board = new Board(Width, Height, MinMatchSize, new[] { 0, 1, 2, 3, 4 }, 0);// UnityEngine.Random.Range(int.MinValue, int.MaxValue));
        _board.RandomFillUp();

        _game = new Match3Game(_board);

        BoardView.Initialize(_board);
        BoardView.SwapComplete += OnSwapComplete;
        BoardView.RemoveComplete += OnRemoveComplete;
        BoardView.MovingPiecesDownComplete += OnMovingPiecesDownComplete;

        _gameTimer = new GameTimer();
        _gameTimer.SetTime(60f);
        TimerView.Initialize(_gameTimer);

        _score = new ScoreCounter(_board);
        ScoreView.Initialize(_score);
    }

    private void Update()
    {
        _gameTimer.UpdateTimePassed(Time.deltaTime);
        if (_gameTimer.RemainingTime <= 0)
        {
            GameOverView.Show();
            enabled = false;
        }
    }

    private void OnDisable()
    {
        if (BoardView == null)
        {
            return;
        }
        BoardView.SwapComplete -= OnSwapComplete;
        BoardView.RemoveComplete -= OnRemoveComplete;
        BoardView.MovingPiecesDownComplete -= OnMovingPiecesDownComplete;
    }

    private void OnSwapComplete(object sender, EventArgs e)
    {
        _game.Process(); // to run WaitingToSwap
        _game.Process(); // to run CheckingMatches
        _game.Process(); // to run SwappingBack or RemovingMatchPieces
    }

    private void OnRemoveComplete(object sender, EventArgs e)
    {
        _game.Process(); // to run MovingDownPieces
    }

    private void OnMovingPiecesDownComplete(object sender, EventArgs e)
    {
        _game.Process(); // to run CheckingMatches
        _game.Process(); // to run WaitingToSwap or RemovingMatchPieces
    }
}
