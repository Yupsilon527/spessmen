using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobComponent : MonoBehaviour
{
    public Mob Owner;
    protected virtual void Awake()
    {
        if (Owner==null)
        Owner = GetComponent<Mob>();
    }
}
