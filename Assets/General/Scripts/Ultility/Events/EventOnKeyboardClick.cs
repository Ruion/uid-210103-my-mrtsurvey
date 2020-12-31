using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnKeyboardClick : MonoBehaviour
{
    public KeyCode key;
    public UnityEvent eventOnKeyClick;
    public bool executeOnce;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyUp(key))
        {
            Debug.Log($"{key} clicked");
            eventOnKeyClick.Invoke();
            if (executeOnce)
                enabled = false;
        }
    }

    public void InvokeEvent()
    {
        eventOnKeyClick.Invoke();
    }
}