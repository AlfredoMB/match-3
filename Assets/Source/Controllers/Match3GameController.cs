using System;
using UnityEngine;

public class Match3GameController : MonoBehaviour
{
    public int Width = 8;
    public int Height = 8;
    public int MinMatchSize = 3;

    public BoardView BoardView;

    private Board _board;
    private Match3Game _game;

    private void Awake()
    {
        _board = new Board(Width, Height, MinMatchSize);
        _board.RandomFillUp(UnityEngine.Random.Range(int.MinValue, int.MaxValue), 0, 1, 2, 3, 4);

        _game = new Match3Game(_board);

        BoardView.Initialize(_board);
        BoardView.SwapComplete += OnSwapComplete;
    }

    private void OnSwapComplete(object sender, EventArgs e)
    {
        _game.Process(); // to check
        _game.Process(); // to swap back
        _game.Process(); // swap back
    }
}
