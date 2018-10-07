using System;
using UnityEngine;

public class PieceBehaviour : MonoBehaviour
{
    protected PieceView _pieceView;
    private Action _completedEventHandler;

    protected RectTransform _myRectTransform { get { return _pieceView.MyRectTransform; } }
    protected Animator _myAnimator { get { return _pieceView.MyAnimator; } }

    protected RectTransform _reference { get { return _pieceView.Reference; } }

    public void Initialize(PieceView pieceView, Action completedEventHandler)
    {
        _pieceView = pieceView;
        _completedEventHandler = completedEventHandler;
    }

    public virtual void Play()
    {
        enabled = true;
    }

    public void OnCompleted()
    {
        if (_completedEventHandler != null)
        {
            _completedEventHandler();
        }
    }
}