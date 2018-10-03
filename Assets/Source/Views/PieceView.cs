using System;
using UnityEngine;

public class PieceView : MonoBehaviour
{
    public Transform Transform;
    public Transform Reference;
    public Animator Animator;
    public string ShakeParameter = "Shake";
    public string AlternativeParameter = "Alternative";

    public float Gravity;

    private float _currentSpeed;
    private int _shakeParameterHash;
    private int _alternativeParameterHash;

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
    }

    private void Update()
    {
        _currentSpeed += Gravity * Time.deltaTime;
        Transform.position = new Vector3(Transform.position.x, Mathf.Max(Transform.position.y + _currentSpeed, Reference.position.y));

        if (Transform.position.y <= Reference.position.y || Mathf.Approximately(Transform.position.y, Reference.position.y))
        {
            Animator.SetBool(_shakeParameterHash, true);
            Animator.SetBool(_alternativeParameterHash, UnityEngine.Random.Range(0, 1) > 0);
            enabled = false;
        }
    }

    public void SetReference(Transform reference)
    {
        Reference = reference;
        Transform.position = Reference.position + new Vector3(0, 20);
    }
}
