using System;
using System.Collections;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    public event EventHandler SwapComplete;
    public event EventHandler RemoveComplete;
    public event EventHandler MovingPiecesDownComplete;

    public PieceView PieceViewPrefab;
    public Transform PositionReferences;

    public Transform PieceContainer;

    private Board _board;
    private PieceView[,] _pieceViews;
    private int _completed;
    private int _movedPieces;
    private int _removedPieces;

    public void Initialize(Board board)
    {
        _board = board;
        _board.SwappedBack += OnSwappedBack;
        _board.MatchesFound += OnMatchesFound;
        _board.PieceSpawned += OnPieceSpawned;
        _pieceViews = new PieceView[_board.Width, _board.Height];
        StartCoroutine(SpawnBoard());
    }

    private void OnDestroy()
    {
        if (_board != null)
        {
            _board.MatchesFound -= OnMatchesFound;
            _board.PieceSpawned -= OnPieceSpawned;
        }

        foreach (var pieceView in _pieceViews)
        {
            if (pieceView == null)
            {
                continue;
            }
            pieceView.SwapComplete -= OnSwapComplete;
            pieceView.RemoveComplete -= OnPieceRemoveComplete;
            pieceView.FallComplete -= OnPieceMovingDownComplete;
        }
    }

    private IEnumerator SpawnBoard()
    {
        for (int y = 0; y < _board.Height; y++)
        {
            for (int x = 0; x < _board.Width; x++)
            {
                var boardPiece = _board.GetPieceAt(x, y);
                SpawnPiece(boardPiece);
                yield return null;
            }
        }
    }

    private void SpawnPiece(BoardPiece boardPiece)
    {
        var piece = Instantiate(PieceViewPrefab, PieceContainer);
        piece.name = boardPiece.X + "-" + boardPiece.Y;
        _pieceViews[boardPiece.X, boardPiece.Y] = piece;
        piece.Initialize(this, boardPiece);
        piece.SwapComplete += OnSwapComplete;
        piece.RemoveComplete += OnPieceRemoveComplete;
        piece.FallComplete += OnPieceMovingDownComplete;
    }

    private void OnFallComplete(object sender, EventArgs e)
    {
        throw new NotImplementedException();
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

    public void SetView(PieceView pieceView, int x, int y)
    {
        _pieceViews[x, y] = pieceView;
    }

    public void Select(PieceView pieceView)
    {
        if (!_board.IsReadyForInput)
        {
            return;
        }

        Debug.Log("Select: " + pieceView + " " + pieceView.Reference + " " + pieceView.X + " " + pieceView.Y);
        _board.SelectPieceAt(pieceView.X, pieceView.Y);
    }

    public void SwapWithNeighbor(PieceView pieceView, int dx, int dy)
    {
        Debug.Log(_board);

        if (!_board.IsReadyForInput)
        {
            return;
        }

        Debug.Log("SwapWithNeighbor: " + pieceView + " " + pieceView.Reference + " " + pieceView.X + " " + pieceView.Y + ", " + dx + " " + dy);

        int neighborX = pieceView.X + dx;
        int neighborY = pieceView.Y + dy;
        if (IsOutOfBounds(neighborX, neighborY))
        {
            return;
        }
        _board.SelectPieceAt(neighborX, neighborY);

        Debug.LogFormat("{0}, {1}, {2}", pieceView, neighborX, neighborY);
        Swap(pieceView, neighborX, neighborY);
    }

    private void Swap(PieceView pieceView, int neighborX, int neighborY)
    {
        var neighbor = _pieceViews[neighborX, neighborY];

        _pieceViews[pieceView.X, pieceView.Y] = neighbor;
        Debug.Log(neighbor);
        neighbor.SetReference(pieceView.X, pieceView.Y);
        neighbor.PlaySwap();

        _pieceViews[neighborX, neighborY] = pieceView;
        pieceView.SetReference(neighborX, neighborY);
        Debug.Log(pieceView);
        pieceView.PlaySwap();
    }

    private void OnSwapComplete(object sender, EventArgs e)
    {
        var view = sender as PieceView;
        Debug.Log("OnSwapComplete: " + view.Reference, view);
        if (_completed > 0 && SwapComplete != null)
        {
            _completed = 0;
            SwapComplete(this, EventArgs.Empty);
        }
        else
        {
            _completed = 1;
        }
    }

    private void OnSwappedBack(object sender, SwappedEventArgs e)
    {
        Swap(_pieceViews[e.SelectedPiece.X, e.SelectedPiece.Y], e.SwapCandidate.X, e.SwapCandidate.Y);
    }

    private void OnMatchesFound(object sender, MatchesFoundEventArgs e)
    {
        //_currentMatchPieces = e.Matches.TotalCount();
    }

    private void OnPieceSpawned(object sender, PieceSpawnedEventArgs e)
    {
        SpawnPiece(e.NewPiece);
    }

    private void OnPieceRemoveComplete(object sender, EventArgs e)
    {
        var view = sender as PieceView;
        Destroy(view.gameObject);

        Debug.Log((_removedPieces + 1) + " - " + _board.RemovedPiecesCount);

        if (++_removedPieces < _board.RemovedPiecesCount)
        {
            return;
        }

        _removedPieces = 0;

        if (RemoveComplete != null)
        {
            RemoveComplete(this, EventArgs.Empty);
        }
    }

    private void OnPieceMovingDownComplete(object sender, EventArgs e)
    {
        Debug.Log((_movedPieces + 1) + " - " + _board.MovedPiecesCount);

        if (++_movedPieces < _board.MovedPiecesCount)
        {
            return;
        }

        _movedPieces = 0;


        if (MovingPiecesDownComplete != null)
        {
            MovingPiecesDownComplete(this, EventArgs.Empty);
        }
    }
}
