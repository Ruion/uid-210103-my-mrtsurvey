using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Execute events when at the Awake()
/// Tips: attach this component to gameObject and assign functions to eventsOnAwake.
/// Caution: gameObject attaching this component must active in hierarchy to execute the events
/// </summary>
public class StartEventsOnAwake : MonoBehaviour {

    public UnityEvent eventsOnAwake;

	void Awake()
    {
        eventsOnAwake.Invoke();
    }

}
