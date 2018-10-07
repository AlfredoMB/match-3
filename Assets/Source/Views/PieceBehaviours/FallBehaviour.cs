using UnityEngine;

public class FallBehaviour : PieceBehaviour
{
    public string ShakeParameter = "Shake";
    public string AlternativeParameter = "Alternative";

    public float Gravity;

    private float _currentSpeed;
    private float _lastDistance;

    private int _shakeParameterHash;
    private int _alternativeParameterHash;

    public override void Play()
    {
        _currentSpeed = 0;
        _lastDistance = float.MaxValue;
        base.Play();
    }

    private void Start()
    {
        _shakeParameterHash = Animator.StringToHash(ShakeParameter);
        _alternativeParameterHash = Animator.StringToHash(AlternativeParameter);
        _myAnimator.SetBool(_shakeParameterHash, false);
        _myAnimator.SetBool(_alternativeParameterHash, false);
    }

    private void Update()
    {
        _currentSpeed += Gravity * Time.deltaTime;
        _myRectTransform.position = _myRectTransform.position + (_reference.position - _myRectTransform.position).normalized * _currentSpeed * Time.deltaTime;

        var distance = Vector2.Distance(_myRectTransform.position, _reference.position);
        if (distance < _lastDistance)
        {
            _lastDistance = distance;
            return;
        }

        _myRectTransform.position = _reference.position;
        _myAnimator.SetBool(_shakeParameterHash, true);
        _myAnimator.SetBool(_alternativeParameterHash, Random.Range(0, 1) > 0);
        enabled = false;
        OnCompleted();
    }
}
