using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    public DataItemPlayer player;
    protected virtual void Awake()
    {
        if (player == null)
            player = GetComponent<DataItemPlayer>();
    }
    public virtual void Setup()
    {

    }
    protected virtual void OnValidate()
    {
        if (player == null)
            player = GetComponent<DataItemPlayer>();
    }
}
