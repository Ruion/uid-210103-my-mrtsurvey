using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Execute events when a scene is loaded.
/// Tips: attach this component to gameObject and assign functions to EventOnSceneLoaded.
/// Caution: gameObject attaching this component will execute EventsOnEnable everytime the choosen scene loaded
/// </summary>
public class StartEventsOnSceneLoaded : MonoBehaviour
{
    public bool addEventsOnce = true;
    private bool eventsAdded = false;

    [Header("Optional")]
    public bool ExecuteEventsOnSpecifiedScene = false;

/// <summary>
/// Everytime the scene with this name loaded, EventOnSceneLoaded will be Invoke()
/// </summary>
    public string sceneNameToRunEvent = "HOME";

/// <summary>
/// Everytime the scene with this name loaded, EventOnSceneLoaded will be Invoke()
/// </summary>
    public int sceneIndexToRunEvent = 0;

    public UnityEvent EventOnSceneLoaded;

    public void OnEnable()
    {
        if (eventsAdded) return;

        if (addEventsOnce) eventsAdded = true;
   
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (ExecuteEventsOnSpecifiedScene)
        {
            if (SceneManager.GetActiveScene().name == sceneNameToRunEvent || SceneManager.GetActiveScene().buildIndex == sceneIndexToRunEvent)
                EventOnSceneLoaded.Invoke();
        }
        else { EventOnSceneLoaded.Invoke(); }
       
    }
}
