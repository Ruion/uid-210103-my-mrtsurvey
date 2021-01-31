using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// TimeScaleController change the speed of app. Using this, You can increase the speed of
/// Time.timeScale to speed up testing.
/// </summary>
public class TimeScaleController : MonoBehaviour
{
    public Slider slider;

    private void OnEnable()
    {
        if (!JSONExtension.LoadEnvBool("DEBUG_MODE")) gameObject.SetActive(false);
    }

    public void ChangeTimeScale()
    {
        Time.timeScale = slider.value;
    }
}