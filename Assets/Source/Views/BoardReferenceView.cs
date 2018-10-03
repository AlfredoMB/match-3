using UnityEngine;

public class BoardReferenceView : MonoBehaviour
{
    public Transform Transform;

    public Transform GetReference(int x, int y, int width)
    {
        return Transform.GetChild(x + y * width);
    }
}
