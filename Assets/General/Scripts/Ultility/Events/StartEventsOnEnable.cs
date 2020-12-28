using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Execute events when at the OnEnable(). Execute events when the gameObject is activated.
/// Tips: attach this component to gameObject and assign functions to eventsOEnable.
/// Caution: gameObject attaching this component will execute eventsOEnable everytime it is activated
/// </summary>
public class StartEventsOnEnable : MonoBehaviour {

    public UnityEvent eventsOEnable;

	void OnEnable()
    {
        eventsOEnable.Invoke();
    }

}
