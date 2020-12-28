using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Execute Unity Event whenever left or right swipe is detected. Events for left swipe and right swipe are separated
/// By default it only execute events once and disable itself. Change this behaviour by toggling executeOnce boolean.
/// Tips: Attach SwipeDetector to a gameObject and drag function in to OnSwipeLeft / OnSwipeRight event.
/// </summary>
public class VerticalSwipeDetector : MonoBehaviour
{
    public UnityEvent OnSwipeUp;
    public UnityEvent OnSwipeDown;

    [Header("Swipe Config")]
    /// <summary>
    /// control sensitivity of swipe detection
    /// </summary>
    public float verticalThresholdSwipe = .06f;
    private Vector3 mouseDownPosition;
    private Vector3 mouseUpPosition;
    private float yDistance;
    public bool executeOnce = true;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPosition = Input.mousePosition; //Get mouse down position
        }

        if (Input.GetMouseButtonUp(0))
        {
            mouseUpPosition = Input.mousePosition; //Get mouse position

            yDistance = (mouseDownPosition.y - mouseUpPosition.y) / Screen.height; //Caculate the distance between them

            // Side swiping
            if (Mathf.Abs(yDistance) > verticalThresholdSwipe && mouseDownPosition.y != 0)
            {
                if (yDistance < 0) // Down
                {
                    OnSwipeUp.Invoke();
                }
                else // Up
                {
                    OnSwipeDown.Invoke();
                }

                if (executeOnce)
                {
                    enabled = false;
                }
            }
        }
    }

    public void SwipeUp()
    {
        OnSwipeUp.Invoke(); if (executeOnce)
        {
            enabled = false;
        }
    }

    public void SwipeDown()
    { 
        OnSwipeDown.Invoke(); if (executeOnce)
        {
            enabled = false;
        }
    }
}
