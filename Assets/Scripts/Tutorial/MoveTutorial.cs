using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveTutorial : MonoBehaviour
{
    public Text WText, AText, SText, DText;

    private void Start()
    {

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
            WText.color = Color.green;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            AText.color = Color.green;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SText.color = Color.green;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            DText.color = Color.green;
        }
    }
    
}
