using UnityEngine;

public class RemoveBehaviour : PieceBehaviour
{
    public float RemoveSpeed = 1;
    
    public override void Play()
    {
        base.Play();
    }

    private void Update()
    {
        _myRectTransform.localScale -= Vector3.one * RemoveSpeed * Time.deltaTime;

        if (_myRectTransform.localScale.x <= 0)
        {
            _myRectTransform.localScale = Vector3.zero;
            enabled = false;
            OnCompleted();
        }
    }
}
