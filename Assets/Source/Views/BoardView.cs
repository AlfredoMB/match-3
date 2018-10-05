﻿using System;
using System.Collections;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    public PieceView[] PiecePrefabs;
    public Transform PositionReferences;

    public Transform PieceContainer;

    private Board _board;
    private PieceView[,] _pieceViews;

    public void Initialize(Board board)
    {
        _board = board;
        _pieceViews = new PieceView[_board.Width, _board.Height];
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
                _pieceViews[x, y] = piece;
                piece.Initialize(this, x, y);
                yield return null;
            }
        }
    }

    public bool IsOutOfBounds(int x, int y)
    {
        return x < 0 || x > _board.Width || y < 0 || y > _board.Height;
    }

    public RectTransform GetReference(int x, int y)
    {
        if (IsOutOfBounds(x, y))
        {
            return null;
        }
        return PositionReferences.GetChild(x + y * _board.Width) as RectTransform;
    }

    public PieceView GetPieceView(int x, int y)
    {
        if (IsOutOfBounds(x, y))
        {
            return null;
        }
        return _pieceViews[x, y];
    }
}