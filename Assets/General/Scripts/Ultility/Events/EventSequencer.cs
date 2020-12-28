using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Execute unity event with delay. Each event has its own delay, set delay to 0 if no delay desired.
/// Tips: attach this component to gameObject and assign functions to eventsOnAwake. Call Run() to execute events.
/// EventSequencer can be use in many case like play sound after delay, looping music after delays, gather and execute function in sequences.
/// </summary>
public class EventSequencer : MonoBehaviour {

    [Header("Make Events and Delays have same sizes")]
    public UnityEvent[] events;
    public float[] delays;

/// <summary>
/// if isRealtime is set to true, the delay go on no mather how much is the Time.timeScale value
/// For example game is running in half speed when Time.timeScale = 0.5f, but the Events will execute in normal speed.
/// </summary>
    public bool isRealtime = true;

/// <summary>
/// Call 
/// </summary>
    public void Run()
    {
        StartCoroutine(ExecuteEvents());
    }

    IEnumerator ExecuteEvents()
    {
        for (int e = 0; e < events.Length; e++)
        {
            if(events.Length == delays.Length)
            {
                if(isRealtime) yield return new WaitForSecondsRealtime(delays[e]);

                else yield return new WaitForSeconds(delays[e]);
            }
            else
            {
              if(isRealtime) yield return new WaitForSecondsRealtime(delays[0]);

                else yield return new WaitForSeconds(delays[0]);  
            }

            events[e].Invoke();
        }
    }
}