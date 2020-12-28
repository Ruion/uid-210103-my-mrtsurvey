using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Execute Unity Event tap is detected.
/// By default it only execute events once and disable itself. Change this behaviour by toggling executeOnce boolean.
/// Notes: beware the OnTap event execute every tap. You may not want some function to execute every tap,
/// executeOnce serve to prevent this happen
/// </summary>
public class TapDetector : MonoBehaviour
{
    public UnityEvent OnTap;

    public bool executeOnce = true;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
           OnTap.Invoke();
           if (executeOnce) enabled = false;
         }
    }
}
