using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureComponent : MonoBehaviour
{
    public CreatureMob parent;
    public virtual void Awake()
    {
        if (parent == null)
            parent = GetComponent<CreatureMob>();
    }
}
