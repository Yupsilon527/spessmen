using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPreview : Initializable
{
    public DataItemShip ship;

    public GameObject TokenPrefab;
    public ObjectPool TokenPool;
    protected override void Initialize()
    {
        base.Initialize();
        if (TokenPool == null)
            TokenPool = GetComponent<ObjectPool>();
    }
    public virtual void Clear()
    {

    }
}
