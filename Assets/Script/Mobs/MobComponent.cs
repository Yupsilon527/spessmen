using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobComponent : MonoBehaviour
{
    public Mob Owner;
    private void Awake()
    {
        if (Owner==null)
        Owner = GetComponent<Mob>();
    }
}
