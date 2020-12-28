using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reposition gameObjects to target gameObjects destination
/// Tips: drag gameObjects into "newObject", drag destination gameObject into "destinations", 
/// right click component in inspector and select Reposition
/// </summary>
public class ObjectReposition : MonoBehaviour
{
    public List<Transform> destinations;
    public List<Transform> newObject;

    [ContextMenu("Reposition")]
    public void Reposition()
    {
        for (int i = 0; i < destinations.Count; i++)
        {
            newObject[i].position = destinations[i].position;
        }
    }
}
