using UnityEngine;

public class PieceBehaviour : MonoBehaviour
{
    protected PieceView _pieceView;

    protected RectTransform _myRectTransform { get { return _pieceView.MyRectTransform; } }
    protected Animator _myAnimator { get { return _pieceView.MyAnimator; } }

    protected RectTransform _reference { get { return _pieceView.Reference; } }

    public void Initialize(PieceView pieceView)
    {
        _pieceView = pieceView;
    }

    public virtual void Play()
    {
        enabled = true;
    }
}