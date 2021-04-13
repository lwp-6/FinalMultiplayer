using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingController : MonoBehaviour
{
    public void RotateLoadIcon(float z)
    {
        transform.Rotate(0, 0, z);
    }
}
