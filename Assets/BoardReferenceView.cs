using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardReferenceView : MonoBehaviour
{
    private void Start()
    {
        Debug.Log(transform.GetChild(0), transform.GetChild(0));


        Debug.Log(transform.GetChild(36), transform.GetChild(36));
    }
}
