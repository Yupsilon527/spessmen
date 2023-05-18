using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public ObjectPool activeObjectPool;
    protected virtual void OnDisable()
    {
        if (activeObjectPool != null)
            activeObjectPool.DeactivateObject(gameObject);
    }
}
