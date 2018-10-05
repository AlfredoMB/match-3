using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PieceView : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
{
    public RectTransform RectTransform;
    public Transform Reference;
    public Animator Animator;
    public string ShakeParameter = "Shake";
    public string AlternativeParameter = "Alternative";
    public float SwapSpeed;

    public float Gravity;

    private float _currentSpeed;
    private int _shakeParameterHash;
    private int _alternativeParameterHash;

    private Board _board;
    private BoardView _boardView;
    private BoardPiece _boardPiece;

    private bool _hasReachedReference;
    private bool _hasReachedReferenceForSwap;
    private bool _isBeingDragged;

    public void Initialize(Board board, BoardView boardView, BoardPiece boardPiece, Transform reference)
    {
        _board = board;
        _boardView = boardView;
        _boardPiece = boardPiece;
        SetReference(reference);
        RectTransform.position = RectTransform.position + new Vector3(0, 20);

        _board.Swapped += OnSwapped;
    }

    private void SetReference(Transform reference)
    {
        if (reference == null)
        {
            return;
        }

        Reference = reference;
        RectTransform.position = Reference.position;
    }

    private void Start()
    {
        _shakeParameterHash = Animator.StringToHash(ShakeParameter);
        _alternativeParameterHash = Animator.StringToHash(AlternativeParameter);
        Animator.SetBool(_shakeParameterHash, false);
        Animator.SetBool(_alternativeParameterHash, false);
    }

    private void OnEnable()
    {
        _currentSpeed = 0;
        _hasReachedReference = false;
        _hasReachedReferenceForSwap = true;
    }

    private void OnDisable()
    {
        _board.Swapped -= OnSwapped;
    }

    private void Update()
    {
        if (!_hasReachedReference)
        {
            _currentSpeed += Gravity * Time.deltaTime;
            RectTransform.position = new Vector3(RectTransform.position.x, Mathf.Max(RectTransform.position.y + _currentSpeed, Reference.position.y));

            if (RectTransform.position.y <= Reference.position.y || Mathf.Approximately(RectTransform.position.y, Reference.position.y))
            {
                Animator.SetBool(_shakeParameterHash, true);
                Animator.SetBool(_alternativeParameterHash, UnityEngine.Random.Range(0, 1) > 0);
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

    /*
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(_boardPiece);
        _board.SelectPiece(_boardPiece);
    }
    */

    private void OnSwapped(object sender, SwappedEventArgs e)
    {
        if (e.SelectedPiece == _boardPiece || e.SwapCandidate == _boardPiece)
        {
            _boardView.GetReference(_boardPiece);
            _hasReachedReferenceForSwap = false;
        }
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

        var angle = Vector2.SignedAngle(Reference.up, eventData.position - eventData.pressPosition);
        if (angle > -45 && angle < 45)
        {
            // up
            SetReference(_boardView.GetReference(_boardPiece.X, _boardPiece.Y + 1));
        }
        else if (angle >= 45 && angle < 135)
        {
            // left
            SetReference(_boardView.GetReference(_boardPiece.X - 1, _boardPiece.Y));
        }
        else if (angle <= -45 && angle > -135)
        {
            // right
            SetReference(_boardView.GetReference(_boardPiece.X + 1, _boardPiece.Y));
        }
        else
        {
            // down
            SetReference(_boardView.GetReference(_boardPiece.X, _boardPiece.Y - 1));
        }
    }

}
