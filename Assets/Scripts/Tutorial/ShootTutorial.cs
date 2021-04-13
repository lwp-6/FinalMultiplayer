using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootTutorial : MonoBehaviour
{
    public Text Mouse0Text, Mouse1Text, ReloadText;
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
        if (Input.GetMouseButton(0))
        {
            Mouse0Text.color = Color.green;
        }
        if (Input.GetMouseButton(1))
        {
            Mouse1Text.color = Color.green;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadText.color = Color.green;
        }
    }
}
