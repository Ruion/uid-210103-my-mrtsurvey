using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class JSONTester : MonoBehaviour
{
    public JSONSetter jsonSetter;
    public string property;
    public string value_;

    [Button(ButtonSizes.Medium)]
    public void AddProperty()
    {
        jsonSetter.UpdateSetting(property, value_);
    }
}
