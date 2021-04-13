using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunTutorial : MonoBehaviour
{
    public Text ShiftText, CText, SpaceText;
    // Start is called before the first frame update
    void Start()
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
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ShiftText.color = Color.green;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            CText.color = Color.green;
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SpaceText.color = Color.green;
        }
    }
}
