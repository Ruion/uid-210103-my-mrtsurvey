using UnityEngine;

/// <summary>
/// Make object persist across scene
/// Tips: Attach to a gameObject and set the tag of gameObject and tagName of this component
/// Notes: only the first and one object with this tagName object will persist, other gameObject
/// with same tagName will be destroyed
/// </summary>
public class DontDestroy : MonoBehaviour
{
    public string tagName;

    void Awake()
    {
        transform.SetParent(null, false);

        gameObject.tag = tagName;
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tagName);

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}