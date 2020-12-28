using UnityEngine;

/// <summary>
/// Destroy a target gameObject after a delay at OnEnable()
/// Tips: Attach to a gameObject, drag target gameObject into "target" field
/// </summary>
public class TimerDestroyer : MonoBehaviour
{
    public float DestroyAfterSeconds;
    public GameObject target;

    public bool destroyOnEnable = true;

    private void OnEnable()
    {
        if(!destroyOnEnable) return;
        Invoke("DestroyTarget", DestroyAfterSeconds);
    }

    public void DestroyTarget()
    {
       Destroy(target);
    }


}
