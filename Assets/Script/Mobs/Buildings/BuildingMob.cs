using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMob : Mob
{
    public BoxCollider2D ConstructionCollider;
    public SpriteRenderer BuildingTexture;
    public float BuildCost = 100;
    public float BuildTime = 10;

    public bool CanBeBuildThere(Vector2 center, Transform builder)
    {
        foreach (RaycastHit2D collision in Physics2D.BoxCastAll(center + (Vector2)builder.up * ConstructionCollider.offset.y, ConstructionCollider.size*.5f, builder.rotation.eulerAngles.z, Vector2.zero,0, LayerMask.GetMask( "Foreground" ) ))
        {
            if (collision.transform.tag == "Building")
            {
                Debug.Log("Building " + collision.transform.name + " In The Way");
                return false;
            }
            if (collision.transform.tag == "Terrain")
            {
                Debug.Log("Terrain In The Way");
                return false;
            }
        }
        return true;
    }
    public GameObject BuildCopy(Vector2 center,float percent)
    {
            GameObject bui = GameObject.Instantiate(gameObject);
            bui.transform.position = center;
        if (bui.TryGetComponent(out BuildingMob building))
        {
            building.SetBuildPercentage(percent);
        }
        return bui;
    }
    float buildPercentage = 100f;
    public void IncreaseBuildPercentage(float percentage)
    {
        SetBuildPercentage(buildPercentage + percentage);
    }
    public virtual void SetBuildPercentage(float percent)
    {
        if (BuildingTexture!=null)
        {
            BuildingTexture.size = new Vector2(1, percent*.01f);
        }
        buildPercentage = Mathf.Clamp(percent, 0, 100);

        //SFX building complete sound, only if the buildPercentage is at 100
    }
    public float GetBuildingPercentage()

    {
        return buildPercentage;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BuilderComponent builder))
        {
            builder.activeBuilding = this;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BuilderComponent builder))
        {
            if (builder.activeBuilding == this)
            {
                builder.activeBuilding = null;
            }
        }
    }
}
