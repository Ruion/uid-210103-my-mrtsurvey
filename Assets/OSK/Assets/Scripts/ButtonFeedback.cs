using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFeedback : MonoBehaviour
{
    public Animator anim;

    public void Tap()
    {
        anim.Play("tap", 0, 0);
    }
}
