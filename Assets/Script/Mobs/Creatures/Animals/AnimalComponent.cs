using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalComponent : MonoBehaviour
{
    public AnimalMob parent;
    public virtual void Awake()
    {
        if (parent == null)
            parent = GetComponent<AnimalMob>();
    }
}
