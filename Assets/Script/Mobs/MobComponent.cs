using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobComponent : MonoBehaviour
{
    public Mob parent;
    protected virtual void Awake()
    {
        if (parent==null)
        parent = GetComponent<Mob>();
    }
}
