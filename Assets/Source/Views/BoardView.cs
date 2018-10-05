using System;
using System.Collections;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    public PieceView[] PiecePrefabs;
    public Transform PositionReferences;

    public Transform PieceContainer;

    private Board _board;

    public void Initialize(Board board)
    {
        _board = board;
        StartCoroutine(SpawnBoard());
    }

    private IEnumerator SpawnBoard()
    {
        for (int y = 0; y < _board.Height; y++)
        {
            for (int x = 0; x < _board.Width; x++)
            {
                var boardPiece = _board.GetPieceAt(x, y);
                var piece = Instantiate(PiecePrefabs[boardPiece.Type], PieceContainer);
                piece.Initialize(_board, this, boardPiece, GetReference(x, y));
                yield return null;
            }
        }
    }

    public Transform GetReference(int x, int y)
    {
        if (x < 0 || x > _board.Width || y < 0 || y > _board.Height)
        {
            return null;
        }
        return PositionReferences.GetChild(x + y * _board.Width);
    }

    public Transform GetReference(BoardPiece boardPiece)
    {
        return GetReference(boardPiece.X, boardPiece.Y);
    }
}