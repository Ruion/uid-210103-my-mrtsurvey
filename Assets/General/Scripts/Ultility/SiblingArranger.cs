using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Change the sibling index of gameObject attaching this component at OnEnable()
/// This is use to fix prefab bug in Unity that mesh up child arrangement inside a parent gameObject
/// Tips: Attach to a gameObject, and set desire siblingIndex 
/// Notes: You can also change siblingIndex or call SetSiblingIndex() via script
/// </summary>
public class SiblingArranger : MonoBehaviour
{
    public int siblingIndex;

    [SerializeField]
    private bool arrangeOnEnable = true;

    void OnEnable()
    {
        if(arrangeOnEnable)
        SetSiblingIndex();
    }

    [ContextMenu("SetSibling")]
    public void SetSiblingIndex()
    {
        transform.SetSiblingIndex(siblingIndex);
    }

    public void SetLastSibling()
    {
        transform.SetAsLastSibling();
    }
}
