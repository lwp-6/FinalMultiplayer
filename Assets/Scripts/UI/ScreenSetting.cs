using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSetting : MonoBehaviour
{
    public Dropdown Resolution;
    public Dropdown ScreenMode;

    public void SetScreenSize()
    {
        //Screen.SetResolution(width, height, isFullScreen);

        Screen.SetResolution(GlobleVar.ScreenResolution[Resolution.value, 0], 
            GlobleVar.ScreenResolution[Resolution.value, 1], 
            ScreenMode.value > 0);
        /*Debug.Log(GlobleVar.ScreenResolution[Resolution.value, 0]);
        Debug.Log(GlobleVar.ScreenResolution[Resolution.value, 1]);
        Debug.Log(ScreenMode.value > 0);*/
    }

    public void SetScreenMode()
    {
        Screen.fullScreen = ScreenMode.value > 0;
    }
}
