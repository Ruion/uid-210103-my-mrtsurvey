using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Activate object with this component attached, and call Finish() at desire timing on this component, 
/// it will deactivate itself after a delay sec.
/// This component is used for showing success and fail status when sending data to server
/// Tips: Attach to a gameObject, set the sec to desire value.
/// </summary>
public class StatusBar : MonoBehaviour
{
    public float sec = 2f;
    public bool finish = false;
    public bool ignoreFinish = false;

    private void OnEnable()
    {
        finish = false;
        StartCoroutine(FadeOut());
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        if (!ignoreFinish) { while (!finish) yield return null; }

        yield return new WaitForSecondsRealtime(sec);

        gameObject.SetActive(false);
    }

    public void Finish()
    {
        finish = true;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

}
