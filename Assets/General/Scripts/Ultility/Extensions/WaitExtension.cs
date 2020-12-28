using UnityEngine;
using System.Collections;
using System;

public class WaitExtension : MonoBehaviour
{
    public void Wait(float seconds, Action action)
    {
        StartCoroutine(_wait(seconds, action));
    }
    IEnumerator _wait(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }

    // example usage
    // Wait (5, () => {
    // Debug.Log("5 seconds is lost forever"); });
}