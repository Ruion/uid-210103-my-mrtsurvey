using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// Secure the Admin Page by using password. Person type in correct password in passwordInput field
/// grant control of Admin Page
/// Tips: Attach to AdminPage gameObject, set password in inspector, drag TMP_InputFIeld 
/// and function to execute in OnPasswordCorrect event
/// </summary>
public class AdminValidation : MonoBehaviour
{
    public string password = "hondaBoss";
    public TMP_InputField passwordInput;

    public UnityEvent OnPasswordCorrect;


    public virtual void Validate()
    {
        if(passwordInput.text == password)
        {
            OnPasswordCorrect.Invoke();
            passwordInput.text = "";
           
        }
    }
}
