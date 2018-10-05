using UnityEngine;

public class SwapBehaviour : PieceBehaviour
{
    public float SwapSpeed = 1;

    private float _lastDistance;

    public override void Play()
    {
        _lastDistance = float.MaxValue;
        base.Play();
    }

    private void Update()
    {
        _myRectTransform.position = _myRectTransform.position + (_reference.position - _myRectTransform.position).normalized * SwapSpeed * Time.deltaTime;

        var distance = Vector2.Distance(_myRectTransform.position, _reference.position);
        if (distance < _lastDistance)
        {
            _lastDistance = distance;
            return;
        }
        
        _myRectTransform.position = _reference.position;
        enabled = false;
    }
}
