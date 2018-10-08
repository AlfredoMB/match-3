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

    public BoardPiece BoardPiece { get; private set; }

    public RectTransform Reference { get; private set; }

    public void Initialize(BoardView boardView, Board board, BoardPiece boardPiece, int startingHeight)
    {
        _boardView = boardView;
        _board = board;
        BoardPiece = boardPiece;

        BoardPiece.Removed += OnRemoved;
        BoardPiece.Fell += OnFell;
        BoardPiece.Swapped += OnSwapped;

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
            BoardPiece.Swapped -= OnSwapped;
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

        _board.SelectPiece(BoardPiece);
        _isBeingDragged = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        BoardPiece.EnterReadyForMatchState();
        if (_board.IsReadyToSwap())
        {
            _board.SwapCandidates();
        }

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
        int neighborX = BoardPiece.X + dx;
        int neighborY = BoardPiece.Y + dy;
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

        _board.SelectPiece(neighbor);
        if (_board.IsReadyToSwap())
        {
            _board.SwapCandidates();
        }
    }

    private void OnSwapped(object sender, EventArgs e)
    {
        Reference = _boardView.GetReference(BoardPiece.X, BoardPiece.Y);
        SwapBehaviour.Play();
    }

    private void OnSwapCompleted()
    {
        BoardPiece.EnterReadyForMatchState();
        if (_board.IsReadyToSwap())
        {
            if (!TryToMatchSwap())
            {                
                _board.SwapCandidates();
            }
            _board.ConfirmSwappedPieces();
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

        TryToMatch();
    }

    private bool TryToMatch()
    {
        var match = _board.GetMatchFor(BoardPiece);
        if (match != null)
        {
            _board.ResolveMatch(match);
            return true;
        }
        return false;
    }

    private bool TryToMatchSwap()
    {
        var matches = _board.GetMatchesForCandidates();
        if (matches != null)
        {
            _board.ResolveMatches(matches);
            return true;
        }
        return false;
    }
}