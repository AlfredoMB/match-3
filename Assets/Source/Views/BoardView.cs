using System;
using System.Collections;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    public event EventHandler SwapComplete;

    public PieceView PieceViewPrefab;
    public Transform PositionReferences;

    public Transform PieceContainer;

    private Board _board;
    private PieceView[,] _pieceViews;
    private int _completed;

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
        _pieceViews[boardPiece.X, boardPiece.Y] = piece;
        piece.Initialize(this, boardPiece);
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

    public void Select(PieceView pieceView)
    {
        if (!_board.IsReadyForInput)
        {
            return;
        }

        Debug.Log("SelectPieceAt");
        _board.SelectPieceAt(pieceView.X, pieceView.Y);
    }

    public void SwapWithNeighbor(PieceView pieceView, int dx, int dy)
    {
        Debug.Log(_board);

        if (!_board.IsReadyForInput)
        {
            return;
        }
        
        int neighborX = pieceView.X + dx;
        int neighborY = pieceView.Y + dy;
        if (IsOutOfBounds(neighborX, neighborY))
        {
            return;
        }
        _board.SelectPieceAt(neighborX, neighborY);

        Swap(pieceView, neighborX, neighborY);
    }

    private void Swap(PieceView pieceView, int neighborX, int neighborY)
    {
        var neighbor = _pieceViews[neighborX, neighborY];

        _pieceViews[pieceView.X, pieceView.Y] = neighbor;
        neighbor.SetReference(pieceView.X, pieceView.Y);
        neighbor.SwapComplete += OnSwapComplete;
        neighbor.PlaySwap();

        _pieceViews[neighborX, neighborY] = pieceView;
        pieceView.SetReference(neighborX, neighborY);
        pieceView.SwapComplete += OnSwapComplete;
        pieceView.PlaySwap();
    }

    private void OnSwapComplete(object sender, EventArgs e)
    {
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
        Debug.Log("OnSwappedBack");
        Swap(_pieceViews[e.SelectedPiece.X, e.SelectedPiece.Y], e.SwapCandidate.X, e.SwapCandidate.Y);
    }

    private void OnMatchesFound(object sender, MatchesFoundEventArgs e)
    {

    }

    private void OnPieceSpawned(object sender, PieceSpawnedEventArgs e)
    {
        SpawnPiece(e.NewPiece);
    }
}
