using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreen : MonoBehaviour
{
    private int defWidth;
    private int defHeight;

    public void Awake()
    {
        defWidth = Screen.width;
        defHeight = Screen.height;
    }

    public void ChangeFullScreen()
    {
        if (!Screen.fullScreen)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
        else
        {
            Screen.SetResolution(defWidth, defHeight, false);
        }
    }
}