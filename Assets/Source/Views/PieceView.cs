using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PieceView : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerExitHandler
{
    public event EventHandler FallComplete;
    public event EventHandler SwapComplete;
    public event EventHandler RemoveComplete;

    public RectTransform MyRectTransform;
    public Animator MyAnimator;
    public Image MyImage;

    public Sprite[] PieceImages;

    public FallBehaviour FallBehaviour;
    public SwapBehaviour SwapBehaviour;
    public RemoveBehaviour RemoveBehaviour;

    private BoardView _boardView;
    private BoardPiece _boardPiece;
    private bool _isBeingDragged;

    public int X { get; private set; }
    public int Y { get; private set; }
    public RectTransform Reference { get; private set; }

    public void Initialize(BoardView boardView, BoardPiece boardPiece)
    {
        _boardView = boardView;
        _boardPiece = boardPiece;
        _boardPiece.Removed += OnRemoved;
        _boardPiece.MovedDown += OnMovedDown;

        SetReference(boardPiece.X, boardPiece.Y);
        MyRectTransform.position = Reference.position + new Vector3(0, 20);

        MyImage.sprite = PieceImages[boardPiece.Type];

        FallBehaviour.Initialize(this, OnFallComplete);
        SwapBehaviour.Initialize(this, OnSwapComplete);
        RemoveBehaviour.Initialize(this, OnRemoveComplete);

        PlayFall();
    }

    private void OnFallComplete()
    {
        if (FallComplete != null)
        {
            FallComplete(this, EventArgs.Empty);
        }
    }

    private void OnSwapComplete()
    {
        if (SwapComplete != null)
        {
            SwapComplete(this, EventArgs.Empty);
        }
    }

    private void OnRemoveComplete()
    {
        if (RemoveComplete != null)
        {
            RemoveComplete(this, EventArgs.Empty);
        }
    }

    private void OnDestroy()
    {
        if(_boardPiece != null)
        {
            _boardPiece.Removed -= OnRemoved;
            _boardPiece.MovedDown -= OnMovedDown;
        }
    }

    private void OnRemoved(object sender, EventArgs e)
    {
        RemoveBehaviour.Play();
    }

    private void OnMovedDown(object sender, EventArgs e)
    {
        SetReference(_boardPiece.X, _boardPiece.Y);
        FallBehaviour.Play();
    }

    public void SetReference(int x, int y)
    {
        X = x;
        Y = y;
        _boardView.SetView(this, x, y);
        Reference = _boardView.GetReference(x, y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _boardView.Select(this);
        _isBeingDragged = true;
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
            _boardView.SwapWithNeighbor(this, 0, 1);
        }
        else if (angle >= 45 && angle < 135)
        {
            // left
            _boardView.SwapWithNeighbor(this, -1, 0);
        }
        else if (angle <= -45 && angle > -135)
        {
            // right
            _boardView.SwapWithNeighbor(this, 1, 0);
        }
        else
        {
            // down
            _boardView.SwapWithNeighbor(this, 0, -1);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _isBeingDragged = false;
    }

    public void PlayFall()
    {
        FallBehaviour.Play();
    }

    public void PlaySwap()
    {
        SwapBehaviour.Play();
    }

    public void PlayRemove()
    {
        RemoveBehaviour.Play();
    }
}
