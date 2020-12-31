using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputToInputDuplication : MonoBehaviour
{
    public TMP_InputField originalInput;
    public TMP_InputField targetInput { get; set; }

    // Start is called before the first frame update
    private void OnEnable()
    {
        originalInput.text = targetInput.text;

        originalInput.onValueChanged.AddListener(delegate
        {
            targetInput.text = originalInput.text;
        });
    }

    private void OnDisable()
    {
        originalInput.onValueChanged.RemoveAllListeners();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}