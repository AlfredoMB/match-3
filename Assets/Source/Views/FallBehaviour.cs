using UnityEngine;

public class FallBehaviour : MonoBehaviour
{
    public string ShakeParameter = "Shake";
    public string AlternativeParameter = "Alternative";

    public float Gravity;

    private float _currentSpeed;

    private int _shakeParameterHash;
    private int _alternativeParameterHash;

    private PieceView _pieceView;

    private RectTransform _myRectTransform { get { return _pieceView.MyRectTransform; } }
    private Animator _myAnimator { get { return _pieceView.MyAnimator; } }
    private RectTransform _reference { get { return _pieceView.Reference; } }

    public void FallToReference(PieceView pieceView)
    {
        _pieceView = pieceView;
        enabled = true;
    }

    private void Start()
    {
        _shakeParameterHash = Animator.StringToHash(ShakeParameter);
        _alternativeParameterHash = Animator.StringToHash(AlternativeParameter);
        _myAnimator.SetBool(_shakeParameterHash, false);
        _myAnimator.SetBool(_alternativeParameterHash, false);
    }
    
    private void OnEnable()
    {
        _currentSpeed = 0;
    }

    private void Update()
    {
        _currentSpeed += Gravity * Time.deltaTime;
        _myRectTransform.position = new Vector3(_myRectTransform.position.x, Mathf.Max(_myRectTransform.position.y + _currentSpeed, _reference.position.y));

        if (_myRectTransform.position.y <= _reference.position.y || Mathf.Approximately(_myRectTransform.position.y, _reference.position.y))
        {
            _myAnimator.SetBool(_shakeParameterHash, true);
            _myAnimator.SetBool(_alternativeParameterHash, UnityEngine.Random.Range(0, 1) > 0);
            enabled = false;
        }
    }
}
