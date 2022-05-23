using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderComponent : MobComponent
{
    public BuildingMob activeBuilding;
    public int BuildingSkill = 10;
    Coroutine buildCoroutine;
    public Player parent;
    public void BuildBuilding(BuildingMob building)
    {
        if (buildCoroutine!=null)
        {
            StopCoroutine(buildCoroutine);
            StopBuilding();
        }
        activeBuilding = building;
       StartCoroutine( StartBuilding(activeBuilding));
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && activeBuilding != null)
            BuildBuilding(activeBuilding);

    }
    public bool TryBuildBuilding(GameObject BuildingPrefab, Vector3 buildPos)
    {
            if (BuildingPrefab.TryGetComponent(out BuildingMob bmob))
            {
                if (!bmob.CanBeBuildThere(buildPos))
                {
                    Debug.Log("InvalidPosition");
                    return false;
                }
                GameObject deploy = bmob.BuildCopy(buildPos, .15f);
                if (deploy.TryGetComponent(out BuildingMob buildingmobdata))
                    BuildBuilding(buildingmobdata);
            }
        return true;
    }
    IEnumerator StartBuilding(BuildingMob building)
    {
        parent.movement.CanMove = false;
    loopstart:
        yield return new WaitForEndOfFrame();
        Debug.Log("BuildWait");
        if (building.BuildTime>0)
        building.IncreaseBuildPercentage(BuildingSkill * 10 * Time.deltaTime / building.BuildTime);
        else
            building.IncreaseBuildPercentage(100);

        if (building==null || building.GetBuildingPercentage() >= 100 || Input.GetAxis("Horizontal")!= 0 ||Input.GetKeyDown(KeyCode.Space))
        StopBuilding();
        else
        goto loopstart;
    }
    void StopBuilding()
    {
        parent.movement.CanMove = true;
        buildCoroutine = null;
    }
    
}
