using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    public PlayerMob parent;
    public virtual void Awake()
    {
        if (parent == null)
            parent = GetComponent<PlayerMob>();
    }
}