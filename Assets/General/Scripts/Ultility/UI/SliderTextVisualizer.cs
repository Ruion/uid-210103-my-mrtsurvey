using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Visual a slider value to a TextMeshProUGUI component
/// By default it only execute events once and disable itself. Change this behaviour by toggling executeOnce boolean.
/// Tips: Attach SwipeDetector to a gameObject and drag function in to OnSwipeLeft / OnSwipeRight event.
/// </summary>
public class SliderTextVisualizer : MonoBehaviour
{
    public TextMeshProUGUI text_;
    private Slider slider_;

    private void OnEnable()
    {
        slider_ = GetComponent<Slider>();
    }

    public void VisualizeSliderValue()
    {
        text_.text =
            slider_.value.ToString();
    }
}