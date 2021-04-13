using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpringUtility
{
    public Vector3 Values;

    private float frequence;
    private float damp;
    private Vector3 dampValues;

    public CameraSpringUtility(float _frequence, float _damp)
    {
        frequence = _frequence;
        damp = _damp;
    }

    public void UpdateSpring(float deltaTime, Vector3 target)
    {
        Values -= deltaTime * frequence * dampValues;
        dampValues = Vector3.Lerp(dampValues, Values - target, damp * deltaTime);
    }
}
