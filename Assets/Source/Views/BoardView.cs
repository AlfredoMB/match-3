using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    public event EventHandler AllPiecesFell;

    public PieceView PieceViewPrefab;
    public Transform PositionReferences;

    public Transform PieceContainer;

    private Board _board;
    private List<PieceView> _pieceViews = new List<PieceView>();
    private int _fallenPieces;

    public void Initialize(Board board)
    {
        _board = board;;
        _board.PieceSpawned += OnPieceSpawned;
        StartCoroutine(SpawnBoard());
    }

    private void OnDestroy()
    {
        if (_board != null)
        {
            _board.PieceSpawned -= OnPieceSpawned;
        }

        foreach (var pieceView in _pieceViews)
        {
            if (pieceView == null)
            {
                continue;
            }
            pieceView.FellCompleted -= OnFellCompleted;
        }
    }
    
    private IEnumerator SpawnBoard()
    {
        for (int y = 0; y < _board.Height; y++)
        {
            for (int x = 0; x < _board.Width; x++)
            {
                SpawnPiece(_board.GetPieceAt(x, y), y);
                yield return null;
            }
        }
    }

    private void SpawnPiece(BoardPiece boardPiece, int startingHeight)
    {
        var pieceView = Instantiate(PieceViewPrefab, PieceContainer);
        pieceView.name = string.Format("{0}-{1}-{2}", boardPiece.X, boardPiece.Y, boardPiece.Type);
        
        _pieceViews.Add(pieceView);

        pieceView.Initialize(this, _board, boardPiece, startingHeight);
        pieceView.FellCompleted += OnFellCompleted;
    }

    private void OnFellCompleted(object sender, EventArgs e)
    {
        if (++_fallenPieces >= _board.Width * _board.Height && AllPiecesFell != null)
        {
            AllPiecesFell(this, EventArgs.Empty);
        }
    }

    public RectTransform GetReference(int x, int y, bool limitedByBounds = true)
    {
        if (limitedByBounds && _board.IsOutOfBounds(x, y))
        {
            return null;
        }
        return PositionReferences.GetChild(x + y * _board.Width) as RectTransform;
    }
    
    private void OnPieceSpawned(object sender, PieceSpawnedEventArgs e)
    {
        SpawnPiece(e.NewPiece, e.StartingHeight);
    }
    
    public PieceView GetPieceView(BoardPiece boardPiece)
    {
        return _pieceViews.Find(pieceView => pieceView.BoardPiece == boardPiece);
    }

    private void Update()
    {
        _board.MovePiecesDown();
    }
}
