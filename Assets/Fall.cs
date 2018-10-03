using UnityEngine;

public class Fall : MonoBehaviour
{
    public Transform Transform;
    public Transform Reference;
    public Animator Animator;
    public string ShakeParameter = "Shake";

    public float Gravity;

    private float _currentSpeed;
    private int _shakeParameterHash;

    private void Start()
    {
        _shakeParameterHash = Animator.StringToHash(ShakeParameter);
        Animator.SetBool(_shakeParameterHash, false);
    }

    private void OnEnable()
    {
        _currentSpeed = 0;
    }

    private void Update()
    {
        _currentSpeed += Gravity * Time.deltaTime;
        Transform.position = new Vector3(Transform.position.x, Mathf.Max(Transform.position.y + _currentSpeed, Reference.position.y));

        Debug.Log(Transform.position.y + " <= " + Reference.position.y);

        if (Transform.position.y <= Reference.position.y || Mathf.Approximately(Transform.position.y, Reference.position.y))
        {
            Animator.SetBool(_shakeParameterHash, true);
            enabled = false;
        }
    }
}
