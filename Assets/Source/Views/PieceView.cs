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
    private PieceView _swapTarget;

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

        _board.SelectPiece(BoardPiece);
        _isBeingDragged = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _isBeingDragged = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (BoardPiece.CurrentState != BoardPiece.EState.ReadyForMatch)
        {
            return;
        }

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
            return;
        }

        var neighbor = _board.GetPieceAt(neighborX, neighborY);
        if (neighbor.CurrentState != BoardPiece.EState.ReadyForMatch)
        {
            return;
        }
        _board.SelectPiece(neighbor);

        _swapTarget = _boardView.GetPieceView(neighbor);

        SwapWithTarget();
    }

    private void SwapWithTarget()
    {
        var myReference = Reference;

        Reference = _swapTarget.Reference;
        _swapTarget.Reference = myReference;

        _swapTarget.PlaySwap();
        PlaySwap();
    }
    
    public void PlaySwap()
    {
        SwapBehaviour.Play();
    }

    private void OnFellCompleted()
    {
        BoardPiece.ReadyForMatch();
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

    private void OnSwapCompleted()
    {
        BoardPiece.ReadyForMatch();
        if (_swapTarget == null)
        {
            return;
        }

        _board.SwapCandidates();
        var matches = _board.GetMatchesForCandidates();
        if (matches != null)
        {
            _board.ConfirmSwappedPieces();
            _board.ResolveMatches(matches);
        }
        else
        {
            SwapWithTarget();
            _swapTarget = null;
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
}