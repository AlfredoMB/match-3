﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PieceView : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
    public RectTransform MyRectTransform;
    public Animator MyAnimator;
    public Image MyImage;
    public string ShakeParameter = "Shake";
    public string AlternativeParameter = "Alternative";
    public float SwapSpeed;

    public FallBehaviour FallBehaviour;
    public SwapBehaviour SwapBehaviour;
    
    private BoardView _boardView;
    private bool _hasReachedReferenceForSwap;
    private bool _isBeingDragged;

    public int X { get; private set; }
    public int Y { get; private set; }
    public RectTransform Reference { get; private set; }

    public void Initialize(BoardView boardView, int x, int y, Sprite sprite)
    {
        _boardView = boardView;
        SetReference(x, y);
        MyRectTransform.position = Reference.position + new Vector3(0, 20);

        MyImage.sprite = sprite;
        FallBehaviour.Initialize(this);
        SwapBehaviour.Initialize(this);
        FallBehaviour.Play();
    }

    public void SetReference(int x, int y)
    {
        X = x;
        Y = y;
        Reference = _boardView.GetReference(x, y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(X + " " + Y);
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

    public void PlaySwap()
    {
        SwapBehaviour.Play();
    }
}
