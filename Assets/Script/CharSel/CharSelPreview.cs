using System.Collections.Generic;
using UnityEngine;

public class CharSelPreview : RacerSelComponent
{
    public GameObject parent;
    public ObjectPool previewPool;
    GameObject active;

    public override void AssignScriptable(ShipScriptable ship)
    {
        base.AssignScriptable(ship);
     
        if (active != null) { previewPool.DeactivateObject(active); }
        active =  previewPool.PoolItem(ship.prefab,parent);
        active.transform.localPosition = Vector3.zero;
        active.transform.localRotation = Quaternion.identity;
    }
}
