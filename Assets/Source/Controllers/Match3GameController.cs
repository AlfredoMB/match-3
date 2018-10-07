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
        _board = new Board(Width, Height, MinMatchSize, new[] { 0, 1, 2, 3, 4 }, 0);// UnityEngine.Random.Range(int.MinValue, int.MaxValue));
        _board.RandomFillUp();

        _game = new Match3Game(_board);

        BoardView.Initialize(_board);
        BoardView.SwapComplete += OnSwapComplete;
        BoardView.RemoveComplete += OnRemoveComplete;
        BoardView.MovingPiecesDownComplete += OnMovingPiecesDownComplete;
    }

    private void OnDestroy()
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
