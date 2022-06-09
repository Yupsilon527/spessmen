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
        if (Input.GetButtonDown("Build") && activeBuilding != null)
            BuildBuilding(activeBuilding);

    }
    public bool TryBuildBuilding(GameObject BuildingPrefab, Vector3 buildPos, float orientation)
    {
        if (BuildingPrefab.TryGetComponent(out BuildingMob bmob))
        {
            Ray2D GroundRay = new Ray2D(buildPos + transform.up, -transform.up);
            RaycastHit2D rhit = Physics2D.Raycast(GroundRay.origin, GroundRay.direction, 10, LayerMask.GetMask("Foreground"));
            if (rhit.collider!=null)
            {
                buildPos = rhit.point;
            }
            else
            {
                buildPos = GroundRay.direction * 3 + GroundRay.origin;
            }

            if (!bmob.CanBeBuildThere(buildPos, orientation))
            {
                InterfaceController.main.ShowWarning("Invalid Building Position",3);
                return false;
            }

            if (!parent.resources.ChargeValue(ResourceController.Resources.wood, bmob.BuildCost * .15f))
            {
                InterfaceController.main.ShowWarning("Not Enough Resources", 3);
                return false;
            }
            GameObject deploy = bmob.BuildCopy(buildPos, 15f);
                if (deploy.TryGetComponent(out BuildingMob buildingmobdata))
                    BuildBuilding(buildingmobdata);
        }
        return true;
    }
    IEnumerator StartBuilding(BuildingMob building)
    {
        parent.movement.CanMove = false;
        float buildPercent = (building.BuildTime > 0) ? (BuildingSkill * 10 * Time.deltaTime / building.BuildTime) : 100;

    loopstart:
        yield return new WaitForEndOfFrame();
        buildPercent = Mathf.Min(buildPercent, 100 - building.GetBuildingPercentage());
        if (parent.resources.ChargeValue(ResourceController.Resources.wood, buildPercent * building.BuildCost * .01f))
        {
            building.IncreaseBuildPercentage(buildPercent);
            if (building == null || building.GetBuildingPercentage() >= 100 || parent.moveInput.x != 0 || Input.GetButtonDown("Jump"))
                StopBuilding();
            else
                goto loopstart;
        }
        else
        {
            StopBuilding();
        }
    }
    void StopBuilding()
    {
        parent.movement.CanMove = true;
        buildCoroutine = null;
    }
    
}
