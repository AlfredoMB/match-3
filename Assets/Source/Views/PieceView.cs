using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PieceView : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
    public RectTransform MyRectTransform;
    public Animator MyAnimator;
    public string ShakeParameter = "Shake";
    public string AlternativeParameter = "Alternative";
    public float SwapSpeed;

    public float Gravity;

    private RectTransform _reference;

    private float _currentSpeed;
    private int _shakeParameterHash;
    private int _alternativeParameterHash;

    private BoardView _boardView;
    private bool _hasReachedReference;
    private bool _hasReachedReferenceForSwap;
    private bool _isBeingDragged;

    public int X { get; private set; }
    public int Y { get; private set; }

    public void Initialize(BoardView boardView, int x, int y)
    {
        _boardView = boardView;
        SetReference(x, y);
        MyRectTransform.position = MyRectTransform.position + new Vector3(0, 20);
    }

    public void SetReference(int x, int y)
    {
        X = x;
        Y = y;
        _reference = _boardView.GetReference(x, y);

        // needed?
        MyRectTransform.position = _reference.position;
    }

    private void Start()
    {
        _shakeParameterHash = Animator.StringToHash(ShakeParameter);
        _alternativeParameterHash = Animator.StringToHash(AlternativeParameter);
        MyAnimator.SetBool(_shakeParameterHash, false);
        MyAnimator.SetBool(_alternativeParameterHash, false);
    }

    private void OnEnable()
    {
        _currentSpeed = 0;
        _hasReachedReference = false;
        _hasReachedReferenceForSwap = true;
    }

    private void Update()
    {
        if (!_hasReachedReference)
        {
            _currentSpeed += Gravity * Time.deltaTime;
            MyRectTransform.position = new Vector3(MyRectTransform.position.x, Mathf.Max(MyRectTransform.position.y + _currentSpeed, _reference.position.y));

            if (MyRectTransform.position.y <= _reference.position.y || Mathf.Approximately(MyRectTransform.position.y, _reference.position.y))
            {
                MyAnimator.SetBool(_shakeParameterHash, true);
                MyAnimator.SetBool(_alternativeParameterHash, UnityEngine.Random.Range(0, 1) > 0);
                _hasReachedReference = true;
            }
        }
        /*
        if (!_hasReachedReferenceForSwap)
        {
            Transform.position = (Reference.position - Transform.position).normalized * SwapSpeed * Time.deltaTime;
            
            if (Transform.position.y <= Reference.position.y || Mathf.Approximately(Transform.position.y, Reference.position.y))
            {
                _hasReachedReferenceForSwap = true;
            }
        }*/
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown " + this, this);
        _isBeingDragged = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnPointerExit " + this, this);
        if (!_isBeingDragged)
        {
            return;
        }
        _isBeingDragged = false;

        var angle = Vector2.SignedAngle(_reference.up, eventData.position - eventData.pressPosition);
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

    private void SwapWithNeighbor(int dx, int dy)
    {
        SwapWith(_boardView.GetPieceView(X + dx, Y + dy));
    }

    public void SwapWith(PieceView other)
    {
        int otherX = other.X;
        int otherY = other.Y;

        other.SetReference(X, Y);
        SetReference(otherX, otherY);
    }
}
