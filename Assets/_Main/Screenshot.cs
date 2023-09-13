using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshot : MonoBehaviour
{
    void OnEnable()
    {
        ScreenCapture.CaptureScreenshot("GameScreenshot.png");
    }
}
