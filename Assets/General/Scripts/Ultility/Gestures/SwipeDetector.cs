using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Execute Unity Event whenever left or right swipe is detected. Events for left swipe and right swipe are separated
/// By default it only execute events once and disable itself. Change this behaviour by toggling executeOnce boolean.
/// Tips: Attach SwipeDetector to a gameObject and drag function in to OnSwipeLeft / OnSwipeRight event.
/// </summary>
public class SwipeDetector : MonoBehaviour
{
    public UnityEvent OnSwipeLeft;
    public UnityEvent OnSwipeRight;

    [Header("Swipe Config")]
    /// <summary>
    /// control sensitivity of swipe detection
    /// </summary>
    public float horizontalThresholdSwipe = .06f;
    private Vector3 mouseDownPosition;
    private Vector3 mouseUpPosition;
    private float xDistance;
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

                xDistance = (mouseDownPosition.x - mouseUpPosition.x) / Screen.width; //Caculate the distance between them

                // Side swiping
                if (Mathf.Abs(xDistance) > horizontalThresholdSwipe && mouseDownPosition.x != 0)
                {
                    if (xDistance < 0) // Right
                    {
                        OnSwipeRight.Invoke();
                    }
                    else // Left
                    {
                        OnSwipeLeft.Invoke();
                    }

                    if (executeOnce)
                    {
                        enabled = false;
                    }
                }
        }
    }

}
