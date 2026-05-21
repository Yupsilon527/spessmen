using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentPool : MonoBehaviour
{
    HashSet<Behaviour> inactiveObjs = new();
    public virtual tComponent PoolComponent<tComponent>(bool active = false) where tComponent : Behaviour
    {
        foreach (var child in inactiveObjs)
        {
            if (child != null && child is tComponent found)
            {
                found.enabled = active;
                inactiveObjs.Remove(child);
                return found;
            }
        }
        return gameObject.AddComponent<tComponent>();
    }

    public virtual void DeactivateComponent(Behaviour tgt)
    {
        if (tgt == null) return;
        tgt.enabled = false;
        inactiveObjs.Add(tgt);
    }

}
