using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingKitItem : ItemMob
{
    public GameObject BuildingPrefab;
    public override void OnActivate(PlayerMob user)
    {
        if (BuildingPrefab!= null)
        {
            Vector3 buildPos = user.transform.position + user.transform.lossyScale.y * Vector3.down ;
            if (BuildingPrefab.TryGetComponent(out BuildingMob bmob))
            {
                if (!bmob.CanBeBuildThere(buildPos))
                {
                    Debug.Log("InvalidPosition");
                    return;
                }
                bmob.BuildCopy(buildPos,.15f);
                Kill();
            }
        }
    }
}
