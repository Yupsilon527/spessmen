using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingComponent : Initializable
{
    public BuildingMob parentMob;
    protected override void Initialize()
    {
        base.Initialize();
        FindComponent(ref  parentMob);
    }
}
