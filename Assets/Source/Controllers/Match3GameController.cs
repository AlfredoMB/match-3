using System;
using System.Collections.Generic;
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
    private GameTimer _gameTimer;
    private ScoreCounter _score;

    private void Awake()
    {
        _board = new Board(Width, Height, MinMatchSize, new HashSet<int> { 0, 1, 2, 3, 4 }, 0);// UnityEngine.Random.Range(int.MinValue, int.MaxValue));
        _board.RandomFillUp();

        BoardView.Initialize(_board);
        BoardView.AllPiecesFell += OnAllPiecesFell;

        _gameTimer = new GameTimer();
        _gameTimer.SetTime(60f);
        TimerView.Initialize(_gameTimer);

        _score = new ScoreCounter(_board);
        ScoreView.Initialize(_score);

        enabled = false;
    }

    private void OnDestroy()
    {
        BoardView.AllPiecesFell -= OnAllPiecesFell;
    }

    private void OnAllPiecesFell(object sender, EventArgs e)
    {
        enabled = true;
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
}
