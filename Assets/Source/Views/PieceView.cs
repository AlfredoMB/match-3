using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PieceView : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerExitHandler
{
    public event EventHandler FellCompleted;

    public RectTransform MyRectTransform;
    public Animator MyAnimator;
    public Image MyImage;

    public Sprite[] PieceImages;

    public FallBehaviour FallBehaviour;
    public SwapBehaviour SwapBehaviour;
    public RemoveBehaviour RemoveBehaviour;

    private BoardView _boardView;
    private Board _board;
    
    private bool _isBeingDragged;
    private PieceView _swapTargetView;
    private bool IsSwappingBack;

    public BoardPiece BoardPiece { get; private set; }

    public int X { get { return BoardPiece.X; } }
    public int Y { get { return BoardPiece.Y; } }

    public RectTransform Reference { get; private set; }

    public void Initialize(BoardView boardView, Board board, BoardPiece boardPiece, int startingHeight)
    {
        _boardView = boardView;
        _board = board;
        BoardPiece = boardPiece;

        BoardPiece.Removed += OnRemoved;
        BoardPiece.Fell += OnFell;

        MyRectTransform.position = _boardView.GetReference(boardPiece.X, _board.Height + startingHeight, false).position;

        MyImage.sprite = PieceImages[boardPiece.Type];

        FallBehaviour.Initialize(this, OnFellCompleted);
        SwapBehaviour.Initialize(this, OnSwapCompleted);
        RemoveBehaviour.Initialize(this, OnRemovedCompleted);

        PlayFall();        
    }

    private void OnDestroy()
    {
        if(BoardPiece != null)
        {
            BoardPiece.Removed -= OnRemoved;
            BoardPiece.Fell -= OnFell;
        }
    }

    private void PlayFall()
    {
        Reference = _boardView.GetReference(BoardPiece.X, BoardPiece.Y);
        FallBehaviour.Play();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(BoardPiece.CurrentState != BoardPiece.EState.ReadyForMatch)
        {
            return;
        }

        BoardPiece.EnterSwapState();
        _isBeingDragged = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (BoardPiece.CurrentState != BoardPiece.EState.UnderSwap)
        {
            return;
        }

        BoardPiece.EnterReadyForMatchState();
        _isBeingDragged = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isBeingDragged)
        {
            return;
        }
        _isBeingDragged = false;

        var angle = Vector2.SignedAngle(Reference.up, eventData.position - eventData.pressPosition);
        if (angle > -45 && angle < 45)
        {
            // up
            SwapWithNeighbor(0, 1);
        }
        else if (angle >= 45 && angle < 135)
        {
            // left
            SwapWithNeighbor(-1, 0);
        }
        else if (angle <= -45 && angle > -135)
        {
            // right
            SwapWithNeighbor(1, 0);
        }
        else
        {
            // down
            SwapWithNeighbor(0, -1);
        }
    }

    public void SwapWithNeighbor(int dx, int dy)
    {
        int neighborX = X + dx;
        int neighborY = Y + dy;
        if (_board.IsOutOfBounds(neighborX, neighborY))
        {
            BoardPiece.EnterReadyForMatchState();
            return;
        }

        var neighbor = _board.GetPieceAt(neighborX, neighborY);
        if (neighbor == null || neighbor.CurrentState != BoardPiece.EState.ReadyForMatch)
        {
            BoardPiece.EnterReadyForMatchState();
            return;
        }

        _swapTargetView = _boardView.GetPieceView(neighbor);
        if(_swapTargetView == null)
        {
            Debug.LogError(neighbor);
            return;
        }

        SwapWithTarget();
    }

    private void SwapWithTarget()
    {
        var myReference = Reference;

        Reference = _swapTargetView.Reference;
        _swapTargetView.Reference = myReference;

        _swapTargetView.PlaySwap();
        PlaySwap();
    }
    
    public void PlaySwap()
    {
        BoardPiece.EnterSwapState();
        SwapBehaviour.Play();
    }

    private void OnSwapCompleted()
    {
        if (IsSwappingBack)
        {
            IsSwappingBack = false;
            BoardPiece.EnterReadyForMatchState();
            return;
        }

        if (_swapTargetView == null)
        {
            return;
        }
        BoardPiece.EnterReadyForMatchState();
        _swapTargetView.BoardPiece.EnterReadyForMatchState();

        _board.SelectPiece(BoardPiece);
        _board.SelectPiece(_swapTargetView.BoardPiece);

        _board.SwapCandidates();
        var matches = _board.GetMatchesForCandidates();
        if (matches != null)
        {
            _board.ConfirmSwappedPieces();
            _board.ResolveMatches(matches);
        }
        else
        {
            _board.SwapCandidates();
            SwapWithTarget();
            _board.ConfirmSwappedPieces();
            _swapTargetView.IsSwappingBack = IsSwappingBack = true;
            _swapTargetView = null;
        }
    }

    private void OnRemoved(object sender, EventArgs e)
    {
        RemoveBehaviour.Play();
    }

    private void OnRemovedCompleted()
    {
        _board.MovePiecesDown();
    }

    private void OnFell(object sender, EventArgs e)
    {
        PlayFall();
    }

    private void OnFellCompleted()
    {
        BoardPiece.EnterReadyForMatchState();
        if (FellCompleted != null)
        {
            FellCompleted(this, EventArgs.Empty);
        }

        var match = _board.GetMatchFor(BoardPiece);
        if (match != null)
        {
            _board.ResolveMatch(match);
        }
    }
}