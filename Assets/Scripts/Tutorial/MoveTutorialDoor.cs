using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTutorialDoor : MonoBehaviour
{
    private bool W, A, S, D;

    private void Start()
    {
        W = false;
        A = false;
        S = false;
        D = false;
    }
    // Update is called once per frame
    void Update()
    {
        // 暂停
        if (GlobleVar.isPause)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            W = true;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            A = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            S = true;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            D = true;
        }
        if (W && A && S && D)
        { 
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }    
    }
}
