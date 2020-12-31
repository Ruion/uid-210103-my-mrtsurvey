using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class DuplicateValidator : MonoBehaviour
{
    [HideInInspector]
    public bool valid { get; set; }

    // Start is called before the first frame update
    private void Start()
    {
    }

    //public async virtual Task<bool> Validate(string key)
    public virtual bool Validate(string key)
    {
        return valid;
    }

    public async virtual Task<bool> ValidateAsync(string key, System.Action callback)
    {
        return valid;
    }
}